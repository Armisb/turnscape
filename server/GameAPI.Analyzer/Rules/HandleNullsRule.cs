using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace GameAPI.Analyzer.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class HandleNullsRule : DiagnosticAnalyzer
{
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(AnalyzeNulls, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeNulls(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        foreach(var param in method.ParameterList.Parameters)
        {
            var type = context.SemanticModel.GetTypeInfo(param.Type!).Type;
            if (type == null || type.IsValueType)
                continue;
            var body = method.Body;
            if (body == null)
                continue;
            
            var paramName = param.Identifier.Text;
            var hasNullCheck = body.DescendantNodes()
                    .OfType<BinaryExpressionSyntax>()
                    .Any(b =>
                        (b.IsKind(SyntaxKind.EqualsExpression)
                      || b.IsKind(SyntaxKind.IsExpression))
                      && b.ToString().Contains(paramName) && b.ToString().Contains("null"));
            if (!hasNullCheck)
            {
                var diagnostic = Diagnostic.Create(Rule, param.GetLocation(), paramName);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    internal const string RuleId = "NULL0001";
    private static readonly DiagnosticDescriptor Rule = new(
        RuleId,
        title: "Missing null check",
        messageFormat: "Parameter '{0}' is not null-checked",
        category: "Safety",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
}
