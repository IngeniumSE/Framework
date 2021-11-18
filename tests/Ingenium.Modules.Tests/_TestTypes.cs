using Ingenium.Modules;

public class TestModule : Module { }

[Module(
	id: "CustomModuleId",
	name: "Other Module",
	description: "An example module using a ModuleAttribute",
	dependencies: new[] { "Test" })]
public class OtherModule : Module { }