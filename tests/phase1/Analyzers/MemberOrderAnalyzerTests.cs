using System.Threading.Tasks;
using KingdomZombies.Phase1.Tests.Verification;
using Xunit;

namespace KingdomZombies.Phase1.Tests.Analyzers;

/// <summary>
/// In-memory proofs of SCR0008: members run by access first (public, internal, protected internal, protected,
/// private protected, private), then by kind (constants, fields, constructors, properties, indexers, events,
/// methods, operators), with nested types last. The first member that breaks the order is reported, once per
/// type declaration part.
/// </summary>
public sealed class MemberOrderAnalyzerTests
{
    /// <summary>The full D-27 order passes.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_FullOrder_Passes")]
    public Task AnalyzeFullOrderPasses()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_FullOrder_Passes", diagnostic: "ANALYZER_ACCEPTED"
                , source: """
                using System;

                class C
                {
                    public const int Max = 1;
                    public int Field;
                    public C() { }
                    public int Property { get; set; }
                    public int this[int index] => index;
                    public event Action Changed = delegate { };
                    public void Method() { }
                    public static C operator +(C left, C right) => left;
                    public static implicit operator int(C value) => 0;
                    internal const int InternalMax = 1;
                    internal int _internalField;
                    internal C(int value) { }
                    internal int InternalProperty { get; set; }
                    internal void InternalMethod() { }
                    protected int _protectedField;
                    protected void ProtectedMethod() { }
                    private const int PrivateMax = 1;
                    private int _privateField;
                    private C(string value) { }
                    private int PrivateProperty { get; set; }
                    private event Action PrivateChanged = delegate { };
                    private void PrivateMethod() { }

                private void ReadFields15() {
                    Field = 0;
                    _ = Field;
                    _ = Changed;
                    _internalField = 0;
                    _ = _internalField;
                    _protectedField = 0;
                    _ = _protectedField;
                    _privateField = 0;
                    _ = _privateField;
                    _ = PrivateChanged;
                }
                    private sealed class Nested { }
                    public enum Kind { None }
                    public delegate void Handler();
                }
                """);
    }

    /// <summary>A private field declared before a public method is reported on the method.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-33.negative")]
    public Task AnalyzePrivateFieldBeforePublicMethodReports()
    {
        return VerifyAsync(caseId: "R-33.negative", diagnostic: "SCR0008", source: """
                class C
                {
                    private int _value;

                    public void {|SCR0008:Method|}()
                    {
                    }

                private void ReadFields0() {
                    _value = 0;
                    _ = _value;
                }
                }
                """);
    }

    /// <summary>A public property after a public method is reported on the property.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_PublicPropertyAfterPublicMethod_Reports")]
    public Task AnalyzePublicPropertyAfterPublicMethodReports()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_PublicPropertyAfterPublicMethod_Reports",
                diagnostic: "SCR0008", source: """
                class C
                {
                    public void Method()
                    {
                    }

                    public int {|SCR0008:Property|} { get; set; }
                }
                """);
    }

    /// <summary>A constructor after a property of the same access is reported on the constructor.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.ConstructorAfterProperty_SameAccess_Reports")]
    public Task ConstructorAfterPropertySameAccessReports()
    {
        return VerifyAsync(caseId: "rules.memberorder.ConstructorAfterProperty_SameAccess_Reports", diagnostic:
                "SCR0008", source: """
                class C
                {
                    public int Property { get; set; }

                    public {|SCR0008:C|}()
                    {
                    }
                }
                """);
    }

    /// <summary>A nested type before a member is reported on the member, whatever the access of either.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_NestedTypeBeforeMember_Reports")]
    public Task AnalyzeNestedTypeBeforeMemberReports()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_NestedTypeBeforeMember_Reports", diagnostic:
                "SCR0008", source: """
                class C
                {

                private void ReadFields0() {
                    _value = 0;
                    _ = _value;
                }
                    public sealed class Nested
                    {
                    }

                    private int {|SCR0008:_value|};
                }
                """);
    }

    /// <summary>Nested types come last in any access order, and each one's own members are checked.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.NestedTypes_AnyAccessOrder_PassAndAreCheckedThemselves")]
    public Task NestedTypesAnyAccessOrderPassAndAreCheckedThemselves()
    {
        return VerifyAsync(caseId: "rules.memberorder.NestedTypes_AnyAccessOrder_PassAndAreCheckedThemselves",
                diagnostic: "SCR0008", source: """
                class C
                {
                    public void Method()
                    {
                    }

                    private sealed class First
                    {
                        private int _value;

                        public void {|SCR0008:Method|}()
                        {
                        }

                private void ReadFields52() {
                    _value = 0;
                    _ = _value;
                }
                    }

                    public interface ISecond
                    {
                    }
                }
                """);
    }

    /// <summary>
    /// The six access levels run public, internal, protected internal, protected, private protected, private.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-33.positive")]
    public Task AllSixAccessLevelsInOrderPass()
    {
        return VerifyAsync(caseId: "R-33.positive", diagnostic: "ANALYZER_ACCEPTED", source: """
                class C
                {
                    public void A()
                    {
                    }

                    internal void B()
                    {
                    }

                    protected internal void C1()
                    {
                    }

                    internal protected void C2()
                    {
                    }

                    protected void D()
                    {
                    }

                    private protected void E()
                    {
                    }

                    private void F()
                    {
                    }

                    void G()
                    {
                    }
                }
                """);
    }

    /// <summary>A protected internal member after a protected one is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_ProtectedInternalAfterProtected_Reports")]
    public Task AnalyzeProtectedInternalAfterProtectedReports()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_ProtectedInternalAfterProtected_Reports",
                diagnostic: "SCR0008", source: """
                class C
                {
                    protected void D()
                    {
                    }

                    protected internal void {|SCR0008:C1|}()
                    {
                    }
                }
                """);
    }

    /// <summary>A protected member after a private protected one is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_ProtectedAfterPrivateProtected_Reports")]
    public Task AnalyzeProtectedAfterPrivateProtectedReports()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_ProtectedAfterPrivateProtected_Reports",
                diagnostic: "SCR0008", source: """
                class C
                {
                    private protected void E()
                    {
                    }

                    protected void {|SCR0008:D|}()
                    {
                    }
                }
                """);
    }

    /// <summary>A member with no access modifier in a class is private.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_NoModifierInClass_IsPrivate")]
    public Task AnalyzeNoModifierInClassIsPrivate()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_NoModifierInClass_IsPrivate", diagnostic:
                "SCR0008", source: """
                class C
                {
                    void Method()
                    {
                    }

                    internal void {|SCR0008:Other|}()
                    {
                    }
                }
                """);
    }

    /// <summary>An explicit interface implementation ranks as public, so it may sit among the public methods.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_ExplicitInterfaceImplementation_RanksAsPublic")]
    public Task AnalyzeExplicitInterfaceImplementationRanksAsPublic()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_ExplicitInterfaceImplementation_RanksAsPublic",
                diagnostic: "ANALYZER_ACCEPTED", source: """
                using System;

                class C : IDisposable
                {
                    public void Method()
                    {
                    }

                    void IDisposable.Dispose()
                    {
                    }

                    internal void Other()
                    {
                    }
                }
                """);
    }

    /// <summary>An explicit interface implementation after an internal member is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_ExplicitInterfaceImplementationAfterInternal_Reports")]
    public Task AnalyzeExplicitInterfaceImplementationAfterInternalReports()
    {
        return VerifyAsync(caseId:
                "rules.memberorder.Analyze_ExplicitInterfaceImplementationAfterInternal_Reports", diagnostic:
                "SCR0008", source: """
                using System;

                class C : IDisposable
                {
                    internal void Other()
                    {
                    }

                    void IDisposable.{|SCR0008:Dispose|}()
                    {
                    }
                }
                """);
    }

    /// <summary>A member with no access modifier in an interface is public.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_InterfaceMembers_ArePublicAndChecked")]
    public Task AnalyzeInterfaceMembersArePublicAndChecked()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_InterfaceMembers_ArePublicAndChecked", diagnostic
                : "SCR0008", source: """
                interface I
                {
                    void Method();

                    int {|SCR0008:Property|} { get; }
                }
                """);
    }

    /// <summary>Structs, records and record structs are checked like classes.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_StructAndRecords_AreChecked")]
    public Task AnalyzeStructAndRecordsAreChecked()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_StructAndRecords_AreChecked", diagnostic:
                "SCR0008", source: """
                struct S
                {
                    public void Method()
                    {
                    }

                    public int {|SCR0008:Field|};

                private void ReadFields0() {
                    Field = 0;
                    _ = Field;
                }
                }

                record R
                {
                    public void Method()
                    {
                    }

                    public int {|SCR0008:Property|} { get; init; }
                }

                record struct Q
                {
                    private int _value;

                    public const int {|SCR0008:Max|} = 1;

                private void ReadFields189() {
                    _value = 0;
                    _ = _value;
                }
                }
                """);
    }

    /// <summary>A static constructor has no access modifier, so it ranks as a private constructor.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_StaticConstructor_RanksAsPrivateConstructor")]
    public Task AnalyzeStaticConstructorRanksAsPrivateConstructor()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_StaticConstructor_RanksAsPrivateConstructor",
                diagnostic: "SCR0008", source: """
                class C
                {
                    private static readonly int _value = 0;

                    static C()
                    {
                        _value = 1;
                    }

                    public void {|SCR0008:Method|}()
                    {
                    }

                private static void ReadFields0() {
                    _ = _value;
                }
                }
                """);
    }

    /// <summary>
    /// Each part of a partial type is checked on its own: two parts that are each in order pass, even though their
    /// members taken together are not.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_PartialParts_AreCheckedOnTheirOwn")]
    public Task AnalyzePartialPartsAreCheckedOnTheirOwn()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_PartialParts_AreCheckedOnTheirOwn", diagnostic:
                "SCR0008", source: """
                partial class C
                {
                    private int _value;

                private void ReadFields0() {
                    _value = 0;
                    _ = _value;
                }
                }

                partial class C
                {
                    public void Method()
                    {
                    }

                    private void Other()
                    {
                    }
                }

                partial class C
                {
                    private void Third()
                    {
                    }

                    public void {|SCR0008:Fourth|}()
                    {
                    }
                }
                """);
    }

    /// <summary>Only the first member out of order is reported in a type declaration part.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_SeveralViolations_OnlyTheFirstIsReported")]
    public Task AnalyzeSeveralViolationsOnlyTheFirstIsReported()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_SeveralViolations_OnlyTheFirstIsReported",
                diagnostic: "SCR0008", source: """
                class C
                {
                    private int _value;

                    public void {|SCR0008:Method|}()
                    {
                    }

                    private int _other;

                    public void Second()
                    {
                    }

                private void ReadFields0() {
                    _value = 0;
                    _ = _value;
                    _other = 0;
                    _ = _other;
                }
                }
                """);
    }

    /// <summary>The message names both members and their access and kind.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_Message_NamesBothMembers")]
    public Task AnalyzeMessageNamesBothMembers()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_Message_NamesBothMembers", diagnostic: "SCR0008",
                source: """
                class C
                {
                    private int _value;

                    public void {|SCR0008:Method|}()
                    {
                    }

                private void ReadFields0() {
                    _value = 0;
                    _ = _value;
                }
                }
                """,
                message: "'Method' (public method) must come before '_value' (private field)");
    }

    /// <summary>
    /// A C# 14 extension block takes no place in the order: one between a public method and a private field
    /// passes.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.ExtensionBlock_BetweenMembers_Passes")]
    public Task ExtensionBlockBetweenMembersPasses()
    {
        return VerifyAsync(caseId: "rules.memberorder.ExtensionBlock_BetweenMembers_Passes", diagnostic:
                "ANALYZER_ACCEPTED", source: """
                static class E
                {
                    public static int Twice(int value)
                    {
                        return value * 2;
                    }

                    extension(string s)
                    {
                        public int Doubled => Twice(value: s.Length);
                    }

                    private static int _count;
                    private static void ReadFields0() {
                        _count = 0;
                        _ = _count;
                    }
                }
                """);
    }

    /// <summary>A private method before a public method inside an extension block is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.ExtensionBlock_PrivateBeforePublic_Reports")]
    public Task ExtensionBlockPrivateBeforePublicReports()
    {
        return VerifyAsync(caseId: "rules.memberorder.ExtensionBlock_PrivateBeforePublic_Reports", diagnostic:
                "SCR0008", source: """
                static class E
                {
                    extension(string s)
                    {
                        private int Helper()
                        {
                            return s.Length;
                        }

                        public int {|SCR0008:Run|}()
                        {
                            return s.Helper();
                        }
                    }
                }
                """);
    }

    /// <summary>
    /// A member with no access modifier inside an extension block ranks as private, so an internal method after it
    /// is reported.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.ExtensionBlock_NoModifier_RanksAsPrivate")]
    public Task ExtensionBlockNoModifierRanksAsPrivate()
    {
        return VerifyAsync(caseId: "rules.memberorder.ExtensionBlock_NoModifier_RanksAsPrivate", diagnostic: "SCR0008",
                source: """
                static class E
                {
                    extension(string s)
                    {
                        int Helper()
                        {
                            return s.Length;
                        }

                        internal int {|SCR0008:Run|}()
                        {
                            return s.Helper();
                        }
                    }
                }
                """,
                message: "'Run' (internal method) must come before 'Helper' (private method)");
    }

    /// <summary>A generated marker cannot exempt a handwritten violation.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_GeneratedCode_IsSkipped")]
    public Task GeneratedConfigurationCannotExemptHandwrittenCode()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_GeneratedCode_IsSkipped", diagnostic: "SCR0008",
                source: """
                class C
                {
                    private int _value;

                    public void {|SCR0008:Method|}()
                    {
                    }

                private void ReadFields0() {
                    _value = 0;
                    _ = _value;
                }
                }
                """,
                editorConfig: """
                root = true

                [*.cs]
                generated_code = true
                """);
    }

    private static Task VerifyAsync(string caseId, string diagnostic, string source, string? editorConfig = null
            , string? message = null)
    {
        return InheritedAnalyzerVerifier.VerifyAsync(caseId: caseId, diagnostic: diagnostic, analyzerType:
                "MemberOrderAnalyzer", source: source, editorConfig: editorConfig, message: message);
    }
}
