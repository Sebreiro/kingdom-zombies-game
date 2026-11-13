using System;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Requires actual clean and intended-negative builds under the repository policy.</summary>
public sealed class EnforcementTests
{
    /// <summary>A real copied Core consumer rejects precisely the sixty-first body line.</summary>
    [Fact, Trait("Case", "rules.build-wiring")]
    public void RealConsumerRejectsSixtyFirstBodyLine()
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
                .DirectoryPath + "/src/core/Subject.cs(6,25)");
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
                column: 0, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static int "
                        + "Read()\n    {\n        return 1;\n    }\n}\n");
    }

    /// <summary>Exercises R-16 through its actual isolated compiler consumer.</summary>
    [Fact, Trait("Case", "R-16.negative")]
    public void Rule16_ViolatingConsumer_Rejected()
    {
        VerifyConsumer(id: "R-16.negative", diagnostic: "CS0246", line: 6,
                column: 21, source: "namespace KingdomZombies.Core;\n\n/// <summary>Isolated real consumer."
                        + "</summary>\ninternal static class Subject\n{\n    internal static Miss"
                        + "ing Read()\n    {\n        return new();\n    }\n}\n");
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

    private static void VerifyConsumer(string id, string diagnostic, int line, int column, string source)
    {
        var fixture = new BuildFixture();
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

        string location = fixture.DirectoryPath + "/src/core/Subject.cs(" + line.ToString(
                provider: System.Globalization.CultureInfo.InvariantCulture) + "," + column.ToString(
                        provider: System.Globalization.CultureInfo.InvariantCulture) + ")";
        ProofProtocol.Record(id: id, diagnostic: line == 0 ? "REAL_BUILD_CLEAN" : diagnostic, location: location);
    }
}
