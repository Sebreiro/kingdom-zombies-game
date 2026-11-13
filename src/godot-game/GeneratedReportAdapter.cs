using System.Diagnostics.CodeAnalysis;

// The pinned generator owns this exact method. ConfigurationTests verifies generator origin,
// rejects a handwritten method collision, and proves adjacent Console calls still fail.
[assembly: SuppressMessage("ApiDesign", "RS0030", Scope = "member", Target = "~M:GodotPlugins.Game.Main.InitializeFromGameProject(System.IntPtr,System.IntPtr,System.IntPtr,System.Int32)", Justification = "Godot 4.7.2 generated native-load failure report; KZ-EX-GODOT-REPORT-01 in docs/code/engine-adaptations.json")]
