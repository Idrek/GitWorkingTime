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

// ---------------------------------
// initHours
// ---------------------------------     

[<Fact>]
let ``Test initHours function`` () =
    Assert.Equal<Map<int, int>>(
        Map [(0, 0); (1, 0); (2, 0); (3, 0); (4, 0); (5, 0); (6, 0); (7, 0); (8, 0); (9, 0); (10, 0)
             (11, 0); (12, 0); (13, 0); (14, 0); (15, 0); (16, 0); (17, 0); (18, 0); (19, 0)
             (20, 0); (21, 0); (22, 0); (23, 0)],
        initHours ())

// ---------------------------------
// updateWithDefault
// ---------------------------------        

[<Fact>]
let ``Test updateWithDefault function`` () = 
    Assert.Equal<Map<int, string>>(
        Map.ofArray [|(1, "one")|],
        updateWithDefault ((+) "-suffix") 1 "one" Map.empty)
    Assert.Equal<Map<int, string>>(
        Map.ofArray [|(1, "prefix-ONE")|],
        updateWithDefault ((+) "prefix-") 1 "one" <| Map.ofArray [|(1, "ONE")|])
    Assert.Equal<Map<int, string>>(
        Map.ofArray [|(1, "one"); (2, "two")|],
        updateWithDefault ((+) "prefix-") 1 "one" <| Map.ofArray [|(2, "two")|])        

// ---------------------------------
// groupBy
// ---------------------------------

[<Fact>]
let ``Test groupBy function`` () =
    Assert.Equal<Map<int, int>>(Map.empty, groupBy id Array.empty)
    Assert.Equal<Map<int, int>>(Map.ofArray [|(1, 1);|], groupBy id [|1|])
    Assert.Equal<Map<int, int>>(
        Map.ofArray [|(0, 4); (1, 2)|], 
        groupBy (fun n -> n % 2) [|2;4;6;7;8;9|])

