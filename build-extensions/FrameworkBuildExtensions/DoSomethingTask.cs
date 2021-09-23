namespace FrameworkBuildExtensions
{
	using Build;
	using Build.Tasks;

	using Cake.Core.Diagnostics;
	using Cake.Frosting;

	[TaskName("DoSomething")]
	[IsDependeeOf(typeof(ResolveVerson))]
	public class DoSomethingTask : BuildTask
	{
		protected override void RunCore(BuildContext context)
		{
			context.Log.Write(
				Verbosity.Normal,
				LogLevel.Error,
				"Hello from build extension");
		}
	}

	public class AfterBuildHook : BuildHook
	{
		public override void BeforeBuild(BuildContext context, BuildProject project)
		{
			context.Log.Information($"Before building project: {project.Name}");
		}

		public override void AfterBuild(BuildContext context, BuildProject project)
		{
			context.Log.Information($"After building project: {project.Name}");
		}
	}
}
