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
        return VerifyAsync(caseId: "rules.memberorder.Analyze_FullOrder_Passes", diagnostic: "ANALYZER_ACCEPTED", source: "using System;\n\nclass C\n{\n    public const int Max = 1;\n    public int Field;\n    public C() { }\n    public int Property { get; set; }\n    public int this[int index] => index;\n    public event Action Changed = delegate { };\n    public void Method() { }\n    public static C operator +(C left, C right) => left;\n    public static implicit operator int(C value) => 0;\n    internal const int InternalMax = 1;\n    internal int _internalField;\n    internal C(int value) { }\n    internal int InternalProperty { get; set; }\n    internal void InternalMethod() { }\n    protected int _protectedField;\n    protected void ProtectedMethod() { }\n    private const int PrivateMax = 1;\n    private int _privateField;\n    private C(string value) { }\n    private int PrivateProperty { get; set; }\n    private event Action PrivateChanged = delegate { };\n    private void PrivateMethod() { }\n\nprivate void ReadFields15() { Field = 0; _ = Field; _ = Changed; _internalField = 0; _ = _internalField; _protectedField = 0; _ = _protectedField; _privateField = 0; _ = _privateField; _ = PrivateChanged; }\n    private sealed class Nested { }\n    public enum Kind { None }\n    public delegate void Handler();\n}");
    }

    /// <summary>A private field declared before a public method is reported on the method.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-33.negative")]
    public Task AnalyzePrivateFieldBeforePublicMethodReports()
    {
        return VerifyAsync(caseId: "R-33.negative", diagnostic: "SCR0008", source: "class C\n{\n    private int _value;\n\n    public void {|SCR0008:Method|}()\n    {\n    }\n\nprivate void ReadFields0() { _value = 0; _ = _value; }\n}");
    }

    /// <summary>A public property after a public method is reported on the property.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_PublicPropertyAfterPublicMethod_Reports")]
    public Task AnalyzePublicPropertyAfterPublicMethodReports()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_PublicPropertyAfterPublicMethod_Reports", diagnostic: "SCR0008", source: "class C\n{\n    public void Method()\n    {\n    }\n\n    public int {|SCR0008:Property|} { get; set; }\n}");
    }

    /// <summary>A constructor after a property of the same access is reported on the constructor.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.ConstructorAfterProperty_SameAccess_Reports")]
    public Task ConstructorAfterPropertySameAccessReports()
    {
        return VerifyAsync(caseId: "rules.memberorder.ConstructorAfterProperty_SameAccess_Reports", diagnostic: "SCR0008", source: "class C\n{\n    public int Property { get; set; }\n\n    public {|SCR0008:C|}()\n    {\n    }\n}");
    }

    /// <summary>A nested type before a member is reported on the member, whatever the access of either.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_NestedTypeBeforeMember_Reports")]
    public Task AnalyzeNestedTypeBeforeMemberReports()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_NestedTypeBeforeMember_Reports", diagnostic: "SCR0008", source: "class C\n{\n\nprivate void ReadFields0() { _value = 0; _ = _value; }\n    public sealed class Nested\n    {\n    }\n\n    private int {|SCR0008:_value|};\n}");
    }

    /// <summary>Nested types come last in any access order, and each one's own members are checked.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.NestedTypes_AnyAccessOrder_PassAndAreCheckedThemselves")]
    public Task NestedTypesAnyAccessOrderPassAndAreCheckedThemselves()
    {
        return VerifyAsync(caseId: "rules.memberorder.NestedTypes_AnyAccessOrder_PassAndAreCheckedThemselves", diagnostic: "SCR0008", source: "class C\n{\n    public void Method()\n    {\n    }\n\n    private sealed class First\n    {\n        private int _value;\n\n        public void {|SCR0008:Method|}()\n        {\n        }\n\nprivate void ReadFields52() { _value = 0; _ = _value; }\n    }\n\n    public interface ISecond\n    {\n    }\n}");
    }

    /// <summary>
    /// The six access levels run public, internal, protected internal, protected, private protected, private.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-33.positive")]
    public Task AllSixAccessLevelsInOrderPass()
    {
        return VerifyAsync(caseId: "R-33.positive", diagnostic: "ANALYZER_ACCEPTED", source: "class C\n{\n    public void A()\n    {\n    }\n\n    internal void B()\n    {\n    }\n\n    protected internal void C1()\n    {\n    }\n\n    internal protected void C2()\n    {\n    }\n\n    protected void D()\n    {\n    }\n\n    private protected void E()\n    {\n    }\n\n    private void F()\n    {\n    }\n\n    void G()\n    {\n    }\n}");
    }

    /// <summary>A protected internal member after a protected one is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_ProtectedInternalAfterProtected_Reports")]
    public Task AnalyzeProtectedInternalAfterProtectedReports()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_ProtectedInternalAfterProtected_Reports", diagnostic: "SCR0008", source: "class C\n{\n    protected void D()\n    {\n    }\n\n    protected internal void {|SCR0008:C1|}()\n    {\n    }\n}");
    }

    /// <summary>A protected member after a private protected one is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_ProtectedAfterPrivateProtected_Reports")]
    public Task AnalyzeProtectedAfterPrivateProtectedReports()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_ProtectedAfterPrivateProtected_Reports", diagnostic: "SCR0008", source: "class C\n{\n    private protected void E()\n    {\n    }\n\n    protected void {|SCR0008:D|}()\n    {\n    }\n}");
    }

    /// <summary>A member with no access modifier in a class is private.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_NoModifierInClass_IsPrivate")]
    public Task AnalyzeNoModifierInClassIsPrivate()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_NoModifierInClass_IsPrivate", diagnostic: "SCR0008", source: "class C\n{\n    void Method()\n    {\n    }\n\n    internal void {|SCR0008:Other|}()\n    {\n    }\n}");
    }

    /// <summary>An explicit interface implementation ranks as public, so it may sit among the public methods.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_ExplicitInterfaceImplementation_RanksAsPublic")]
    public Task AnalyzeExplicitInterfaceImplementationRanksAsPublic()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_ExplicitInterfaceImplementation_RanksAsPublic", diagnostic: "ANALYZER_ACCEPTED", source: "using System;\n\nclass C : IDisposable\n{\n    public void Method()\n    {\n    }\n\n    void IDisposable.Dispose()\n    {\n    }\n\n    internal void Other()\n    {\n    }\n}");
    }

    /// <summary>An explicit interface implementation after an internal member is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_ExplicitInterfaceImplementationAfterInternal_Reports")]
    public Task AnalyzeExplicitInterfaceImplementationAfterInternalReports()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_ExplicitInterfaceImplementationAfterInternal_Reports", diagnostic: "SCR0008", source: "using System;\n\nclass C : IDisposable\n{\n    internal void Other()\n    {\n    }\n\n    void IDisposable.{|SCR0008:Dispose|}()\n    {\n    }\n}");
    }

    /// <summary>A member with no access modifier in an interface is public.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_InterfaceMembers_ArePublicAndChecked")]
    public Task AnalyzeInterfaceMembersArePublicAndChecked()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_InterfaceMembers_ArePublicAndChecked", diagnostic: "SCR0008", source: """
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
        return VerifyAsync(caseId: "rules.memberorder.Analyze_StructAndRecords_AreChecked", diagnostic: "SCR0008", source: "struct S\n{\n    public void Method()\n    {\n    }\n\n    public int {|SCR0008:Field|};\n\nprivate void ReadFields0() { Field = 0; _ = Field; }\n}\n\nrecord R\n{\n    public void Method()\n    {\n    }\n\n    public int {|SCR0008:Property|} { get; init; }\n}\n\nrecord struct Q\n{\n    private int _value;\n\n    public const int {|SCR0008:Max|} = 1;\n\nprivate void ReadFields189() { _value = 0; _ = _value; }\n}");
    }

    /// <summary>A static constructor has no access modifier, so it ranks as a private constructor.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_StaticConstructor_RanksAsPrivateConstructor")]
    public Task AnalyzeStaticConstructorRanksAsPrivateConstructor()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_StaticConstructor_RanksAsPrivateConstructor", diagnostic: "SCR0008", source: "class C\n{\n    private static readonly int _value = 0;\n\n    static C()\n    {\n        _value = 1;\n    }\n\n    public void {|SCR0008:Method|}()\n    {\n    }\n\nprivate static void ReadFields0() { _ = _value; }\n}");
    }

    /// <summary>
    /// Each part of a partial type is checked on its own: two parts that are each in order pass, even though their
    /// members taken together are not.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_PartialParts_AreCheckedOnTheirOwn")]
    public Task AnalyzePartialPartsAreCheckedOnTheirOwn()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_PartialParts_AreCheckedOnTheirOwn", diagnostic: "SCR0008", source: "partial class C\n{\n    private int _value;\n\nprivate void ReadFields0() { _value = 0; _ = _value; }\n}\n\npartial class C\n{\n    public void Method()\n    {\n    }\n\n    private void Other()\n    {\n    }\n}\n\npartial class C\n{\n    private void Third()\n    {\n    }\n\n    public void {|SCR0008:Fourth|}()\n    {\n    }\n}");
    }

    /// <summary>Only the first member out of order is reported in a type declaration part.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_SeveralViolations_OnlyTheFirstIsReported")]
    public Task AnalyzeSeveralViolationsOnlyTheFirstIsReported()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_SeveralViolations_OnlyTheFirstIsReported", diagnostic: "SCR0008", source: "class C\n{\n    private int _value;\n\n    public void {|SCR0008:Method|}()\n    {\n    }\n\n    private int _other;\n\n    public void Second()\n    {\n    }\n\nprivate void ReadFields0() { _value = 0; _ = _value; _other = 0; _ = _other; }\n}");
    }

    /// <summary>The message names both members and their access and kind.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_Message_NamesBothMembers")]
    public Task AnalyzeMessageNamesBothMembers()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_Message_NamesBothMembers", diagnostic: "SCR0008", 
                source: "class C\n{\n    private int _value;\n\n    public void {|SCR0008:Method|}()\n    {\n    }\n\nprivate void ReadFields0() { _value = 0; _ = _value; }\n}",
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
        return VerifyAsync(caseId: "rules.memberorder.ExtensionBlock_BetweenMembers_Passes", diagnostic: "ANALYZER_ACCEPTED", source: "static class E\n{\n    public static int Twice(int value)\n    {\n        return value * 2;\n    }\n\nprivate static void ReadFields0() { _count = 0; _ = _count; }\n\n    extension(string s)\n    {\n        public int Doubled => Twice(value: s.Length);\n    }\n\n    private static int _count;\n}");
    }

    /// <summary>A private method before a public method inside an extension block is reported.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.ExtensionBlock_PrivateBeforePublic_Reports")]
    public Task ExtensionBlockPrivateBeforePublicReports()
    {
        return VerifyAsync(caseId: "rules.memberorder.ExtensionBlock_PrivateBeforePublic_Reports", diagnostic: "SCR0008", source: "static class E\n{\n    extension(string s)\n    {\n        private int Helper()\n        {\n            return s.Length;\n        }\n\n        public int {|SCR0008:Run|}()\n        {\n            return s.Helper();\n        }\n    }\n}");
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
                source: "static class E\n{\n    extension(string s)\n    {\n        int Helper()\n        {\n            return s.Length;\n        }\n\n        internal int {|SCR0008:Run|}()\n        {\n            return s.Helper();\n        }\n    }\n}",
                message: "'Run' (internal method) must come before 'Helper' (private method)");
    }

    /// <summary>A generated marker cannot exempt a handwritten violation.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.memberorder.Analyze_GeneratedCode_IsSkipped")]
    public Task GeneratedConfigurationCannotExemptHandwrittenCode()
    {
        return VerifyAsync(caseId: "rules.memberorder.Analyze_GeneratedCode_IsSkipped", diagnostic: "SCR0008", 
                source: "class C\n{\n    private int _value;\n\n    public void {|SCR0008:Method|}()\n    {\n    }\n\nprivate void ReadFields0() { _value = 0; _ = _value; }\n}",
                editorConfig: """
                        root = true

                        [*.cs]
                        generated_code = true
                        """);
    }

    private static Task VerifyAsync(string caseId, string diagnostic, string source, string? editorConfig = null, string? message = null)
    {
        return InheritedAnalyzerVerifier.VerifyAsync(caseId: caseId, diagnostic: diagnostic, analyzerType: "MemberOrderAnalyzer", source: source, editorConfig: editorConfig, message: message);
    }
}
