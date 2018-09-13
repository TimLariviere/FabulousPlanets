#r "paket:
nuget Fake.Core.Target
nuget Fake.IO.FileSystem
nuget Fake.DotNet.MSBuild //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.DotNet

let buildDir = "./build/"

Target.create "Clean" (fun _ ->
    Shell.cleanDir buildDir
)

Target.create "Build" (fun _ ->
    // Restore before build
    !! "**/*.fsproj"
    |> MSBuild.runRelease id buildDir "Restore"
    |> Trace.logItems "AppRestore-Output: "

    // Build
    !! "**/*.fsproj"
    |> MSBuild.runRelease id buildDir "Build"
    |> Trace.logItems "AppBuild-Output: "
)

Target.create "Default" ignore

open Fake.Core.TargetOperators

"Clean"
 ==> "Build"
 ==> "Default"

Target.runOrDefault "Default"