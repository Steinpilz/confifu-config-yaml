#r @"..\packages\FAKE\tools\FakeLib.dll"
#r @"..\packages\Fria2.FakeBuild\tools\Steinpilz.DevFlow.Fake.Lib.dll"
#r @"..\packages\Fria2.FakeBuild\tools\Fria2.FakeBuild.Lib.dll"

open Fake
open Fria2.FakeBuild
open Steinpilz.DevFlow.Fake

let libParams = Fria2.FakeBuild.Lib.setup <| fun p -> 
    { p with 
        AppProjects = !!"src/app/**/*.*proj"
        TestProjects = !!"src/test/**/*.*proj"
        PublishProjects = !! "src/app/**/*.*proj"

        UseDotNetCliToPack = true
        // You can use NuGet command line tools as a fallback:
        //UseNuGetToPack = true
        //UseNuGetToRestore = true
        //NuGetFeed = 
        //    { p.NuGetFeed with 
        //        ApiKey = environVarOrFail <| "NUGET_API_KEY" |> Some
        //    }
    }

RunTargetOrDefault "Watch"