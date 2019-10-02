// Learn more about F# at http://fsharp.org

open CommandLine
open System

type OptionAttribute = CommandLine.OptionAttribute

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
        