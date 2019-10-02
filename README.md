# Git Working Time

Print console graph with work hours of any repository collaborator. Dotnet version of [Famous programmers work time](https://ivan.bessarabov.com/blog/famous-programmers-work-time)

## Getting Started

```
git clone https://github.com/Idrek/GitWorkingTime GitWorkingTime
git clone https://github.com/memcached/memcached memcached
cd GitWorkingTime
dotnet run --project=src/GitWorkingTime.fsproj -- --author='Brad Fitzpatrick' --repo='../memcached'
```

### Prerequisites

* [Linux](https://www.distrowatch.com/)
* [Git](https://git-scm.com/)
* [.Net Core](https://dotnet.microsoft.com/download)

### Installing

```
git clone https://github.com/Idrek/GitWorkingTime GitWorkingTime && cd $_
dotnet build --output /tmp/build src/GitWorkingTime.fsproj
cd /tmp/build
printf '#!/usr/bin/env bash\n\ndotnet /tmp/build/GitWorkingTime.dll "$@"\n' > git_working_time.sh
chmod u+x git_working_time.sh
git clone https://github.com/memcached/memcached memcached
./git_working_time.sh --author='Brad Fitzpatrick' --repo='memcached'
```

## Running the tests

```
git clone https://github.com/Idrek/GitWorkingTime GitWorkingTime && cd $_
dotnet test test/GitWorkingTimeTest.fsproj
```

## Acknowledgments

* Ivan Bessarabov and its [blog entry](https://ivan.bessarabov.com/blog/famous-programmers-work-time)

