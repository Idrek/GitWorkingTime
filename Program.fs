// Learn more about F# at http://fsharp.org

open System

type OptionAttribute = CommandLine.OptionAttribute

type Options = {
    [<Option("author", Required = true, HelpText = "Get its work time")>]
    Author: string
}

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    0 // return an integer exit code
