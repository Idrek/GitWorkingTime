﻿// Learn more about F# at http://fsharp.org
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
    [<Option("authors", Required = true, Separator = ';', HelpText = "Get their work time combined. \nUse `git shortlog --summary --numbered` for a list of authors to choose from.")>]
    Authors: seq<string>

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

let buildCommandLogByAuthor (options: Options) : string = 
    let authors = options.Authors |> Seq.map (sprintf "--author='%s'") |> String.concat " "
    sprintf "cd '%s' && git --no-pager log %s --format='%%H %%ai'" 
        options.Repository 
        authors

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

let authorHours (authorLog: array<string>) : Hours =
    let emptyHours = { Weekend = Map.empty; Workdays = Map.empty }
    (emptyHours, authorLog)
    ||> Array.fold 
        (fun hours line ->
            match (words line).[1 .. 2] with
            | [|date; time|] ->
                let h : int = hour time
                if isWeekend (date |> DateTime.Parse)
                then { hours with Weekend = updateWithDefault ((+) 1) h 1 hours.Weekend }
                else { hours with Workdays = updateWithDefault ((+) 1) h 1 hours.Workdays }
            | _ -> failwith "Error: Parsing git log")         
 
let sumCommits (hours: Map<_, int>) : int =
    if Map.isEmpty hours
    then 0
    else hours |> Map.toArray |> Array.sumBy snd

let repeat (count: int) (str: string) : string =
    String.init count (fun _ -> str)

let printHourChart (maxCommits: int) ({ Weekend = weekend; Workdays = workdays}: Hours) : unit =
    let fMaxCommits = float maxCommits
    let weekendCommits = sumCommits weekend
    let workDaysCommits = sumCommits workdays   
    printfn "%6s   %6s %-30s  %6s %-30s" "hour" "" "Monday to Friday" "" "Saturday and Sunday"
    workdays
    |> Map.toArray 
    |> Array.iter (fun (hour, commits) -> 
        let (scommits, fcommits) = (string commits, float commits)
        let weekendHourCommits = Map.find hour weekend
        printfn "%6s   %6s %-30s  %6s %-30s" 
            (sprintf "%02d" hour) 
            scommits
            (repeat (int (fcommits / fMaxCommits * 25.0)) "*")
            (string weekendHourCommits)
            (repeat (int (float weekendHourCommits / fMaxCommits * 25.0)) "*"))
    let totalCommits = weekendCommits + workDaysCommits |> float            
    printfn "\n%6s   %6s %-30s  %6s %-30s"
        "Total:"
        (string workDaysCommits)
        (sprintf "(%.1f%%)" ((float workDaysCommits) * 100.0 / totalCommits))
        (string weekendCommits)
        (sprintf "(%.1f%%)" ((float weekendCommits) * 100.0 / totalCommits))

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
            let hours : Hours = authorHours authorLog'
            let maxCommits : int = Hours.maxCommits hours
            let allHours : Hours = { 
                hours with
                    Weekend = hours.Weekend |> merge emptyHours
                    Workdays = hours.Workdays |> merge emptyHours
            }
            printHourChart maxCommits allHours           
        int ExitCode.Success
    | _ -> 
        failwith "Error: CommandLine.Parser.Default.ParseArguments<options>(...)"
        int ExitCode.CommandLineParseError
        