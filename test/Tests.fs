module GitWorkingTimeTest.Tests

open GitWorkingTime.Program
open Xunit

type DateTime = System.DateTime

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
        "cd '../Demo' && git --no-pager log --author='me' --format='%H %ai'",
        buildCommandLogByAuthor options)

    let options : Options = { Author = "he and she"; Repository = "../Demo space" }
    Assert.Equal(
        "cd '../Demo space' && git --no-pager log --author='he and she' --format='%H %ai'",
        buildCommandLogByAuthor options)

// ---------------------------------
// authorHours
// ---------------------------------     

[<Fact>]
let ``Test authorHours function`` () =
    let authorLog = [|
        "affed15b38f4157715066d805f43b4facec21ee5 2019-10-06 15:44:15 +0200"
        "9b9cb4a4a7350835dce58acba7a4a5c7a78b40f7 2019-10-05 15:43:34 +0200"
        "3867261383436a01220d6b9b0b03358db457446c 2019-10-05 13:46:48 +0200"
        "c9e2a5f5cfa06b470073c3c1ed8428c3ed82f79d 2019-10-05 13:40:44 +0200"
        "d5a687b84df5cecccffc78b630f61887ece9f0fa 2019-10-05 13:36:23 +0200"
        "31ed5b9f989033a85c3c5ae848d79f26dcbabb66 2019-10-05 13:18:26 +0200"
        "b2b1e10402828f69dde0e6e837a638ac549fd65c 2019-10-04 13:17:43 +0200"
        "63dec9ba36a51f111bb4531788d9c880619906dd 2019-10-03 13:10:23 +0200"
        "ff24930aeff8b4c5cdd9422e4ef0ffdd40d48cfd 2019-10-03 13:07:11 +0200"
        "b805342974b4cb97736795822bd090e947c8dd90 2019-10-02 13:06:34 +0200"
        "396eff88477a580b928b62e9ca762cb9720bb39c 2019-10-02 13:04:00 +0200"
        "a1381829627d9f5c77ba0fc8840703c91d14c771 2019-10-02 13:03:32 +0200"
        "d88a8de729bc6887425ffbb77af27645139defae 2019-10-02 12:58:23 +0200"
    |]
    Assert.Equal<Hours>(
        { 
            Weekend = Map [|(13, 4); (15, 2)|]
            Workdays = Map [|(12, 1); (13, 6)|]
        },
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
// merge
// ---------------------------------        

[<Fact>]
let ``Test merge function`` () =
    Assert.Equal<Map<int, string>>(Map.empty, merge Map.empty Map.empty)
    Assert.Equal<Map<int, string>>(
        Map.ofArray [|(1, "one"); (2, "two")|], 
        merge Map.empty <| Map.ofArray [|(1, "one"); (2, "two")|])
    Assert.Equal<Map<int, string>>(
        Map.ofArray [|(1, "one"); (2, "two")|], 
        merge (Map.ofArray [|(1, "one"); (2, "two")|]) Map.empty)
    Assert.Equal<Map<int, string>>(
        Map.ofArray [|(1, "one"); (2, "two")|], 
        merge (Map.ofArray [|(1, "one")|]) (Map.ofArray [|(2, "two")|]))
    Assert.Equal<Map<int, string>>(
        Map.ofArray [|(1, "one"); (2, "TWO")|], 
        merge (Map.ofArray [|(1, "one"); (2, "two")|]) (Map.ofArray [|(2, "TWO")|]))                          
    Assert.Equal<Map<int, string>>(
        Map.ofArray [|(1, "ONE"); (2, "TWO")|], 
        merge (Map.ofArray [|(1, "one"); (2, "two")|]) (Map.ofArray [|(1, "ONE"); (2, "TWO")|]))    
    Assert.Equal<Map<int, string>>(
        Map.ofArray [|(1, "one"); (2, "TWO"); (3, "three")|], 
        merge (Map.ofArray [|(1, "one"); (2, "two")|]) (Map.ofArray [|(3, "three"); (2, "TWO")|]))        

// ---------------------------------
// maxHourCommits
// ---------------------------------        

[<Fact>]
let ``Test maxHourCommits function`` () =
    Assert.Equal(0, Map.empty |> maxHourCommits)
    Assert.Equal(9, Map [(4, 1); (14, 6); (2, 9); (3, 7)] |> maxHourCommits)

// ---------------------------------
// repeat
// ---------------------------------        

[<Fact>]
let ``Test repeat function`` () =
    Assert.Equal("aaaa", repeat 4 "a")
    Assert.Equal("abcabcabc", repeat 3 "abc")
    
// ---------------------------------
// isWeekend
// ---------------------------------        

[<Fact>]
let ``Test isWeekend function`` () =
    let date = DateTime(2019, 10, 4)
    Assert.Equal(false, isWeekend date)

    let date = DateTime(2019, 10, 5)
    Assert.Equal(true, isWeekend date)

// ---------------------------------
// words
// ---------------------------------        

[<Fact>]
let ``Test words function`` () =
    Assert.Equal<array<string>>(Array.empty, words "")
    Assert.Equal<array<string>>([|""|], words "    \n    ")
    Assert.Equal<array<string>>([|"one"|], words "one")
    Assert.Equal<array<string>>([|"one"|], words "one   ")
    Assert.Equal<array<string>>([|"one"|], words "one\n")
    Assert.Equal<array<string>>([|"Lorem"; "ipsum"; "dolor"|], words "Lorem ipsum\ndolor")
    Assert.Equal<array<string>>([|"Beth"; "4.00"; "0"|], words "Beth\t4.00\t0")

// ---------------------------------
// hour
// ---------------------------------        

[<Fact>]
let ``Test hour function`` () =
    Assert.Equal(16, hour "16:24:51")
    Assert.Equal(0, hour "00:24:51")

// ---------------------------------
// Hours.maxCommits
// ---------------------------------        

[<Fact>]
let ``Test Hours.maxCommits function`` () =
    let hours : Hours = {
        Weekend = Map [|(16, 2); (18, 19); (21, 7)|]
        Workdays = Map [|(11, 8); (12, 14); (20, 3); (23, 1)|]
    }
    Assert.Equal(19, Hours.maxCommits hours)


    