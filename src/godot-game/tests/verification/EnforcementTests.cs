using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Requires actual clean and intended-negative builds under the repository policy.</summary>
public sealed class EnforcementTests
{
    /// <summary>A real copied Core consumer rejects precisely the sixty-first body line.</summary>
    [Fact, Trait("Case", "rules.build-wiring")]
    public void Build_SixtyFirstBodyLine_ReportsScr0006()
    {
        var fixture = new BuildFixture();
        fixture.Restore();
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
        fixture.Write(source: BuildFixture.CleanSource.Replace(oldValue: "        return 1;",
                newValue: new string(c: '\n', count: 60) + "        return 1;",
                comparisonType: StringComparison.Ordinal));
        (int Exit, string Output) changed = fixture.Build();
        BuildFixture.HasDiagnostic(result: changed, id: "SCR0006", line: 6, column: 25).ShouldBeTrue(
                customMessage: changed.Output);
        ProofProtocol.Record(id: "rules.build-wiring", diagnostic: "REAL_BUILD_SCR0006", location: fixture
                .DirectoryPath + "/src/godot-game/core/Subject.cs(6,25)");
    }

    /// <summary>Exercises R-01 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-01.positive")]
    public void Rule01_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-01.positive", diagnostic: "", line: 0,
                column: 0, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static int "
                        + "Read()\n    {\n        return 1;\n    }\n}\n");
    }

    /// <summary>Exercises R-01 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-01.negative")]
    public void Rule01_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-01.negative", diagnostic: "CS0168", line: 8,
                column: 13, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static int "
                        + "Read()\n    {\n        int unused;\n        return 1;\n    }\n}\n");
    }

    /// <summary>Exercises R-11 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-11.positive")]
    public void Rule11_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-11.positive", diagnostic: "", line: 0,
                column: 0, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static int "
                        + "Read()\n    {\n        return 1;\n    }\n}\n");
    }

    /// <summary>Exercises R-11 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-11.negative")]
    public void Rule11_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-11.negative", diagnostic: "IDE0161", line: 1,
                column: 11, source: "namespace KingdomZombies.Core\n{\n    internal static class Subject\n "
                        + "   {\n    }\n}\n");
    }

    /// <summary>Exercises R-12 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-12.positive")]
    public void Rule12_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-12.positive", diagnostic: "", line: 0,
                column: 0, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static int "
                        + "Read()\n    {\n        if (System.Environment.Is64BitProcess)\n       "
                        + " {\n            return 1;\n        }\n\n        return 0;\n    }\n}\n");
    }

    /// <summary>Exercises R-12 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-12.negative")]
    public void Rule12_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-12.negative", diagnostic: "IDE0011", line: 8,
                column: 9, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static int "
                        + "Read()\n    {\n        if (System.Environment.Is64BitProcess)\n       "
                        + "     return 1;\n        return 0;\n    }\n}\n");
    }

    /// <summary>Exercises R-13 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-13.positive")]
    public void Rule13_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-13.positive", diagnostic: "", line: 0,
                column: 0, source: "using System;\n\nnamespace KingdomZombies.Core;\n\n/// <summary>Isolat"
                        + "ed real consumer.</summary>\ninternal static class Subject\n{\n    int"
                        + "ernal static Type Read()\n    {\n        return typeof(int);\n    }\n}"
                        + "\n");
    }

    /// <summary>Exercises R-13 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-13.negative")]
    public void Rule13_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-13.negative", diagnostic: "IDE0065", line: 3,
                column: 5, source: "namespace KingdomZombies.Core\n{\n    using System;\n    internal stat"
                        + "ic class Subject\n    {\n        internal static Type Read()\n        "
                        + "{\n            return typeof(int);\n        }\n    }\n}\n");
    }

    /// <summary>Exercises R-15 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-15.positive")]
    public void Rule15_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-15.positive", diagnostic: "", line: 0,
                column: 0, source: "using System;\n\nnamespace KingdomZombies.Core;\n\n/// <summary>Isolat"
                        + "ed real consumer.</summary>\ninternal static class Subject\n{\n    int"
                        + "ernal static Type Read()\n    {\n        return typeof(int);\n    }\n}"
                        + "\n");
    }

    /// <summary>Exercises R-15 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-15.negative")]
    public void Rule15_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-15.negative", diagnostic: "IDE0005", line: 1,
                column: 1, source: "using System;\n\nnamespace KingdomZombies.Core;\n\n/// <summary>Isolat"
                        + "ed real consumer.</summary>\ninternal static class Subject\n{\n    int"
                        + "ernal static int Read()\n    {\n        return 1;\n    }\n}\n");
    }

    /// <summary>Exercises R-16 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-16.positive")]
    public void Rule16_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-16.positive", diagnostic: "", line: 0,
                column: 0, source: ImplicitUsingSource.Replace(oldValue: "static Uri", newValue: "static System.Uri",
                        comparisonType: StringComparison.Ordinal));
    }

    /// <summary>Exercises R-16 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-16.negative")]
    public void Rule16_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-16.negative", diagnostic: "CS0246", line: 6,
                column: 21, source: ImplicitUsingSource);
    }

    /// <summary>Changing only SDK implicit usings must make the same rejected subject compile.</summary>
    [Fact, Trait("Case", "rules.implicit-usings-control")]
    public void Configure_ImplicitUsings_ChangesNamespaceAvailability()
    {
        var fixture = new BuildFixture();
        string project = Path.Combine(path1: fixture.DirectoryPath, path2: ProjectEvaluator.Expected[0]);
        string disabledProject = File.ReadAllText(path: project).Replace(oldValue: "</Project>", newValue:
                "<PropertyGroup><EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup>"
                        + "<ItemGroup><Compile Include=\"Subject.cs\"/></ItemGroup></Project>",
                comparisonType: StringComparison.Ordinal);
        File.WriteAllText(path: project, contents: disabledProject);
        fixture.Restore();
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
        fixture.Write(source: ImplicitUsingSource);
        (int Exit, string Output) disabled = fixture.Build();
        BuildFixture.HasDiagnostic(result: disabled, id: "CS0246", line: 6, column: 21).ShouldBeTrue(
                customMessage: disabled.Output);
        File.WriteAllText(path: project, contents: disabledProject.Replace(
                oldValue: "<ImplicitUsings>disable</ImplicitUsings>",
                newValue: "<ImplicitUsings>enable</ImplicitUsings>", comparisonType: StringComparison.Ordinal));
        (int Exit, string Output) enabled = fixture.Build();
        enabled.Exit.ShouldBe(expected: 0, customMessage: enabled.Output);
        File.WriteAllText(path: project, contents: disabledProject);
        (int Exit, string Output) restored = fixture.Build();
        BuildFixture.HasDiagnostic(result: restored, id: "CS0246", line: 6, column: 21).ShouldBeTrue(
                customMessage: restored.Output);
        ProofProtocol.Record(id: "rules.implicit-usings-control", diagnostic: "IMPLICIT_USINGS_SENSITIVE",
                location: fixture.DirectoryPath +
                        "/src/godot-game/core/Subject.cs(6,21), same bytes; disable/enable/disable");
    }

    /// <summary>Exercises R-17 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-17.positive")]
    public void Rule17_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-17.positive", diagnostic: "", line: 0,
                column: 0, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static int "
                        + "Read()\n    {\n        int value = 1;\n        return value;\n    }\n}"
                        + "\n");
    }

    /// <summary>Exercises R-17 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-17.negative")]
    public void Rule17_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-17.negative", diagnostic: "IDE0008", line: 8,
                column: 9, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static int "
                        + "Read()\n    {\n        var value = 1;\n        return value;\n    }\n}"
                        + "\n");
    }

    /// <summary>Exercises R-19 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-19.positive")]
    public void Rule19_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-19.positive", diagnostic: "", line: 0,
                column: 0, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static object Read()\n"
                        + "    {\n        return _value;\n    }\n\n"
                        + "    private static readonly object _value = new();\n}\n");
    }

    /// <summary>Exercises R-19 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-19.negative")]
    public void Rule19_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-19.negative", diagnostic: "IDE0090", line: 11,
                column: 45, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static object Read()\n"
                        + "    {\n        return _value;\n    }\n\n"
                        + "    private static readonly object _value = new object();\n}\n");
    }

    /// <summary>Exercises R-20 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-20.positive")]
    public void Rule20_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-20.positive", diagnostic: "", line: 0,
                column: 0, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static int["
                        + "] Read()\n    {\n        return [1];\n    }\n}\n");
    }

    /// <summary>Exercises R-20 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-20.negative")]
    public void Rule20_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-20.negative", diagnostic: "IDE0300", line: 8,
                column: 16, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static int["
                        + "] Read()\n    {\n        return new[] { 1 };\n    }\n}\n");
    }

    /// <summary>Exercises R-21 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-21.positive")]
    public void Rule21_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-21.positive", diagnostic: "", line: 0,
                column: 0, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static bool"
                        + " Read(object? value)\n    {\n        return value is not null;\n    }"
                        + "\n}\n");
    }

    /// <summary>Exercises R-21 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-21.negative")]
    public void Rule21_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-21.negative", diagnostic: "RCS1248", line: 8,
                column: 16, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static bool"
                        + " Read(object? value)\n    {\n        return value != null;\n    }\n}\n");
    }

    /// <summary>Exercises R-22 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-22.positive")]
    public void Rule22_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-22.positive", diagnostic: "", line: 0,
                column: 0, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static int "
                        + "Read()\n    {\n        return 1;\n    }\n}\n");
    }

    /// <summary>Exercises R-22 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-22.negative")]
    public void Rule22_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-22.negative", diagnostic: "IDE0022", line: 6,
                column: 32, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static int "
                        + "Read() => 1;\n}\n");
    }

    /// <summary>Exercises R-27 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-27.positive")]
    public void Rule27_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-27.positive", diagnostic: "", line: 0,
                column: 0, source: "namespace KingdomZombies.Core;\n\ninternal sealed class Subject\n{\n  "
                        + "  internal int Read()\n    {\n        return _value;\n    }\n\n    pri"
                        + "vate readonly int _value = 1;\n}\n");
    }

    /// <summary>Exercises R-27 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-27.negative")]
    public void Rule27_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-27.negative", diagnostic: "IDE1006", line: 10,
                column: 26, source: "namespace KingdomZombies.Core;\n\ninternal sealed class Subject\n{\n  "
                        + "  internal int Read()\n    {\n        return value;\n    }\n\n    priv"
                        + "ate readonly int value = 1;\n}\n");
    }

    /// <summary>Exercises R-29 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-29.positive")]
    public void Rule29_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-29.positive", diagnostic: "", line: 0,
                column: 0, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static int "
                        + "Read()\n    {\n        const int Value = 1;\n        return Value;\n  "
                        + "  }\n}\n");
    }

    /// <summary>Exercises R-29 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-29.negative")]
    public void Rule29_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-29.negative", diagnostic: "IDE1006", line: 8,
                column: 19, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static int "
                        + "Read()\n    {\n        const int value = 1;\n        return value;\n  "
                        + "  }\n}\n");
    }

    /// <summary>Exercises R-31 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-31.positive")]
    public void Rule31_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-31.positive", diagnostic: "", line: 0,
                column: 0, source: "namespace KingdomZombies.Core;\n\n/// <summary>Unsealed control.</summ"
                        + "ary>\npublic sealed class Subject\n{\n}\n");
    }

    /// <summary>Exercises R-31 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-31.negative")]
    public void Rule31_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-31.negative", diagnostic: "MA0053", line: 4,
                column: 14, source: "namespace KingdomZombies.Core;\n\n/// <summary>Unsealed control.</summ"
                        + "ary>\npublic class Subject\n{\n}\n");
    }

    /// <summary>Exercises R-32 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-32.positive")]
    public void Rule32_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-32.positive", diagnostic: "", line: 0,
                column: 0, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static int "
                        + "Read()\n    {\n        return 1;\n    }\n}\n");
    }

    /// <summary>Exercises R-32 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-32.negative")]
    public void Rule32_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-32.negative", diagnostic: "MA0048", line: 12,
                column: 23, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static int "
                        + "Read()\n    {\n        return 1;\n    }\n}\n\ninternal sealed class Ot"
                        + "her\n{\n}\n");
    }

    /// <summary>Exercises R-35 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-35.positive")]
    public void Rule35_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-35.positive", diagnostic: "", line: 0,
                column: 0, source: "namespace KingdomZombies.Core;\n\n/// <summary>Documented control.</su"
                        + "mmary>\npublic sealed class Subject\n{\n}\n");
    }

    /// <summary>Exercises R-35 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-35.negative")]
    public void Rule35_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-35.negative", diagnostic: "CS1591", line: 3,
                column: 21, source: "namespace KingdomZombies.Core;\n\npublic sealed class Subject\n{\n}\n");
    }

    /// <summary>Exercises R-39 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-39.positive")]
    public void Rule39_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-39.positive", diagnostic: "", line: 0,
                column: 0, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static Syst"
                        + "em.DateTime Read(System.DateTime now)\n    {\n        return now;\n   "
                        + " }\n}\n");
    }

    /// <summary>Exercises R-39 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-39.negative")]
    public void Rule39_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-39.negative", diagnostic: "RS0030", line: 8,
                column: 16, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static Syst"
                        + "em.DateTime Read()\n    {\n        return System.DateTime.UtcNow;\n   "
                        + " }\n}\n");
    }

    /// <summary>Exercises R-40 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-40.positive")]
    public void Rule40_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-40.positive", diagnostic: "", line: 0,
                column: 0, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static void"
                        + " Read(System.Action<int> report)\n    {\n        report(1);\n    }\n}"
                        + "\n");
    }

    /// <summary>Exercises R-40 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-40.negative")]
    public void Rule40_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-40.negative", diagnostic: "RS0030", line: 8,
                column: 9, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static void"
                        + " Read()\n    {\n        System.Console.WriteLine(value: 1);\n    }\n}"
                        + "\n");
    }

    /// <summary>Exercises R-41 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-41.positive")]
    public void Rule41_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-41.positive", diagnostic: "", line: 0,
                column: 0, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static asyn"
                        + "c System.Threading.Tasks.Task Read()\n    {\n        await System.Thre"
                        + "ading.Tasks.Task.Delay(millisecondsDelay: 1).ConfigureAwait(continueOn"
                        + "CapturedContext: false);\n    }\n}\n");
    }

    /// <summary>Exercises R-41 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-41.negative")]
    public void Rule41_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-41.negative", diagnostic: "RS0030", line: 8,
                column: 9, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static void"
                        + " Read()\n    {\n        System.Threading.Thread.Sleep(millisecondsTime"
                        + "out: 1);\n    }\n}\n");
    }

    /// <summary>Exercises R-42 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-42.positive")]
    public void Rule42_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-42.positive", diagnostic: "", line: 0,
                column: 0, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static asyn"
                        + "c System.Threading.Tasks.Task<int> Read(System.Threading.Tasks.Task<in"
                        + "t> value)\n    {\n        return await value.ConfigureAwait(continueOn"
                        + "CapturedContext: false);\n    }\n}\n");
    }

    /// <summary>Exercises R-42 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-42.negative")]
    public void Rule42_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-42.negative", diagnostic: "RS0030", line: 8,
                column: 16, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static int "
                        + "Read(System.Threading.Tasks.Task<int> value)\n    {\n        return va"
                        + "lue.Result;\n    }\n}\n");
    }

    /// <summary>Exercises R-45 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-45.positive")]
    public void Rule45_CompliantConsumer_Accepted()
    {
        VerifyConsumer(id: "R-45.positive", diagnostic: "", line: 0,
                column: 0, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static asyn"
                        + "c System.Threading.Tasks.Task Read(System.Threading.Tasks.Task value)"
                        + "\n    {\n        await value.ConfigureAwait(continueOnCapturedContext:"
                        + " false);\n    }\n}\n");
    }

    /// <summary>Exercises R-45 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-45.negative")]
    public void Rule45_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-45.negative", diagnostic: "MA0004", line: 8,
                column: 9, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static asyn"
                        + "c System.Threading.Tasks.Task Read(System.Threading.Tasks.Task value)"
                        + "\n    {\n        await value;\n    }\n}\n");
    }

    /// <summary>Invalid UTF-8 must name the intended rule and precise raw source byte.</summary>
    [Fact, Trait("Case", "R-02.negative")]
    public void Check_InvalidUtf8_ReportsRuleAndLocation()
    {
        IReadOnlyList<string> diagnostics = SourceFileFormat.Check(path: "Subject.txt", content: [0xFF, 0x0A]);
        diagnostics.Count.ShouldBe(expected: 1);
        diagnostics[0].ShouldBe(expected: "R-02 FORMAT_UTF8 Subject.txt(byte 1)");
        ProofProtocol.Record(id: "R-02.negative", diagnostic: "FORMAT_UTF8", location: "Subject.txt(byte 1)");
    }

    /// <summary>Strict UTF-8 preserves combining, supplementary, Cyrillic and CJK representations.</summary>
    [Fact, Trait("Case", "R-02.positive")]
    public void Check_ValidUnicode_AcceptsUtf8()
    {
        SourceFileFormat.Check(path: "Subject.txt", content: Encoding.UTF8.GetBytes(s: "é中e\u0301😀Ж\n"))
                .ShouldBeEmpty();
        ProofProtocol.Record(id: "R-02.positive", diagnostic: "FORMAT_UTF8_ACCEPTED", location: "Subject.txt");
    }

    /// <summary>A UTF-8 BOM remains a violation even when all following bytes are valid.</summary>
    [Fact, Trait("Case", "rules.source-format.bom")]
    public void Check_Utf8Bom_ReportsExactByte()
    {
        RequireFormat(path: "Subject.txt", content: [0xEF, 0xBB, 0xBF, 0x78, 0x0A],
                expected: "R-02 FORMAT_BOM Subject.txt(byte 1)");
        ProofProtocol.Record(id: "rules.source-format.bom", diagnostic: "FORMAT_BOM", location: "Subject.txt(byte 1)");
    }

    /// <summary>LF-only text passes without any final-newline exemption.</summary>
    [Fact, Trait("Case", "R-03.positive")]
    public void Check_LfText_AcceptsLineEndings()
    {
        SourceFileFormat.Check(path: "Subject.txt", content: [0x78, 0x0A]).ShouldBeEmpty();
        ProofProtocol.Record(id: "R-03.positive", diagnostic: "FORMAT_LF_ACCEPTED", location: "Subject.txt");
    }

    /// <summary>The first CR is diagnosed without normalizing the input bytes.</summary>
    [Fact, Trait("Case", "R-03.negative")]
    public void Check_CrlfText_RejectsCarriageReturn()
    {
        RequireFormat(path: "Subject.txt", content: [0x78, 0x0D, 0x0A], expected: "R-03 FORMAT_LF Subject.txt(byte 2)");
        ProofProtocol.Record(id: "R-03.negative", diagnostic: "FORMAT_LF", location: "Subject.txt(byte 2)");
    }

    /// <summary>Only registered observed lock paths may omit their final LF; every other format rule applies.</summary>
    [Fact, Trait("Case", "R-04.positive")]
    public void Check_RegisteredLocks_AcceptsOnlyFinalNewlinePermission()
    {
        IReadOnlySet<string> paths = SourceFileFormat.ReadFinalNewlineExemptions();
        paths.Count.ShouldBe(expected: 5);
        foreach (string path in paths)
        {
            SourceFileFormat.Check(path: path, content: [0x7B, 0x7D], finalNewlineExemptions: paths).ShouldBeEmpty();
            IReadOnlyList<string> crlf = SourceFileFormat.Check(path: path, content: [0x7B, 0x7D, 0x0D, 0x0A],
                    finalNewlineExemptions: paths);
            crlf.Count.ShouldBe(expected: 1);
            crlf[0].ShouldBe(expected: "R-03 FORMAT_LF " + path + "(byte 3)");
        }

        ProofProtocol.Record(id: "R-04.positive", diagnostic: "FORMAT_LOCK_SCOPE_EXACT", location: "five exact locks");
    }

    /// <summary>Missing final LF immediately outside every exact lock scope still fails.</summary>
    [Fact, Trait("Case", "R-04.negative")]
    public void Check_AdjacentLockPaths_RejectsMissingNewline()
    {
        IReadOnlySet<string> paths = SourceFileFormat.ReadFinalNewlineExemptions();
        foreach (string path in paths)
        {
            string adjacent = path + ".adjacent";
            IReadOnlyList<string> diagnostics = SourceFileFormat.Check(path: adjacent, content: [0x7B, 0x7D],
                    finalNewlineExemptions: paths);
            diagnostics.Count.ShouldBe(expected: 1);
            diagnostics[0].ShouldBe(expected: "R-04 FORMAT_FINAL_NEWLINE " + adjacent + "(byte 3)");
        }

        RequireFormat(path: "elsewhere/packages.lock.json", content: [0x7B, 0x7D],
                expected: "R-04 FORMAT_FINAL_NEWLINE elsewhere/packages.lock.json(byte 3)");
        ProofProtocol.Record(id: "R-04.negative", diagnostic: "FORMAT_FINAL_NEWLINE", location: "adjacent lock paths");
    }

    /// <summary>Leading spaces are preserved and valid when no line ends with whitespace.</summary>
    [Fact, Trait("Case", "R-05.positive")]
    public void Check_LeadingSpaces_AcceptsCleanLineEnds()
    {
        SourceFileFormat.Check(path: "Subject.txt", content: Encoding.UTF8.GetBytes(s: "  x\n")).ShouldBeEmpty();
        ProofProtocol.Record(id: "R-05.positive", diagnostic: "FORMAT_WHITESPACE_ACCEPTED", location: "Subject.txt");
    }

    /// <summary>Trailing spaces are rejected at the exact raw byte without trimming the source.</summary>
    [Fact, Trait("Case", "R-05.negative")]
    public void Check_TrailingSpace_ReportsExactByte()
    {
        RequireFormat(path: "Subject.txt", content: Encoding.UTF8.GetBytes(s: "x \n"),
                expected: "R-05 FORMAT_TRAILING_WHITESPACE Subject.txt(byte 2)");
        ProofProtocol.Record(id: "R-05.negative", diagnostic: "FORMAT_TRAILING_WHITESPACE",
                location: "Subject.txt(byte 2)");
    }

    /// <summary>Four ordinary spaces do not create a tab permission or a format violation.</summary>
    [Fact, Trait("Case", "R-06.positive")]
    public void Check_SpacesOnly_AcceptsText()
    {
        SourceFileFormat.Check(path: "Subject.txt", content: Encoding.UTF8.GetBytes(s: "x    y\n")).ShouldBeEmpty();
        ProofProtocol.Record(id: "R-06.positive", diagnostic: "FORMAT_SPACES_ACCEPTED", location: "Subject.txt");
    }

    /// <summary>A tab inside a line fails even though it is not trailing whitespace.</summary>
    [Fact, Trait("Case", "R-06.negative")]
    public void Check_InteriorTab_ReportsExactByte()
    {
        RequireFormat(path: "Subject.txt", content: Encoding.UTF8.GetBytes(s: "x\ty\n"),
                expected: "R-06 FORMAT_TAB Subject.txt(byte 2)");
        ProofProtocol.Record(id: "R-06.negative", diagnostic: "FORMAT_TAB", location: "Subject.txt(byte 2)");
    }

    /// <summary>Git's NUL probe uses raw bytes and an exclusive 8000-byte boundary.</summary>
    [Fact, Trait("Case", "rules.source-format.binary-boundary")]
    public void Classify_NulProbeBoundary_PreservesExactScope()
    {
        int length = SourceFileFormat.TextProbeLength + 1;
        byte[] bytes = [.. Enumerable.Repeat(element: (byte)'x', count: length)];
        bytes[SourceFileFormat.TextProbeLength - 1] = 0;
        SourceFileFormat.IsText(content: bytes).ShouldBeFalse();
        bytes[SourceFileFormat.TextProbeLength - 1] = (byte)'x';
        bytes[SourceFileFormat.TextProbeLength] = 0;
        SourceFileFormat.IsText(content: bytes).ShouldBeTrue();
        SourceFileFormat.IsText(content: []).ShouldBeTrue();
        SourceFileFormat.IsChecked(path: "assets/new.unknown").ShouldBeTrue();
        SourceFileFormat.IsChecked(path: "assets/font.woff2").ShouldBeFalse();
        SourceFileFormat.IsChecked(path: ".codex/vendor/bad.cs").ShouldBeFalse();
        SourceFileFormat.IsChecked(path: "src/.codex/Subject.cs").ShouldBeTrue();
        ProofProtocol.Record(id: "rules.source-format.binary-boundary", diagnostic: "FORMAT_BINARY_SCOPE_EXACT",
                location: "NUL indices 7999/8000; empty input; exact installed-root and binary-extension scopes");
    }

    /// <summary>Missing tracked files and invalid text in unfamiliar extensions remain visible failures.</summary>
    [Fact, Trait("Case", "rules.source-format.tracked-missing")]
    public void Check_MissingOrUnknownTrackedText_ReportsFailure()
    {
        IReadOnlyList<string> missing = SourceFileFormat.CheckTracked(path: "Subject.unknown", content: null);
        missing.Count.ShouldBe(expected: 1);
        missing[0].ShouldBe(expected: "R-02 FORMAT_TRACKED_FILE Subject.unknown");
        IReadOnlyList<string> invalid = SourceFileFormat.CheckTracked(path: "Subject.unknown", content: [0xFF, 0x0A]);
        invalid.Count.ShouldBe(expected: 1);
        invalid[0].ShouldBe(expected: "R-02 FORMAT_UTF8 Subject.unknown(byte 1)");
        ProofProtocol.Record(id: "rules.source-format.tracked-missing", diagnostic: "FORMAT_TRACKED_FILE_REJECTED",
                location: "Subject.unknown");
    }

    /// <summary>All tracked project text is checked without modifying any approved input.</summary>
    [Fact, Trait("Case", "rules.source-format")]
    public void Check_TrackedProjectText_AcceptsCurrentCheckout()
    {
        IReadOnlyList<string> errors = SourceFileFormat.CheckRepository();
        errors.ShouldBeEmpty(customMessage: string.Join(separator: "\n", values: errors));
        ProofProtocol.Record(id: "rules.source-format", diagnostic: "FORMAT_TRACKED_TEXT_EXACT",
                location: "actual Git tracked project text outside installed tooling");
    }

    /// <summary>Actual locked restores preserve the canonical LF bytes and exact final-newline scopes.</summary>
    [Fact, Trait("Case", "rules.source-format.lock-restore")]
    public void Restore_CanonicalLfLocks_PreservesExactBytes()
    {
        IReadOnlySet<string> exemptions = SourceFileFormat.ReadFinalNewlineExemptions();
        foreach (string project in ConfigurationTests.Projects)
        {
            string relative = Path.GetDirectoryName(path: project)?.Replace(oldChar: '\\', newChar: '/')
                    + "/packages.lock.json";
            string file = Path.Combine(path1: ProofProtocol.Root, path2: relative);
            byte[] before = File.ReadAllBytes(path: file);
            SourceFileFormat.Check(path: relative, content: before, finalNewlineExemptions: exemptions).ShouldBeEmpty();
            (int Exit, string Output) restore = ConfigurationTests.Dotnet("restore", project, "--locked-mode",
                    "--disable-parallel", "-p:NuGetAudit=false");
            restore.Exit.ShouldBe(expected: 0, customMessage: restore.Output);
            File.ReadAllBytes(path: file).ShouldBe(expected: before);
        }

        ProofProtocol.Record(id: "rules.source-format.lock-restore", diagnostic: "FORMAT_LOCKED_RESTORE_UNCHANGED",
                location: "all five actual canonical projects/locks");
    }

    /// <summary>The real formatter accepts the four-space handwritten control under actual inherited
    /// settings.</summary>
    [Fact, Trait("Case", "R-07.positive")]
    public void Format_FourSpaceIndent_AcceptsSubject()
    {
        var fixture = new BuildFixture();
        fixture.Restore();
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
        (int Exit, string Output) formatted = fixture.Format();
        formatted.Exit.ShouldBe(expected: 0, customMessage: formatted.Output);
        Regex.IsMatch(input: formatted.Output, pattern: @"Formatted 0 of [1-9][0-9]* files\.",
                options: RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.NonBacktracking,
                matchTimeout: TimeSpan.FromSeconds(value: 1)).ShouldBeTrue(customMessage: formatted.Output);
        ProofProtocol.Record(id: "R-07.positive", diagnostic: "FORMAT_INDENT_FOUR",
                location: fixture.DirectoryPath + "/src/godot-game/core/Subject.cs");
    }

    /// <summary>The real formatter rejects both immediate indentation neighbors at their precise subject
    /// positions.</summary>
    [Fact, Trait("Case", "R-07.negative")]
    public void Format_ThreeAndFiveSpaceIndent_RejectsExactSubject()
    {
        var fixture = new BuildFixture();
        fixture.Restore();
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
        foreach (int indent in new[] { 3, 5 })
        {
            string prefix = new string(c: ' ', count: indent) + "internal";
            fixture.Write(source: BuildFixture.CleanSource.Replace(oldValue: "    internal", newValue: prefix,
                    comparisonType: StringComparison.Ordinal));
            (int Exit, string Output) changed = fixture.Format();
            int column = indent == 3 ? 4 : 5;
            BuildFixture.HasDiagnostic(result: changed, id: "WHITESPACE", line: 6, column: column).ShouldBeTrue(
                    customMessage: changed.Output);
        }

        ProofProtocol.Record(id: "R-07.negative", diagnostic: "FORMAT_INDENT_NEIGHBORS",
                location: fixture.DirectoryPath + "/src/godot-game/core/Subject.cs(6,4)/(6,5)");
    }

    /// <summary>Tab rendering retains four columns while tab bytes remain independently prohibited.</summary>
    [Fact, Trait("Case", "R-08.positive")]
    public void Configure_FourColumnTabWidth_AcceptsPolicy()
    {
        string source = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: ".editorconfig"));
        SourceFileFormat.RequireEditorSetting(source: source, name: "tab_width", expected: "4");
        ProofProtocol.Record(id: "R-08.positive", diagnostic: "FORMAT_TAB_WIDTH_FOUR",
                location: ".editorconfig tab_width");
    }

    /// <summary>Permission drift to three/five columns or missing width is rejected as configuration rather
    /// than invented syntax.</summary>
    [Fact, Trait("Case", "R-08.negative")]
    public void Configure_TabWidthDrift_RejectsPolicy()
    {
        string source = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: ".editorconfig"));
        foreach (string width in new[] { "3", "5" })
        {
            string changed = source.Replace(oldValue: "tab_width = 4", newValue: "tab_width = " + width,
                    comparisonType: StringComparison.Ordinal);
            RequireTabWidthFailure(source: changed);
        }

        string missing = source.Replace(oldValue: "tab_width = 4", newValue: "",
                comparisonType: StringComparison.Ordinal);
        RequireTabWidthFailure(source: missing);
        ProofProtocol.Record(id: "R-08.negative", diagnostic: "FORMAT_TAB_WIDTH_DRIFT",
                location: ".editorconfig tab_width");
    }

    /// <summary>An actual public Task method without Async passes the retained permission.</summary>
    [Fact, Trait("Case", "R-30.positive")]
    public void Compile_AwaitableWithoutSuffix_Accepted()
    {
        VerifyConsumer(id: "R-30.positive", diagnostic: "", line: 0, column: 0, source: AsyncPermissionSource);
    }

    /// <summary>Changing only the isolated naming requirement emits the exact intended MA0137.</summary>
    [Fact, Trait("Case", "R-30.negative")]
    public void Configure_AsyncSuffixRequirement_RejectsPermissionDrift()
    {
        var fixture = new BuildFixture();
        fixture.Restore();
        fixture.Write(source: AsyncPermissionSource);
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
        string config = Path.Combine(path1: fixture.DirectoryPath, path2: ".editorconfig");
        string original = File.ReadAllText(path: config);
        original.ShouldContain(expected: "dotnet_diagnostic.MA0137.severity = none");
        File.WriteAllText(path: config, contents: original.Replace(
                oldValue: "dotnet_diagnostic.MA0137.severity = none",
                newValue: "dotnet_diagnostic.MA0137.severity = error", comparisonType: StringComparison.Ordinal));
        (int Exit, string Output) changed = fixture.Build();
        BuildFixture.HasDiagnostic(result: changed, id: "MA0137", line: 7, column: 47).ShouldBeTrue(
                customMessage: changed.Output);
        ProofProtocol.Record(id: "R-30.negative", diagnostic: "ASYNC_PERMISSION_DRIFT",
                location: fixture.DirectoryPath + "/src/godot-game/core/Subject.cs(7,47)");
    }

    /// <summary>The actual copied test project accepts the compliant custom-rule source.</summary>
    [Fact, Trait("Case", "R-46.positive")]
    public void Compile_CompliantTestConsumer_Accepted()
    {
        VerifyConsumer(id: "R-46.positive", diagnostic: "", line: 0, column: 0, source: TestCallSource, tests: true);
    }

    /// <summary>The test project cannot bypass the handwritten positional-argument rule.</summary>
    [Fact, Trait("Case", "R-46.negative")]
    public void Compile_PositionalTestArgument_Rejected()
    {
        VerifyConsumer(id: "R-46.negative", diagnostic: "SCR0001", line: 8, column: 21,
                source: TestCallSource.Replace(oldValue: "Echo(value: 1)", newValue: "Echo(1)",
                        comparisonType: StringComparison.Ordinal), tests: true);
    }

    /// <summary>The actual copied test project permits its declared Shouldly assertion API.</summary>
    [Fact, Trait("Case", "R-47.positive")]
    public void Compile_ShouldlyTestAssertion_Accepted()
    {
        VerifyConsumer(id: "R-47.positive", diagnostic: "", line: 0, column: 0,
                source: TestAssertionSource, tests: true);
    }

    /// <summary>The same actual test-project wiring rejects the banned Xunit.Assert type.</summary>
    [Fact, Trait("Case", "R-47.negative")]
    public void Compile_XunitAssertInTests_Rejected()
    {
        VerifyConsumer(id: "R-47.negative", diagnostic: "RS0030", line: 8, column: 9,
                source: BannedTestAssertionSource, tests: true);
    }

    /// <summary>The pinned actual compiler accepts cached LoggerMessage delegates under the logging rule.</summary>
    [Fact, Trait("Case", "R-44.positive")]
    public void Compile_CachedLoggerMessage_Accepted()
    {
        BuildFixture fixture = LoggingConsumer();
        fixture.Write(source: LoggerMessageSource);
        (int Exit, string Output) result = fixture.Build();
        result.Exit.ShouldBe(expected: 0, customMessage: result.Output);
        ProofProtocol.Record(id: "R-44.positive", diagnostic: "LOGGERMESSAGE_COMPILER_ACCEPTED",
                location: fixture.DirectoryPath + "/src/godot-game/core/Subject.cs");
    }

    /// <summary>The actual ILogger extension call is rejected at its exact handwritten source location.</summary>
    [Fact, Trait("Case", "R-44.negative")]
    public void Compile_DirectLoggerExtension_Rejected()
    {
        BuildFixture fixture = LoggingConsumer();
        fixture.Write(source: DirectLoggerSource);
        (int Exit, string Output) result = fixture.Build();
        BuildFixture.HasDiagnostic(result: result, id: "CA1848", line: 10, column: 9).ShouldBeTrue(
                customMessage: result.Output);
        BuildFixture.HasDiagnostic(result: result, id: "CA1873", line: 10, column: 83).ShouldBeTrue(
                customMessage: result.Output);
        ProofProtocol.Record(id: "R-44.negative", diagnostic: "CA1848",
                location: fixture.DirectoryPath + "/src/godot-game/core/Subject.cs(10,9)");
    }

    /// <summary>The actual async-stream consumer permits an omitted suffix under the captured policy.</summary>
    [Fact, Trait("Case", "rules.async-enumerable-positive")]
    public void Compile_AsyncEnumerableWithoutSuffix_AcceptsDeclaredPermission()
    {
        VerifyConsumer(id: "rules.async-enumerable-positive", diagnostic: "", line: 0, column: 0,
                source: AsyncEnumerablePermissionSource);
    }

    /// <summary>Changing only MA0156 severity rejects the intended async-stream method at its exact location.</summary>
    [Fact, Trait("Case", "rules.async-enumerable-negative")]
    public void Configure_AsyncEnumerableSuffix_RejectsPermissionDrift()
    {
        var fixture = new BuildFixture();
        fixture.Restore();
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
        fixture.Write(source: AsyncEnumerablePermissionSource);
        (int Exit, string Output) allowed = fixture.Build();
        allowed.Exit.ShouldBe(expected: 0, customMessage: allowed.Output);
        string file = Path.Combine(path1: fixture.DirectoryPath, path2: ".editorconfig");
        File.WriteAllText(path: file, contents: File.ReadAllText(path: file).Replace(
                oldValue: "dotnet_diagnostic.MA0156.severity = none",
                newValue: "dotnet_diagnostic.MA0156.severity = error", comparisonType: StringComparison.Ordinal));
        (int Exit, string Output) changed = fixture.Build();
        BuildFixture.HasDiagnostic(result: changed, id: "MA0156", line: 8, column: 43).ShouldBeTrue(
                customMessage: changed.Output);
        ProofProtocol.Record(id: "rules.async-enumerable-negative", diagnostic: "MA0156",
                location: fixture.DirectoryPath + "/src/godot-game/core/Subject.cs(8,43)");
    }

    private const string ImplicitUsingSource = "namespace KingdomZombies.Core;\n\n"
            + "/// <summary>Isolated real consumer.</summary>\ninternal static class Subject\n{\n"
            + "    internal static Uri Read()\n    {\n"
            + "        return new(uriString: \"https://example.invalid/\");\n    }\n}\n";
    private const string AsyncEnumerablePermissionSource = "using System.Collections.Generic;\n\n"
            + "namespace KingdomZombies.Core;\n\n/// <summary>Async stream permission control.</summary>\n"
            + "internal static class Subject\n{\n"
            + "    internal static IAsyncEnumerable<int> Read(IAsyncEnumerable<int> source)\n"
            + "    {\n        return source;\n    }\n}\n";
    private const string LoggerMessageSource = "using System;\nusing Microsoft.Extensions.Logging;\n\nname"
            + "space KingdomZombies.Core;\n\n/// <summary>Actual logging "
            + "compiler control.</summary>\ninternal static class Subject"
            + "\n{\n    internal static void Read(ILogger logger, int val"
            + "ue)\n    {\n        _write(logger, value, null);\n    }\n"
            + "\n    private static readonly Action<ILogger, int, Excepti"
            + "on?> _write = LoggerMessage.Define<int>(\n            logL"
            + "evel: LogLevel.Information, eventId: new EventId(id: 1), f"
            + "ormatString: \"Value {Value}\");\n}\n";
    private const string DirectLoggerSource = "using Microsoft.Extensions.Logging;\n\nnamespace KingdomZo"
            + "mbies.Core;\n\n/// <summary>Actual logging compiler negati"
            + "ve.</summary>\ninternal static class Subject\n{\n    inter"
            + "nal static void Read(ILogger logger)\n    {\n        Logge"
            + "rExtensions.LogInformation(logger: logger, message: \"Valu"
            + "e {Value}\", args: [CreateValue()]);\n    }\n\n    private"
            + " static string CreateValue()\n    {\n        return new st"
            + "ring(c: 'x', count: 10);\n    }\n}\n";

    private const string AsyncPermissionSource = "namespace KingdomZombies.Core;\n\n/// <summary>Awaitable p"
            + "ermission control.</summary>\npublic static class Subject"
            + "\n{\n    /// <summary>Permitted suffix omission.</summary>"
            + "\n    public static System.Threading.Tasks.Task Read()\n  "
            + "  {\n        return System.Threading.Tasks.Task.CompletedT"
            + "ask;\n    }\n}\n";
    private const string TestCallSource = "namespace KingdomZombies.Phase1.Tests;\n\n/// <summary>Rea"
            + "l test-project rule control.</summary>\ninternal static cl"
            + "ass Subject\n{\n    internal static int Read()\n    {\n   "
            + "     return Echo(value: 1);\n    }\n\n    private static i"
            + "nt Echo(int value)\n    {\n        return value;\n    }\n}"
            + "\n";
    private const string TestAssertionSource = "using Shouldly;\n\nnamespace KingdomZombies.Phase1.Tests;"
            + "\n\n/// <summary>Real test-project assertion control.</sum"
            + "mary>\ninternal static class Subject\n{\n    internal stat"
            + "ic void Read()\n    {\n        1.ShouldBe(expected: 1);\n "
            + "   }\n}\n";
    private const string BannedTestAssertionSource = "namespace KingdomZombies.Phase1.Tests;\n\n/// <summary>Rea"
            + "l test-project assertion control.</summary>\ninternal stat"
            + "ic class Subject\n{\n    internal static void Read()\n    "
            + "{\n        Xunit.Assert.True(condition: true);\n    }\n}\n";

    private static BuildFixture LoggingConsumer()
    {
        var fixture = new BuildFixture();
        fixture.AddLoggingReference();
        fixture.Restore();
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
        return fixture;
    }

    private static void RequireTabWidthFailure(string source)
    {
        InvalidDataException rejected = Should.Throw<InvalidDataException>(actual: () => SourceFileFormat
                .RequireEditorSetting(source: source, name: "tab_width", expected: "4"));
        rejected.Message.ShouldContain(expected: "FORMAT_SETTING .editorconfig tab_width");
    }

    private static void RequireFormat(string path, byte[] content, string expected)
    {
        IReadOnlyList<string> diagnostics = SourceFileFormat.Check(path: path, content: content);
        diagnostics.Count.ShouldBe(expected: 1);
        diagnostics[0].ShouldBe(expected: expected);
    }

    private static void VerifyConsumer(string id, string diagnostic, int line, int column, string source,
            bool tests = false)
    {
        var fixture = new BuildFixture(tests: tests);
        fixture.Restore();
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
        fixture.Write(source: source);
        (int Exit, string Output) result = fixture.Build();
        if (line == 0)
        {
            result.Exit.ShouldBe(expected: 0, customMessage: result.Output);
        }
        else
        {
            BuildFixture.HasDiagnostic(result: result, id: diagnostic, line: line, column: column).ShouldBeTrue(
                    customMessage: result.Output);
        }

        string subject = tests ? "/src/godot-game/tests/verification/Subject.cs(" : "/src/godot-game/core/Subject.cs(";
        string location = fixture.DirectoryPath + subject + line.ToString(
                provider: System.Globalization.CultureInfo.InvariantCulture) + "," + column.ToString(
                        provider: System.Globalization.CultureInfo.InvariantCulture) + ")";
        ProofProtocol.Record(id: id, diagnostic: line == 0 ? "REAL_BUILD_CLEAN" : diagnostic, location: location);
    }
}
