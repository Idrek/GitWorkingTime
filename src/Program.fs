// Learn more about F# at http://fsharp.org
module GitWorkingTime.Program

open CommandLine

type Bash = Shell.NET.Bash
type DateTime = System.DateTime
type DayOfWeek = System.DayOfWeek
type Match = System.Text.RegularExpressions.Match
type OptionAttribute = CommandLine.OptionAttribute
type Regex = System.Text.RegularExpressions.Regex
type String = System.String

type Options = {
    [<Option("author", Required = true, HelpText = "Get its work time")>]
    Author: string

    [<Option("repo", Required = true, HelpText = "Local path to the repository to parse")>]
    Repository: string
}

type ExitCode = 
    | Success = 0
    | CommandLineParseError = 1
    | CommandLineNotParsed = 2

let isWeekend (date: DateTime) : bool =
    date.DayOfWeek = DayOfWeek.Saturday || date.DayOfWeek = DayOfWeek.Sunday

let runCommand (bash: Bash) (command: string) : array<string> =
    bash.Command(command).Lines
let words (s: string) : array<string> =
    match s with
    | s when String.IsNullOrEmpty s -> Array.empty
    | s -> Regex.Split(s.Trim(), @"\s+")

let authorHours (authorLog: array<string>) : array<int> =
    authorLog
    |> Array.choose (fun line ->
        let m : Match = Regex.Match(line, @"^Date:\s+[\d-]{10}\s+(\d{2})")
        if m.Success = false then None else m.Groups.[1].Value |> int |> Some)

let initHours () : Map<int, int> =
    let dayHours = 24
    let commits = 0
    Seq.init dayHours (fun h -> (h, commits)) |> Map.ofSeq
    
let hour (time: string) : int =
    time.Trim().Split(":", 2).[0] |> int

let updateWithDefault 
        (f: 'b -> 'b) 
        (key: 'a) 
        (defaultValue: 'b) 
        (m: Map<'a, 'b>) 
        : Map<'a, 'b> =
    match Map.tryFind key m with
    | None -> Map.add key defaultValue m
    | Some value -> Map.add key (f value) m

let groupBy (f: 'a -> 'b) (arr: array<'a>) : Map<'b, int> =
    let lastArrPosition = Array.length arr - 1
    Array.foldBack
        (fun x (index, map) ->
            let r = f x
            let defaultValue = 1
            let updatedMap = updateWithDefault (fun (v: int) -> v + 1) r defaultValue map
            (index - 1, updatedMap))
        arr
        (lastArrPosition, (Map.empty<'b, int>))
    |> snd

let groupCommitsByHour (hours: array<int>) : Map<int, int> =
    groupBy id hours
let buildCommandLogByAuthor (options: Options) : string = 
    sprintf "cd '%s' && git --no-pager log --author='%s' --format='%%H %%ai'" 
        options.Repository 
        options.Author

let merge (m1: Map<'a, 'b>) (m2: Map<'a, 'b>) : Map<'a, 'b> =
    Map.fold (fun acc k v -> Map.add k v acc) m1 m2

let maxHourCommits (hours: Map<_, int>) : int =
    if Map.isEmpty hours
    then 0
    else hours |> Map.toArray |> Array.maxBy snd |> snd

type Hours = {
    Weekend: Map<int, int>
    Workdays: Map<int, int>
} with
    static member maxCommits ({ Weekend = weekend; Workdays = workdays}: Hours) : int =
        max (maxHourCommits weekend) (maxHourCommits workdays)

let repeat (count: int) (str: string) : string =
    String.init count (fun _ -> str)

let printHourChart (maxCommits: int) (hours: Map<int, int>) : unit =
    let fMaxCommits = float maxCommits
    hours
    |> Map.toArray
    |> Array.iter (fun (hour, commits) -> 
        printfn "%02d | %4d %s" hour commits (repeat (int (float commits / fMaxCommits * 50.0)) "*"))

[<EntryPoint>]
let main argv =
    let commandLine = CommandLine.Parser.Default.ParseArguments<Options>(argv)
    match commandLine with
    | :? CommandLine.NotParsed<Options> -> int ExitCode.CommandLineNotParsed
    | :? CommandLine.Parsed<Options> as parsed -> 
        let bash : Bash = new Bash()
        let options : Options = parsed.Value
        let authorLog : array<string> = options |> buildCommandLogByAuthor |> runCommand bash
        match authorLog with
        | [||] | [|""|] -> ()
        | authorLog' -> 
            let emptyHours : Map<int, int> = initHours ()
            let workHours : array<int> = authorHours authorLog'
            let allHours : Map<int, int> = workHours |> groupCommitsByHour |> merge emptyHours
            let maxCommits : int = maxHourCommits allHours
            printHourChart maxCommits allHours           
        int ExitCode.Success
    | _ -> 
        failwith "Error: CommandLine.Parser.Default.ParseArguments<options>(...)"
        int ExitCode.CommandLineParseError
        