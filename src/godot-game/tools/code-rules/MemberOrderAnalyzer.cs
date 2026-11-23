using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Sebreiro.CodeRules.Analyzers;
/// <summary>
/// SCR0008: members are ordered by access first, then by kind. Access runs public, internal, protected internal,
/// protected, private protected, private; an explicit interface implementation ranks as public, and a member with
/// no access modifier takes its default (private in a class, struct or record, public in an interface). Within an
/// access group, kinds run constants, fields, constructors (and the destructor), properties, indexers, events,
/// methods, operators (and conversion operators). Nested types come last, whatever their access. Each type
/// declaration part is checked on its own, and the first member that ranks below its predecessor is reported.
/// C# 14 extension blocks take no place in the order; their own members are checked like a type's.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MemberOrderAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic ID of the rule.</summary>
    public const string DiagnosticId = "SCR0008";
    /// <summary>The descriptor of the rule.</summary>
    public static readonly DiagnosticDescriptor Rule = new(id: DiagnosticId,
            title: "Member order: access first, then kind",
            messageFormat: "'{0}' ({1}) must come before '{2}' ({3})", category: "Sebreiro.CodeRules",
            defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(analysisMode: GeneratedCodeAnalysisFlags.Analyze
                | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(action: AnalyzeType, SyntaxKind.ClassDeclaration, SyntaxKind
                .StructDeclaration, SyntaxKind.RecordDeclaration, SyntaxKind.RecordStructDeclaration, SyntaxKind
                        .InterfaceDeclaration, SyntaxKind.ExtensionBlockDeclaration);
    }

    private const int KindCount = 8;
    private const int NestedTypeKind = KindCount;
    private const int NestedTypeRank = 100;
    private static readonly string[] _accessNames = ["public", "internal", "protected internal", "protected",
        "private protected", "private", ];
    private static readonly string[] _kindNames = ["constant", "field", "constructor", "property", "indexer",
        "event", "method", "operator", ];
    private static void AnalyzeType(SyntaxNodeAnalysisContext context)
    {
        var type = (TypeDeclarationSyntax)context.Node;
        bool inInterface = type is InterfaceDeclarationSyntax;
        MemberDeclarationSyntax? previous = null;
        int previousRank = -1;
        foreach (MemberDeclarationSyntax member in type.Members)
        {
            if (Rank(member: member, inInterface: inInterface) is not int rank)
            {
                continue;
            }

            if (previous is not null && rank < previousRank)
            {
                if (GeneratedSyntax.IsPermitted(diagnostic: DiagnosticId, tree: context.Node.SyntaxTree,
                        span: NameToken(member: member).Span))
                {
                    return;
                }

                context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: Rule, location: NameToken(
                        member: member).GetLocation(), Name(member: member), Describe(member: member,
                                inInterface: inInterface), Name(member: previous), Describe(member: previous,
                                        inInterface: inInterface)));
                return;
            }

            previous = member;
            previousRank = rank;
        }
    }

    private static int? Rank(MemberDeclarationSyntax member, bool inInterface)
    {
        if (KindRank(member: member) is not int kind)
        {
            return null;
        }

        return kind == NestedTypeKind ? NestedTypeRank : (AccessRank(member: member, inInterface: inInterface)
                * KindCount) + kind;
    }

    private static int? KindRank(MemberDeclarationSyntax member)
    {
        return member switch
        {
            FieldDeclarationSyntax field => field.Modifiers.Any(kind: SyntaxKind.ConstKeyword) ? 0 : 1,
            ConstructorDeclarationSyntax or DestructorDeclarationSyntax => 2,
            PropertyDeclarationSyntax => 3,
            IndexerDeclarationSyntax => 4,
            EventDeclarationSyntax or EventFieldDeclarationSyntax => 5,
            MethodDeclarationSyntax => 6,
            OperatorDeclarationSyntax or ConversionOperatorDeclarationSyntax => 7,
            ExtensionBlockDeclarationSyntax => null,
            BaseTypeDeclarationSyntax or DelegateDeclarationSyntax => NestedTypeKind,
            _ => null,
        };
    }

    private static int AccessRank(MemberDeclarationSyntax member, bool inInterface)
    {
        if (IsExplicitInterfaceImplementation(member: member))
        {
            return 0;
        }

        SyntaxTokenList modifiers = member.Modifiers;
        bool isInternal = modifiers.Any(kind: SyntaxKind.InternalKeyword);
        bool isProtected = modifiers.Any(kind: SyntaxKind.ProtectedKeyword);
        bool isPrivate = modifiers.Any(kind: SyntaxKind.PrivateKeyword);
        if (modifiers.Any(kind: SyntaxKind.PublicKeyword))
        {
            return 0;
        }

        return (isInternal, isProtected, isPrivate) switch
        {
            (true, true, _) => 2,
            (_, true, true) => 4,
            (true, _, _) => 1,
            (_, true, _) => 3,
            (_, _, true) => 5,
            _ => inInterface ? 0 : 5,
        };
    }

    private static bool IsExplicitInterfaceImplementation(MemberDeclarationSyntax member)
    {
        return member switch
        {
            MethodDeclarationSyntax method => method.ExplicitInterfaceSpecifier is not null,
            BasePropertyDeclarationSyntax property => property.ExplicitInterfaceSpecifier is not null,
            OperatorDeclarationSyntax @operator => @operator.ExplicitInterfaceSpecifier is not null,
            ConversionOperatorDeclarationSyntax conversion => conversion.ExplicitInterfaceSpecifier is not null,
            _ => false,
        };
    }

    private static string Describe(MemberDeclarationSyntax member, bool inInterface)
    {
        int kind = KindRank(member: member) ?? NestedTypeKind;
        return kind == NestedTypeKind ? "nested type" : _accessNames[AccessRank(member: member,
                inInterface: inInterface)] + " " + _kindNames[kind];
    }

    private static string Name(MemberDeclarationSyntax member)
    {
        return member switch
        {
            OperatorDeclarationSyntax @operator => "operator " + @operator.OperatorToken.Text,
            ConversionOperatorDeclarationSyntax conversion => "operator " + conversion.Type,
            _ => NameToken(member: member).Text,
        };
    }

    private static SyntaxToken NameToken(MemberDeclarationSyntax member)
    {
        return member switch
        {
            BaseFieldDeclarationSyntax field => field.Declaration.Variables[0].Identifier,
            ConstructorDeclarationSyntax constructor => constructor.Identifier,
            DestructorDeclarationSyntax destructor => destructor.Identifier,
            PropertyDeclarationSyntax property => property.Identifier,
            IndexerDeclarationSyntax indexer => indexer.ThisKeyword,
            EventDeclarationSyntax @event => @event.Identifier,
            MethodDeclarationSyntax method => method.Identifier,
            OperatorDeclarationSyntax @operator => @operator.OperatorToken,
            ConversionOperatorDeclarationSyntax conversion => conversion.OperatorKeyword,
            BaseTypeDeclarationSyntax type => type.Identifier,
            DelegateDeclarationSyntax @delegate => @delegate.Identifier,
            _ => member.GetFirstToken(),
        };
    }
}
