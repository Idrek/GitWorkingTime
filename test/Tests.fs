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

// ---------------------------------
// authorHours
// ---------------------------------     

[<Fact>]
let ``Test authorHours function`` () =
    let authorLog = [|
        "commit c75e48cc89212868bd3980232bf56705a6db8146"
        "Author: Me <false@gmail.com>"
        "Date:   2019-09-27 17:40:01 +0200"
        ""
        "New Demo type"
        ""
        "commit 42e1657989773cdc45441e2b27f68d20e557e74d"
        "Author: Me <false@gmail.com>"
        "Date:   2019-09-25 12:40:47 +0200"
        ""
        "Create a bug"
        ""
    |]
    Assert.Equal<array<int>>(
        [|17; 12|],
        authorHours authorLog)
        