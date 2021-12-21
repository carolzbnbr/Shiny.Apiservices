﻿using Cake.Common.IO;
using Cake.Common.Tools.MSBuild;
using Cake.Frosting;


namespace ShinyBuild.Tasks.Library
{
    [TaskName("Build")]
    public sealed class BuildTask : FrostingTask<BuildContext>
    {
        public override bool ShouldRun(BuildContext context)
        {
            if (context.IsRunningInCI && context.BuildNumber == 0)
                throw new ArgumentException("BuildNumber argument is missing");

            return true;
        }


        public override void Run(BuildContext context)
        {
            context.CleanDirectories($"./src/**/obj/");
            context.CleanDirectories($"./src/**/bin/{context.MsBuildConfiguration}");

            context.MSBuild("Build.slnf", x => x
                .WithRestore()
                .WithTarget("Clean")
                .WithTarget("Build")
                .WithProperty("ShinyVersion", context.NugetVersion)
                .WithProperty("CI", context.IsRunningInCI ? "true" : "")
                .SetConfiguration(context.MsBuildConfiguration)
            );
        }
    }
}