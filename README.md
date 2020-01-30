# Git Working Time

Dotnet tool to print console graph with work hours of any repository collaborator. Dotnet version of [Famous programmers work time](https://ivan.bessarabov.com/blog/famous-programmers-work-time) and its second part [Workweek vs Weekend](https://ivan.bessarabov.com/blog/famous-programmers-work-time-part-2-workweek-vs-weekend)

### Prerequisites

* [Linux](https://www.distrowatch.com/)
* [Git](https://git-scm.com/)
* [.Net Core](https://dotnet.microsoft.com/download)

### Installing

Install as tool from the nuget repository.

```
$ dotnet tool install --global GitWorkingTime
```

## Getting Started

```
$ git clone https://github.com/memcached/memcached memcached

## Print help
$ dotnet-worktime --help

GitWorkingTime 1.0.0
Copyright (C) 2020 GitWorkingTime

  --authors    Required. Get their work time combined

  --repo       Required. Local path to the repository to parse

  --help       Display this help screen.

  --version    Display version information.

## Print work time of Brad Fitzpatrick
$ dotnet-worktime -- --authors='Brad Fitzpatrick' --repo='memcached'

  hour          Monday to Friday                       Saturday and Sunday           
    00       11 ******************                   0                               
    01        8 *************                        2 ***                           
    02       15 *************************            2 ***                           
    03        6 **********                           1 *                             
    04        7 ***********                          0                               
    05       12 ********************                 1 *                             
    06        8 *************                        0                               
    07        8 *************                        0                               
    08        2 ***                                  0                               
    09        0                                      0                               
    10        3 *****                                0                               
    11        1 *                                    0                               
    12        0                                      0                               
    13        0                                      0                               
    14        0                                      0                               
    15        0                                      0                               
    16        3 *****                                1 *                             
    17        7 ***********                          1 *                             
    18        9 ***************                      0                               
    19        8 *************                        1 *                             
    20       10 ****************                     2 ***                           
    21        8 *************                        2 ***                           
    22        9 ***************                      2 ***                           
    23        9 ***************                      5 ********                      

Total:      144 (87.8%)                             20 (12.2%)

## Print combined work time of multiple authors, separated by ';' character. Most useful when one
## author has collaborated with single or multiple aliases (not the case of this example)
$ dotnet-worktime -- --authors='Brad Fitzpatrick;dormando' --repo='memcached'

  hour          Monday to Friday                       Saturday and Sunday           
    00       57 *************************            5 **                            
    01       27 ***********                         17 *******                       
    02       24 **********                           4 *                             
    03        9 ***                                  1                               
    04       11 ****                                 0                               
    05       23 **********                           1                               
    06       10 ****                                 0                               
    07       12 *****                                0                               
    08        7 ***                                  2                               
    09        5 **                                   3 *                             
    10       10 ****                                 3 *                             
    11       16 *******                              8 ***                           
    12       23 **********                           9 ***                           
    13       18 *******                              4 *                             
    14       26 ***********                          2                               
    15       24 **********                          23 **********                    
    16       41 *****************                   17 *******                       
    17       41 *****************                   16 *******                       
    18       36 ***************                     10 ****                          
    19       28 ************                        13 *****                         
    20       21 *********                           12 *****                         
    21       23 **********                          17 *******                       
    22       31 *************                       10 ****                          
    23       34 **************                      20 ********                      

Total:      557 (73.9%)                            197 (26.1%)
```

## Running the tests

```
$ git clone https://github.com/Idrek/GitWorkingTime GitWorkingTime && cd $_

$ dotnet test

Test Run Successful.
Total tests: 12
     Passed: 12
 Total time: 1,4480 Seconds
```

## Acknowledgments

* Ivan Bessarabov and his blog entries [part1](https://ivan.bessarabov.com/blog/famous-programmers-work-time) and [Workweek vs Weekend](https://ivan.bessarabov.com/blog/famous-programmers-work-time-part-2-workweek-vs-weekend)

