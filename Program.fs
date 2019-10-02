﻿// Learn more about F# at http://fsharp.org

open CommandLine
open System

type Bash = Shell.NET.Bash
type Match = System.Text.RegularExpressions.Match
type OptionAttribute = CommandLine.OptionAttribute
type Regex = System.Text.RegularExpressions.Regex

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

let buildCommandLogByAuthor (options: Options) : string = 
    sprintf "cd %s && git --no-pager log --author='%s' --date=iso" 
        options.Repository 
        options.Author

let runCommand (bash: Bash) (command: string) : array<string> =
    bash.Command(command).Lines

let authorHours (authorLog: array<string>) : array<int> =
    authorLog
    |> Array.choose (fun line ->
        let m : Match = Regex.Match(line, @"^Date:\s+[\d-]{10}\s+(\d{2})")
        if m.Success = false then None else m.Groups.[1].Value |> int |> Some)

let initHours () : Map<int, int> =
    let dayHours = 24
    let commits = 0
    Seq.init dayHours (fun h -> (h, commits)) |> Map.ofSeq

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

let merge (m1: Map<'a, 'b>) (m2: Map<'a, 'b>) : Map<'a, 'b> =
    Map.fold (fun acc k v -> Map.add k v acc) m1 m2

let maxHourCommits (hours: Map<_, int>) : int =
    hours |> Map.toArray |> Array.maxBy snd |> snd

let repeat (count: int) (str: string) : string =
    String.init count (fun _ -> str)

[<EntryPoint>]
let main argv =
    let commandLineR = CommandLine.Parser.Default.ParseArguments<Options>(argv)
    match commandLineR with
    | :? CommandLine.NotParsed<Options> -> int ExitCode.CommandLineNotParsed
    | :? CommandLine.Parsed<Options> as parsed -> 
        printfn "Options: %A" <| parsed.Value
        int ExitCode.Success
    | _ -> 
        failwith "Error: CommandLine.Parser.Default.ParseArguments<options>(...)"
        int ExitCode.CommandLineParseError
        