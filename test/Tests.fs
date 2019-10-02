module GitWorkingTimeTest.Tests

open GitWorkingTime.Program
open Xunit

// ---------------------------------
// Tests
// ---------------------------------

// ---------------------------------
// buildCommandLogByAuthor
// ---------------------------------     

[<Fact>]
let ``Test buildCommandLogByAuthor function`` () =
    let options : Options = { Author = "me"; Repository = "../Demo" }
    Assert.Equal(
        "cd '../Demo' && git --no-pager log --author='me' --date=iso",
        buildCommandLogByAuthor options)

    let options : Options = { Author = "he and she"; Repository = "../Demo space" }
    Assert.Equal(
        "cd '../Demo space' && git --no-pager log --author='he and she' --date=iso",
        buildCommandLogByAuthor options)

