using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Resolves engine symbols while restricting adapters to exact files, types and guarded methods.</summary>
internal static class EngineAccessPolicy
{
    /// <summary>Exposes a detached exact tuple inventory for independent permission controls.</summary>
    internal static IReadOnlyList<string> ProductionTuples => _production.SelectMany(selector: pair => pair.Value
            .Select(selector: method => pair.Key + "." + method)).ToArray();

    /// <summary>Exposes exact existing inspection signatures independently of the Foundation inventory.</summary>
    internal static IReadOnlyList<string> InspectionTuples => _inspection.SelectMany(selector: pair => pair.Value
            .Select(selector: member => "src/godot-game/game/inspection/" + pair.Key + ".cs|KingdomZombies.Inspection|"
                    + pair.Key + "|" + member)).ToArray();

    /// <summary>Rejects direct engine operations outside declared guarded adapter methods.</summary>
    internal static IReadOnlyList<string> Check(string source, string path, ConsumerCompilation? consumer = null)
    {
        consumer ??= ConsumerCompilation.Read(root: ProofProtocol.Root, project: ProjectEvaluator.Expected[2]);
        CSharpCompilation compilation = consumer.Create(source: source, path: path);
        ConsumerCompilation.RequireValid(compilation: compilation);
        SyntaxTree tree = compilation.SyntaxTrees.Single(predicate: candidate => StringComparer.Ordinal.Equals(
                x: candidate.FilePath, y: path));
        return CheckTree(tree: tree, compilation: compilation);
    }

    /// <summary>Checks resolved operations using the complete actual declaration compilation.</summary>
    internal static IReadOnlyList<string> CheckTree(SyntaxTree tree, CSharpCompilation compilation)
    {
        string path = tree.FilePath;
        SemanticModel model = compilation.GetSemanticModel(syntaxTree: tree);
        var errors = new List<string>();
        IEnumerable<IOperation> operations = tree.GetRoot().DescendantNodes().Select(selector: node =>
                model.GetOperation(node: node)).OfType<IOperation>().DistinctBy(keySelector: operation =>
                        (operation.Kind, operation.Syntax.Span));
        foreach (IOperation operation in operations)
        {
            ISymbol? symbol = operation switch
            {
                IInvocationOperation invocation => invocation.TargetMethod,
                IObjectCreationOperation creation => creation.Constructor,
                IMethodReferenceOperation reference => reference.Method,
                IPropertyReferenceOperation property => property.Property,
                IFieldReferenceOperation field => field.Field,
                IEventReferenceOperation @event => @event.Event,
                _ => null
            };
            if (symbol?.ContainingAssembly?.Name is not ("GodotSharp" or "GodotSharpEditor"))
            {
                continue;
            }

            if (operation is IMethodReferenceOperation || !Allowed(node: operation.Syntax, path: path, model: model))
            {
                FileLinePositionSpan location = operation.Syntax.GetLocation().GetLineSpan();
                errors.Add(item: "ENGINE_ACCESS " + path + "(" + (location.StartLinePosition.Line + 1)
                        .ToString(provider: CultureInfo.InvariantCulture) + "," + (location.StartLinePosition
                                .Character + 1).ToString(provider: CultureInfo.InvariantCulture) + ") "
                        + symbol.ToDisplayString());
            }
        }

        return errors;
    }

    /// <summary>Checks all actual handwritten game sources, retaining their checkout-relative identities.</summary>
    internal static IReadOnlyList<string> Repository()
    {
        var errors = new List<string>();
        var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root,
                project: ProjectEvaluator.Expected[2]);
        CSharpCompilation compilation = consumer.Create();
        ConsumerCompilation.RequireValid(compilation: compilation);
        foreach (SyntaxTree tree in compilation.SyntaxTrees.Where(predicate: tree => consumer.AuthoredPaths.Contains(
                value: tree.FilePath, comparer: StringComparer.Ordinal)))
        {
            errors.AddRange(collection: CheckTree(tree: tree, compilation: compilation));
        }

        return errors;
    }

    private static readonly Dictionary<string, string[]> _production = new(comparer: StringComparer.Ordinal)
    {
        ["FoundationRoot"] = ["FoundationRoot", "_Ready", "_Input", "_Process", "_ExitTree", "Start", "ReturnToMenu",
            "Quit", "LoseFocus", "Observe", "Capture", "Enter", "Browse", "JoinAddress", "ShowWorld",
            "PollKeys", "ActivateFocused"],
        ["FoundationView"] = ["Project", "ProjectRoster"],
        ["FoundationHud"] = ["Project"],
        ["EnetConnectionService"] = ["Host", "Join", "Poll", "Send", "Close", "Check"],
        ["FoundationAutomation"] = ["Deliver"],
        ["FoundationNativeRegression"] = ["Browser", "Projection"],
        ["FoundationDrawObserver"] = ["Start", "Stop", "Draw", "ObserveActor", "ObserveControl", "ObserveControls",
            "Capture", "Write"],
        ["FoundationMenu"] = ["Attach", "Detach", "CommitName", "Browse", "Tick", "Attempt", "Recover", "Activate",
            "PopupOpen", "Escape", "Profiles", "Feedback", "Refresh", "Back", "Lock", "Rows", "AddRow", "Detail",
            "RowLabel", "HideDetail"],
    };

    private static readonly Dictionary<string, string[]> _adapters = new(comparer: StringComparer.Ordinal)
    {
        ["Phase1"] = ["Phase1", "_Ready", "_ExitTree", "WriteEvidence"],
        ["LifecycleFixture"] = ["LifecycleFixture", "_EnterTree", "_Ready", "_Process", "_ExitTree", "Use",
            "ApplyMode", "CheckedExports"],
        ["ProofResource"] = ["ProofResource", "Read"],
        ["ProofSequence"] = ["Reject", "Tick", "Load", "Finish", "Cleanup", "Publish"]
    };

    private static readonly Dictionary<string, string[]> _inspection = new(comparer: StringComparer.Ordinal)
    {
        ["InspectionRoot"] = ["InspectionRoot()", "_Ready()", "_Process(double)", "_ExitTree()", "Recheck()",
            "Cancel()", "Read(string)", "ChangeCategory(long)", "ChangeStatus(long)", "Filter(string)",
            "Select(long)", "Details()", "Copy()", "Project()", "Rows()", "Selection()", "_Input(InputEvent)",
            "AcceptPreview(int,Node)"],
        ["InspectionInput"] = ["Dismiss(InputEvent)", "ReconcileFocus()"],
        ["InspectionShell"] = ["Attach()", "Layout()", "Tick(double)", "Install(Node)", "Retire()", "Detach()",
            "AddButton(Node,string,string,Action)", "Play()", "Speed(long)", "Step()", "Reset()", "ShowTransport()"],
    };

    private static readonly HashSet<string> _presenters = new(comparer: StringComparer.Ordinal)
    {
        "src/godot-game/game/foundation/FoundationDrawObserver.cs|KingdomZombies.Foundation."
                + "FoundationDrawObserver|ObserveAppearance()",
        "src/godot-game/game/foundation/FoundationSpectatorPanel.cs|KingdomZombies.Foundation."
                + "FoundationSpectatorPanel|Dispose()",
        "src/godot-game/game/foundation/FoundationSpectatorPanel.cs|KingdomZombies.Foundation."
                + "FoundationSpectatorPanel|Observe(Node2D,FoundationRunReadback?,FoundationCoordinator,long)",
        "src/godot-game/game/foundation/FoundationSpectatorPanel.cs|KingdomZombies.Foundation."
                + "FoundationSpectatorPanel|Remember(Node2D)",
        "src/godot-game/game/foundation/FoundationSpectatorPanel.cs|KingdomZombies.Foundation."
                + "FoundationSpectatorPanel|Build()",
        "src/godot-game/game/foundation/FoundationSpectatorPanel.cs|KingdomZombies.Foundation."
                + "FoundationSpectatorPanel|Draw()",
        "src/godot-game/game/foundation/FoundationSpectatorPanel.cs|KingdomZombies.Foundation."
                + "FoundationSpectatorPanel|DrawChoices()",
        "src/godot-game/game/foundation/FoundationSpectatorPanel.cs|KingdomZombies.Foundation."
                + "FoundationSpectatorPanel|AddButton(Control,string,string,Action,Rect2)",
        "src/godot-game/game/foundation/FoundationSpectatorPanel.cs|KingdomZombies.Foundation."
                + "FoundationSpectatorPanel|Text(Control,string,Rect2)",
        "src/godot-game/game/foundation/FoundationSpectatorPanel.cs|KingdomZombies.Foundation."
                + "FoundationSpectatorPanel|Backplate()",
        "src/godot-game/game/foundation/FoundationSpectatorPanel.cs|KingdomZombies.Foundation."
                + "FoundationSpectatorPanel|Place(Control,Rect2)",
        "src/godot-game/game/foundation/FoundationHud.Live.cs|KingdomZombies.Foundation.FoundationHud|"
                + "Dispose()",
        "src/godot-game/game/foundation/FoundationHud.Live.cs|KingdomZombies.Foundation.FoundationHud|"
                + "Hide()",
        "src/godot-game/game/foundation/FoundationHud.Live.cs|KingdomZombies.Foundation.FoundationHud|"
                + "Build()",
        "src/godot-game/game/foundation/FoundationHud.Live.cs|KingdomZombies.Foundation.FoundationHud|"
                + "BuildVitals(Control)",
        "src/godot-game/game/foundation/FoundationHud.Live.cs|KingdomZombies.Foundation.FoundationHud|"
                + "BuildActions(Control)",
        "src/godot-game/game/foundation/FoundationHud.Live.cs|KingdomZombies.Foundation.FoundationHud|"
                + "BuildDamage()",
        "src/godot-game/game/foundation/FoundationHud.Live.cs|KingdomZombies.Foundation.FoundationHud|"
                + "UpdateVitals(FoundationHudState,string?,float)",
        "src/godot-game/game/foundation/FoundationHud.Live.cs|KingdomZombies.Foundation.FoundationHud|"
                + "UpdateActions(FoundationHudState,string?,bool)",
        "src/godot-game/game/foundation/FoundationHud.Live.cs|KingdomZombies.Foundation.FoundationHud|"
                + "AddMeter(Control,string,Rect2,Color)",
        "src/godot-game/game/foundation/FoundationHud.Live.cs|KingdomZombies.Foundation.FoundationHud|"
                + "AddTile(Control,string,int,int)",
        "src/godot-game/game/foundation/FoundationHud.Live.cs|KingdomZombies.Foundation.FoundationHud|"
                + "DrawTile(Tile?,Item?,decimal,bool)",
        "src/godot-game/game/foundation/FoundationHud.Live.cs|KingdomZombies.Foundation.FoundationHud|"
                + "UpdateAmmo(int?)",
        "src/godot-game/game/foundation/FoundationHud.Live.cs|KingdomZombies.Foundation.FoundationHud|"
                + "UpdateMeter(Meter?,string,decimal,decimal)",
        "src/godot-game/game/foundation/FoundationHud.Live.cs|KingdomZombies.Foundation.FoundationHud|"
                + "Emphasize(Label?,bool)",
        "src/godot-game/game/foundation/FoundationHud.Live.cs|KingdomZombies.Foundation.FoundationHud|"
                + "Text(Control,string,Rect2)",
        "src/godot-game/game/foundation/FoundationHud.Live.cs|KingdomZombies.Foundation.FoundationHud|"
                + "SetText(Label?,string)",
        "src/godot-game/game/foundation/FoundationHud.Live.cs|KingdomZombies.Foundation.FoundationHud|"
                + "Fill(Control,string,Rect2,Color)",
        "src/godot-game/game/foundation/FoundationHud.Live.cs|KingdomZombies.Foundation.FoundationHud|"
                + "Place(Control,Rect2)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs|KingdomZombies.Fou"
                + "ndation.FoundationInventoryWorkspace|ShowNotice(string)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs|KingdomZombies.Fou"
                + "ndation.FoundationInventoryWorkspace|UpdateNotice()",
        "src/godot-game/game/foundation/FoundationInventoryArt.cs|KingdomZombies.Foundation.Foundati"
                + "onInventoryArt|Dispose()",
        "src/godot-game/game/foundation/FoundationInventoryArt.cs|KingdomZombies.Foundation.Foundati"
                + "onInventoryArt|FoundationInventoryArt(MainThreadGateway)",
        "src/godot-game/game/foundation/FoundationInventoryArt.cs|KingdomZombies.Foundation.Foundati"
                + "onInventoryArt|Atlas(string,string,string)",
        "src/godot-game/game/foundation/FoundationInventoryArt.cs|KingdomZombies.Foundation.Foundati"
                + "onInventoryArt|Read(string)",
        "src/godot-game/game/foundation/FoundationInventoryGrid.cs|KingdomZombies.Foundation.Foundat"
                + "ionInventoryGrid|Add(Control,Grid,ImmutableArray<(int x, int y)>,Action<Button, Item, InputEvent>)",
        "src/godot-game/game/foundation/FoundationInventoryGrid.cs|KingdomZombies.Foundation.Foundat"
                + "ionInventoryGrid|AddHighlights(Control,ImmutableArray<(int x, int y)>)",
        "src/godot-game/game/foundation/FoundationInventoryGrid.cs|KingdomZombies.Foundation.Foundat"
                + "ionInventoryGrid|DrawHighlights(Control,ImmutableArray<(int x, int y)>)",
        "src/godot-game/game/foundation/FoundationInventoryGrid.cs|KingdomZombies.Foundation.Foundat"
                + "ionInventoryGrid|Decorate(Button,Item,bool)",
        "src/godot-game/game/foundation/FoundationInventoryGrid.cs|KingdomZombies.Foundation.Foundat"
                + "ionInventoryGrid|ShowFocusDetails(Button,string)",
        "src/godot-game/game/foundation/FoundationInventoryGrid.cs|KingdomZombies.Foundation.Foundat"
                + "ionInventoryGrid|HideFocusDetails()",
        "src/godot-game/game/foundation/FoundationInventoryGrid.cs|KingdomZombies.Foundation.Foundat"
                + "ionInventoryGrid|DrawCells(Control,int,int)",
        "src/godot-game/game/foundation/FoundationInventoryGrid.cs|KingdomZombies.Foundation.Foundat"
                + "ionInventoryGrid|AddCount(Button,int)",
        "src/godot-game/game/foundation/FoundationInventoryGrid.cs|KingdomZombies.Foundation.Foundat"
                + "ionInventoryGrid|AddCondition(Button,decimal,decimal)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.cs|KingdomZombies.Foundation.Fo"
                + "undationInventoryWorkspace|EnsureLayer()",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.cs|KingdomZombies.Foundation.Fo"
                + "undationInventoryWorkspace|Draw()",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.cs|KingdomZombies.Foundation.Fo"
                + "undationInventoryWorkspace|RestoreFocus(string?,Button)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.cs|KingdomZombies.Foundation.Fo"
                + "undationInventoryWorkspace|DrawEquipment()",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.cs|KingdomZombies.Foundation.Fo"
                + "undationInventoryWorkspace|DrawSlot(Control,string,Item?,string,string,long,Rect,ScrollContainer?)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.cs|KingdomZombies.Foundation.Fo"
                + "undationInventoryWorkspace|DrawQuick()",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.cs|KingdomZombies.Foundation.Fo"
                + "undationInventoryWorkspace|AddBoard(VBoxContainer,Grid,bool,ImmutableArray<(int x, int y)>)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.cs|KingdomZombies.Foundation.Fo"
                + "undationInventoryWorkspace|DrawBag(Window)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|Input(InputEvent)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|ItemInput(Button,Item,bool,long,InputEvent)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|PointerMove(Vector2)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|PlacementAt(Vector2)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|Hits(TargetRegion,Vector2)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|PaintPreview(Vector2)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|DrawPreviewOutline()",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|PaintSlots(Item?)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|ItemMenu(Button,Item,bool)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|SplitMenu(Item)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|Half(int)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|ConfirmSplitField(int)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|UpdateSplit(string,int)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|ConfirmSplit(string,int)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|OpenBag(Button,Item,bool)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|Grip(Window,InputEvent)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|MoveBag(long,Vector2)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|RemovePopup()",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Input.cs|KingdomZombies.Foundat"
                + "ion.FoundationInventoryWorkspace|Feedback(string)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs|KingdomZombies.Founda"
                + "tion.FoundationInventoryWorkspace|Bounds()",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs|KingdomZombies.Founda"
                + "tion.FoundationInventoryWorkspace|Surface(Control,string,Rect,float)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs|KingdomZombies.Founda"
                + "tion.FoundationInventoryWorkspace|Put(Control,Rect)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs|KingdomZombies.Founda"
                + "tion.FoundationInventoryWorkspace|ScrollFlow(Control,string,Rect)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs|KingdomZombies.Founda"
                + "tion.FoundationInventoryWorkspace|ButtonAt(Control,string,string,Rect,Action)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs|KingdomZombies.Founda"
                + "tion.FoundationInventoryWorkspace|LabelAt(Control,string,Rect,bool)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs|KingdomZombies.Founda"
                + "tion.FoundationInventoryWorkspace|TextLine(Control,string)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs|KingdomZombies.Founda"
                + "tion.FoundationInventoryWorkspace|Option(VBoxContainer,string,Action)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs|KingdomZombies.Founda"
                + "tion.FoundationInventoryWorkspace|Popup(string,Rect2)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs|KingdomZombies.Founda"
                + "tion.FoundationInventoryWorkspace|FocusFirst(VBoxContainer)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs|KingdomZombies.Founda"
                + "tion.FoundationInventoryWorkspace|Retire(Control?)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs|KingdomZombies.Founda"
                + "tion.FoundationInventoryWorkspace|From(Rect2)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.External.cs|KingdomZombies."
                + "Foundation.FoundationInventoryWorkspace|UpdateTargetMarker(Node2D)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.External.cs|KingdomZombies."
                + "Foundation.FoundationInventoryWorkspace|DrawExternal()",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.External.cs|KingdomZombies."
                + "Foundation.FoundationInventoryWorkspace|DrawNearbySelector(Control)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.External.cs|KingdomZombies."
                + "Foundation.FoundationInventoryWorkspace|DrawNearbyChoices(Control,"
                + "ImmutableArray<(string kind, string id)>)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.External.cs|KingdomZombies."
                + "Foundation.FoundationInventoryWorkspace|BrowseEntry(LineEdit,FoundationRunView)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.External.cs|KingdomZombies."
                + "Foundation.FoundationInventoryWorkspace|DrawPager(Control,FoundationRunView)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.External.cs|KingdomZombies."
                + "Foundation.FoundationInventoryWorkspace|DrawNearbyGear("
                + "VBoxContainer,FoundationRunView,ImmutableArray<Item>)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.External.cs|KingdomZombies."
                + "Foundation.FoundationInventoryWorkspace|DrawGroundItem(VBoxContainer,Item,long)",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.External.cs|KingdomZombies."
                + "Foundation.FoundationInventoryWorkspace|InspectExternal()",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.External.cs|KingdomZombies."
                + "Foundation.FoundationInventoryWorkspace|RemoveTargetMarker()",
        "src/godot-game/game/foundation/FoundationInventoryWorkspace.External.cs|KingdomZombies."
                + "Foundation.FoundationInventoryWorkspace|NearbyFeedback(string)",
        "src/godot-game/game/foundation/FoundationActorArt.cs|KingdomZombies.Foundation.FoundationAc"
                + "torArt|Dispose()",
        "src/godot-game/game/foundation/FoundationActorArt.cs|KingdomZombies.Foundation.FoundationAc"
                + "torArt|FoundationActorArt(Node2D,MainThreadGateway,FoundationAppearanceAssets,string,bool)",
        "src/godot-game/game/foundation/FoundationActorArt.cs|KingdomZombies.Foundation.FoundationAc"
                + "torArt|Apply(Subject,long,Vector2,(double x, double y)?)",
        "src/godot-game/game/foundation/FoundationAppearance.cs|KingdomZombies.Foundation.Foundation"
                + "Appearance|Dispose()",
        "src/godot-game/game/foundation/FoundationAppearance.cs|KingdomZombies.Foundation.Foundation"
                + "Appearance|FoundationAppearance(Node2D,MainThreadGateway)",
        "src/godot-game/game/foundation/FoundationAppearance.cs|KingdomZombies.Foundation.Foundation"
                + "Appearance|ProjectActors(Dictionary<string, FoundationActorArt>,IEnumerable<Subject>,Frame,"
                + "string?,(double x, double y)?,int)",
        "src/godot-game/game/foundation/FoundationAppearance.cs|KingdomZombies.Foundation.Foundation"
                + "Appearance|Project(FoundationRunReadback,string?,(double x, double y)?)",
        "src/godot-game/game/foundation/FoundationAppearance.cs|KingdomZombies.Foundation.Foundation"
                + "Appearance|ProjectArrows(Frame)",
        "src/godot-game/game/foundation/FoundationAppearance.cs|KingdomZombies.Foundation.Foundation"
                + "Appearance|Point(Frame,(double x, double y))",
        "src/godot-game/game/foundation/FoundationAppearanceAssets.cs|KingdomZombies.Foundation.Foun"
                + "dationAppearanceAssets|Dispose()",
        "src/godot-game/game/foundation/FoundationAppearanceAssets.cs|KingdomZombies.Foundation.Foun"
                + "dationAppearanceAssets|Read(string)",
        "src/godot-game/game/foundation/FoundationAppearanceAssets.cs|KingdomZombies.Foundation.Foun"
                + "dationAppearanceAssets|SpriteTexture(JsonElement,string,string,string,string)",
        "src/godot-game/game/foundation/FoundationAutomation.cs|KingdomZombies.Foundation.Foundation"
                + "Automation|ClickWorld(Control,JsonElement,MainThreadGateway)",
        "src/godot-game/game/foundation/FoundationWorldInput.cs|KingdomZombies.Foundation.Foundation"
                + "WorldInput|AttachWorld(Node2D)",
        "src/godot-game/game/foundation/FoundationWorldInput.cs|KingdomZombies.Foundation.Foundation"
                + "WorldInput|DetachWorld()",
        "src/godot-game/game/foundation/FoundationWorldInput.cs|KingdomZombies.Foundation.Foundation"
                + "WorldInput|GainFocus()",
        "src/godot-game/game/foundation/FoundationWorldInput.cs|KingdomZombies.Foundation.Foundation"
                + "WorldInput|Refresh(bool)",
        "src/godot-game/game/foundation/FoundationWorldInput.cs|KingdomZombies.Foundation.Foundation"
                + "WorldInput|Move()",
        "src/godot-game/game/foundation/FoundationWorldInput.cs|KingdomZombies.Foundation.Foundation"
                + "WorldInput|HandleKey(InputEventKey)",
        "src/godot-game/game/foundation/FoundationWorldInput.cs|KingdomZombies.Foundation.Foundation"
                + "WorldInput|Point(FoundationRunReadback?,Frame?)",
        "src/godot-game/game/foundation/FoundationWorldInput.cs|KingdomZombies.Foundation.Foundation"
                + "WorldInput|HandleWorkspace(InputEventKey)",
        "src/godot-game/game/foundation/FoundationWorldInput.cs|KingdomZombies.Foundation.Foundation"
                + "WorldInput|Click(InputEvent)",
        "src/godot-game/game/foundation/FoundationWorldInput.cs|KingdomZombies.Foundation.Foundation"
                + "WorldInput|Aim(Vector2?)",
        "src/godot-game/game/foundation/FoundationWorldInput.cs|KingdomZombies.Foundation.Foundation"
                + "WorldInput|SetAimVisible(bool)",
        "src/godot-game/game/foundation/FoundationWorldInput.cs|KingdomZombies.Foundation.Foundation"
                + "WorldInput|Stroke(Vector2,Vector2)",
        "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|A"
                + "dmit(string,JsonElement)",
        "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|A"
                + "pplyVolume(AudioStreamPlayer,JsonElement)",
        "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|B"
                + "uild()",
        "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|B"
                + "utton(string,Action)",
        "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|C"
                + "lose()",
        "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|C"
                + "urrentMuted()",
        "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|L"
                + "imits()",
        "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|O"
                + "pen(InspectionShell?,MainThreadGateway,Func<string, byte[]?>,string)",
        "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|P"
                + "lay()",
        "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|R"
                + "econcileLimits()",
        "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|R"
                + "efresh()",
        "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|R"
                + "emove(long)",
        "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|S"
                + "tart(string,string,JsonElement,bool,bool)",
        "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|T"
                + "ick(double)",
        "src/godot-game/game/inspection/AudioPreview.cs|KingdomZombies.Inspection.AudioPreview|V"
                + "olume(string,double)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|AddAccess(Node2D,int,JsonElement)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|AddDefenseGeometry(Node2D,JsonElement,bool)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|AddDefenseGuards(Node2D,JsonElement)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|AddDefenseSprite(Node,string,string,bool)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|AddGuards(Node2D,JsonElement)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|AddSeatMark(Node)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|AddSelector(Node,string,IEnumerable<string>)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|AddSprite(Node,string,string,Vector2)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|AddSurface()",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|AimGuard(int,float,float,bool)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|Animate()",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|AnimateDefense()",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|Attach()",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|AttachDefense()",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|Detach()",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|LightSelected(long)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|Position(JsonElement)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|Project()",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|ProjectDefense()",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|ProjectDefenseMetadata(int,bool,bool,bool)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|ProjectGuardPose(Node2D,int,float,int)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|ResetDefense()",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|Restore(int,int,double,bool)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|RestoreDefense(int,int,double,bool)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|SelectDefenseSample(int)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|SelectSample(int)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|SetDefenseTexture(Sprite2D,string,bool)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|SetTexture(Sprite2D,string)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|ShowDefenseStatus(int,JsonElement,JsonElement)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|ShowStatus(int,JsonElement,JsonElement)",
        "src/godot-game/game/inspection/BuildingPreview.cs|KingdomZombies.Inspection.BuildingPre"
                + "view|ZoomSelected(long)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Add"
                + "Action(Node,string,string,string,bool)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Add"
                + "Icon(Node,string)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Add"
                + "Label(Node,string,string,string)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Att"
                + "ach(JsonElement)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Cle"
                + "ar()",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Cyc"
                + "le(JsonElement,int,long)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Map"
                + "Controls(JsonElement)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Map"
                + "Markers(JsonElement,VBoxContainer)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Map"
                + "Point(Control,JsonElement,string)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Map"
                + "Selected(long,long)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Map"
                + "Terrain(JsonElement,VBoxContainer)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Map"
                + "View(JsonElement)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Mod"
                + "al(int,int)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Not"
                + "ifications(JsonElement)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Peo"
                + "ple(JsonElement,VBoxContainer)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Pro"
                + "ject(byte[],long)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Ral"
                + "ly(JsonElement,VBoxContainer)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Sal"
                + "e(JsonElement,VBoxContainer)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Sit"
                + "e(JsonElement,VBoxContainer)",
        "src/godot-game/game/inspection/CampLayouts.cs|KingdomZombies.Inspection.CampLayouts|Spe"
                + "ctator(JsonElement,VBoxContainer)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|AddBody(Node2D,JsonElement,int,JsonElement)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|AddBowArms(Node2D,JsonElement,int)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|AddFittedSprite(Node2D,JsonElement,JsonElement,string,int,Vector2,Vector2,f"
                + "loat,JsonElement,JsonElement,string)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|AddGuard(Node2D,int)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|AddSelector(Node,string,string,IEnumerable<string>)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|AddSpentArrow(Node2D,JsonElement,int)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|AnimationControls(Node)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|Attach()",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|BowPose(JsonElement)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|CancelWork()",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|CloseComposition()",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|Contact(Node2D,JsonElement,int)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|Detach()",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|DrawComposition(JsonElement,JsonElement,JsonElement,int,JsonElement)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|FitGeometry(JsonElement,JsonElement,JsonElement,string,string,JsonElement,V"
                + "ector2)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|Frame()",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|GuardArms(Node2D,Image,JsonElement,Vector2,int)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|GuardArrow(Node2D,Vector2,int,float)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|GuardLayer(Node2D,string,Image,Vector2,int,float,Vector2)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|GuardLower(Node2D,JsonElement,int,int)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|ObligationChoice(string,string)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|OpenAttached(InspectionShell?,MainThreadGateway,Func<string, byte[]?>,strin"
                + "g)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|OpenComposition(Node2D,MainThreadGateway,Func<string, byte[]?>,string,bool)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|Play()",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|Point(JsonElement)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|Project()",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|ProjectBurst(Node2D,int)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|ProjectGear(Node2D,JsonElement,JsonElement,JsonElement,int,string)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|ProjectSpit(Node2D,JsonElement,int)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|ProjectWork(Node2D,JsonElement,JsonElement,int)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|ReconcileHands()",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|Reset()",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|RestoreComposition(string,string,int,JsonElement,string?,JsonElement,IReadOnlySet<str"
                + "ing>?)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|RetireArt()",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|SegmentOffset(bool)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|SelectObligation(string)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|Selected(string)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|Texture(JsonElement,JsonElement,string)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|TryOpenAttached(InspectionShell?,MainThreadGateway,Func<string, byte[]?>,st"
                + "ring,Node2D)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|UpdateAim(bool)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|WorkPayload(Node2D,JsonElement,int)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|WorkPose(JsonElement,JsonElement,JsonElement)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|WorkSprite(Node2D,string,string,Vector2,int,float)",
        "src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterP"
                + "review|WorkTool(Node2D,JsonElement,int)",
        "src/godot-game/game/inspection/CorpsePreview.cs|KingdomZombies.Inspection.CorpsePreview"
                + "|Close()",
        "src/godot-game/game/inspection/CorpsePreview.cs|KingdomZombies.Inspection.CorpsePreview"
                + "|CommonSprite(Node2D,string,string,Vector2)",
        "src/godot-game/game/inspection/CorpsePreview.cs|KingdomZombies.Inspection.CorpsePreview"
                + "|Outline(Node2D)",
        "src/godot-game/game/inspection/CorpsePreview.cs|KingdomZombies.Inspection.CorpsePreview"
                + "|Pile(Node2D,JsonElement,bool)",
        "src/godot-game/game/inspection/CorpsePreview.cs|KingdomZombies.Inspection.CorpsePreview"
                + "|Project()",
        "src/godot-game/game/inspection/CorpsePreview.cs|KingdomZombies.Inspection.CorpsePreview"
                + "|Retire()",
        "src/godot-game/game/inspection/CorpsePreview.cs|KingdomZombies.Inspection.CorpsePreview"
                + "|Sprite(Node2D,JsonElement,JsonElement,string,Vector2)",
        "src/godot-game/game/inspection/CorpsePreview.cs|KingdomZombies.Inspection.CorpsePreview"
                + "|Wildlife(Node2D,string,JsonElement)",
        "src/godot-game/game/inspection/CrowdPreview.cs|KingdomZombies.Inspection.CrowdPreview|A"
                + "ttach()",
        "src/godot-game/game/inspection/CrowdPreview.cs|KingdomZombies.Inspection.CrowdPreview|C"
                + "lose()",
        "src/godot-game/game/inspection/CrowdPreview.cs|KingdomZombies.Inspection.CrowdPreview|I"
                + "dentity(string)",
        "src/godot-game/game/inspection/CrowdPreview.cs|KingdomZombies.Inspection.CrowdPreview|P"
                + "ublish()",
        "src/godot-game/game/inspection/CrowdPreview.cs|KingdomZombies.Inspection.CrowdPreview|R"
                + "etireWildlife(Node2D)",
        "src/godot-game/game/inspection/CrowdPreview.cs|KingdomZombies.Inspection.CrowdPreview|S"
                + "etNight(bool)",
        "src/godot-game/game/inspection/CrowdPreview.cs|KingdomZombies.Inspection.CrowdPreview|T"
                + "exture(string)",
        "src/godot-game/game/inspection/CrowdPreview.cs|KingdomZombies.Inspection.CrowdPreview|W"
                + "ildlife(string,string,double,int,bool)",
        "src/godot-game/game/inspection/CrowdPreview.cs|KingdomZombies.Inspection.CrowdPreview|W"
                + "ildlifeOutline(Node2D,Sprite2D)",
        "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                + "view|Build()",
        "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                + "view|Close()",
        "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                + "view|Geometry(JsonElement)",
        "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                + "view|Grid(Node,JsonElement,string)",
        "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                + "view|Open(InspectionShell?,MainThreadGateway,Func<string, byte[]?>,string,Node2D)",
        "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                + "view|Project()",
        "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                + "view|ProjectLibrary()",
        "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                + "view|Restore()",
        "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                + "view|RestoreBuilding(JsonElement)",
        "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                + "view|RestoreOwners(JsonElement)",
        "src/godot-game/game/inspection/DataTextPreview.cs|KingdomZombies.Inspection.DataTextPre"
                + "view|Selector(Node,string,string[],int,ItemSelectedEventHandler)",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|AddState(string)",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|Build()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|BuildCorpse()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|BuildCrowd()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|BuildIdentityGallery()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|BuildUi()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|Changed(long)",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|Close()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|FilterCrowd()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|FilterWorld()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|Open(InspectionShell?,MainThreadGateway,Func<string, byte[]?>,string)",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|ProjectTypography(Node)",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|ProjectUiSymbol(Node,string)",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|ProjectZones()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|RequireActive()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|SelectCorpse()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|SelectCrowd()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|SelectState()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|SelectUi()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|SelectUiGallery()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|Step()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|Tick(double)",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|UiItems()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|UiTabs()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit.Res"
                + "ourceView|UiWidget(string)",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit|Add"
                + "Label(Node,string,string)",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit|Cle"
                + "ar()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit|Pro"
                + "jectBinding(JsonElement,JsonElement)",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit|Pro"
                + "jectCondition(JsonElement)",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit|Pro"
                + "jectPlayer(JsonElement)",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit|Req"
                + "uireActive()",
        "src/godot-game/game/inspection/IdentityKit.cs|KingdomZombies.Inspection.IdentityKit|Row"
                + "(string,string,string)",
        "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                + "ayouts|AddCells(Control,int,int)",
        "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                + "ayouts|AddIcon(Node,JsonElement,bool)",
        "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                + "ayouts|AddLabel(Node,string,string)",
        "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                + "ayouts|AddPlaced(Control,JsonElement)",
        "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                + "ayouts|Attach(JsonElement)",
        "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                + "ayouts|Clear()",
        "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                + "ayouts|Project(byte[],long)",
        "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                + "ayouts|ProjectActions(JsonElement,Node)",
        "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                + "ayouts|ProjectGrids(JsonElement,Node)",
        "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                + "ayouts|ProjectHistoric(Node,JsonElement,string,bool)",
        "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                + "ayouts|ProjectHistoricItem(Control,JsonElement,bool)",
        "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                + "ayouts|ProjectInventory(JsonElement)",
        "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                + "ayouts|ProjectLegacy(JsonElement)",
        "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                + "ayouts|ProjectSlots(JsonElement,Node)",
        "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                + "ayouts|ProjectStash(JsonElement,Node)",
        "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                + "ayouts|ProjectTargetRegion(Control,JsonElement,string)",
        "src/godot-game/game/inspection/InventoryLayouts.cs|KingdomZombies.Inspection.InventoryL"
                + "ayouts|ProjectTargets(JsonElement,Node)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|Action(Node,string,string,bool)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|ApplySelectors(HBoxContainer,long,long)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|Attach(JsonElement)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|Bindings(JsonElement,VBoxContainer)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|Browser(JsonElement,VBoxContainer)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|Clear()",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|Diagnostics(JsonElement,VBoxContainer)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|Endpoint(int,int,int,bool)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|Exit(JsonElement,VBoxContainer,bool)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|FinishConfirmation(long,bool)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|Join(JsonElement,VBoxContainer)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|Label(Node,string,string)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|Lobby(JsonElement,VBoxContainer)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|Main(VBoxContainer)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|Manual(JsonElement,VBoxContainer)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|ProjectSettings(JsonElement)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|RequestConfirmation(long)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|RequireActive(long)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|Saves(JsonElement,VBoxContainer)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|Selector(Node,string,string[])",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|SettingsFields(JsonElement,VBoxContainer)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|Setup(JsonElement,VBoxContainer,bool)",
        "src/godot-game/game/inspection/SessionLayouts.cs|KingdomZombies.Inspection.SessionLayou"
                + "ts|WireSelectors(HBoxContainer,JsonElement)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|A"
                + "ddCollision(string,float,float,float,float,uint)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|A"
                + "ddInspectorChoice(Node,string,IEnumerable<string>)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|A"
                + "pplyInspectorLight()",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|A"
                + "ttach()",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|A"
                + "ttachInspector()",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|C"
                + "loseInspector()",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|F"
                + "ilterLibraryWorld(string)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|I"
                + "nspectorArt()",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|L"
                + "ightInspector(long)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "opulateInspectorStates()",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "roject()",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "rojectAnimation(JsonElement,Sprite2D,double,string)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "rojectClearingMarkers()",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "rojectEffect(EffectState)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "rojectGeometry(string)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "rojectInspector()",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "rojectInspectorBuilding()",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "rojectInspectorItem()",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "rojectInterior(string,float,bool)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "rojectLibrarySites(Node2D)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "rojectLoops(JsonElement)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "rojectMap(JsonElement)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "rojectMapState(Sprite2D,string,JsonElement,JsonElement,double)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "rojectMinimap(JsonElement)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "rojectParticles(EffectState,JsonElement,double)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "rojectPortalClip(JsonElement,string,float)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "rojectPortals(string)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "rojectRuin(JsonElement,float)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|P"
                + "rojectWarning(EffectState,JsonElement)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|R"
                + "eadColor(JsonElement)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|R"
                + "equireActive()",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|R"
                + "esetInspectorItem()",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|R"
                + "estoreInspectorChoices()",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|R"
                + "etire()",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|R"
                + "etireInspector()",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|S"
                + "electLibraryBuilding(string)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|S"
                + "howInspector()",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|S"
                + "prite(string,bool,string)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|S"
                + "tateInspector(long)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|T"
                + "exture(string,bool)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|T"
                + "ransformInspector(float,float)",
        "src/godot-game/game/inspection/WorldPreview.cs|KingdomZombies.Inspection.WorldPreview|T"
                + "ransformInspectorArt()",
    };

    private static bool Allowed(SyntaxNode node, string path, SemanticModel model)
    {
        string type = node.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault()?.Identifier.ValueText ?? "";
        BaseMethodDeclarationSyntax? method = node.Ancestors().OfType<BaseMethodDeclarationSyntax>().FirstOrDefault();
        string name = method is MethodDeclarationSyntax ordinary ? ordinary.Identifier.ValueText
                : method is ConstructorDeclarationSyntax constructor ? constructor.Identifier.ValueText : "";
        bool production = StringComparer.Ordinal.Equals(x: path, y: "src/godot-game/game/foundation/" + type + ".cs")
                && StringComparer.Ordinal.Equals(x: node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>()
                        .FirstOrDefault()?.Name.ToString(), y: "KingdomZombies.Foundation")
                && node.Ancestors().OfType<TypeDeclarationSyntax>().Count() == 1
                && _production.TryGetValue(key: type, value: out string[]? productionMethods)
                && productionMethods.Contains(value: name, comparer: StringComparer.Ordinal);
        bool inspection = StringComparer.Ordinal.Equals(x: path, y: "src/godot-game/game/inspection/" + type + ".cs")
                && _inspection.TryGetValue(key: type, value: out string[]? inspectionMethods)
                && StringComparer.Ordinal.Equals(x: node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>()
                        .FirstOrDefault()?.Name.ToString(), y: "KingdomZombies.Inspection")
                && node.Ancestors().OfType<TypeDeclarationSyntax>().Count() == 1
                && method is not null && model.GetDeclaredSymbol(declaration: method) is IMethodSymbol item
                && inspectionMethods.Contains(value: name + "(" + string.Join(separator: ",", values: item.Parameters
                        .Select(selector: parameter => parameter.Type.ToDisplayString(
                                format: SymbolDisplayFormat.MinimallyQualifiedFormat))) + ")",
                        comparer: StringComparer.Ordinal) || Presenter(method: method, path: path, model: model);
        bool adapter = inspection || production || StringComparer.Ordinal.Equals(x: path,
                y: "src/godot-game/game/proofs/" + type + ".cs")
                && StringComparer.Ordinal.Equals(x: node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>()
                        .FirstOrDefault()?.Name.ToString(), y: "KingdomZombies.Proofs")
                && node.Ancestors().OfType<TypeDeclarationSyntax>().Count() == 1
                && _adapters.TryGetValue(key: type, value: out string[]? methods)
                && methods.Contains(value: name, comparer: StringComparer.Ordinal);
        return adapter && !node.Ancestors().OfType<AnonymousFunctionExpressionSyntax>().Any()
                && !node.Ancestors().OfType<LocalFunctionStatementSyntax>().Any()
                && !(method?.Modifiers.Any(kind: SyntaxKind.AsyncKeyword) ?? false)
                && (method?.Body?.Statements.OfType<ExpressionStatementSyntax>().Any(predicate: statement =>
                        (!inspection || statement.SpanStart == method?.Body?.Statements.FirstOrDefault()?.SpanStart)
                                && statement.SpanStart < node.SpanStart
                                && statement.Expression is InvocationExpressionSyntax guard
                                && model.GetSymbolInfo(node: guard).Symbol is IMethodSymbol symbol
                                && StringComparer.Ordinal.Equals(x: symbol.Name, y: "RequireThread")
                                && StringComparer.Ordinal.Equals(x: symbol.ContainingType.ToDisplayString(),
                                        y: "KingdomZombies.Proofs.MainThreadGateway")
                                && CapturedGuard(guard: guard, method: method, model: model)) ?? false);
    }

    private static bool Presenter(BaseMethodDeclarationSyntax? method, string path, SemanticModel model)
    {
        if (method is null || model.GetDeclaredSymbol(declaration: method) is not IMethodSymbol host)
        {
            return false;
        }

        string name = method is MethodDeclarationSyntax ordinary ? ordinary.Identifier.ValueText
                : method is ConstructorDeclarationSyntax constructor ? constructor.Identifier.ValueText : "";
        string signature = name + "(" + string.Join(separator: ",", values: host.Parameters.Select(
                selector: parameter => parameter.Type.ToDisplayString(
                        format: SymbolDisplayFormat.MinimallyQualifiedFormat))) + ")";
        return _presenters.Contains(item: path + "|" + host.ContainingType.ToDisplayString() + "|" + signature);
    }

    private static bool CapturedGuard(InvocationExpressionSyntax guard, BaseMethodDeclarationSyntax? method,
            SemanticModel model)
    {
        if (guard.Expression is not MemberAccessExpressionSyntax access)
        {
            return false;
        }

        ISymbol? symbol = model.GetSymbolInfo(node: access.Expression).Symbol;
        if (symbol is IPropertySymbol { IsStatic: true, Name: "Gateway", SetMethod: null } property
                && StringComparer.Ordinal.Equals(x: property.ContainingType.ToDisplayString(),
                        y: "KingdomZombies.Proofs.ProofSequence"))
        {
            return true;
        }

        if (access.Expression is not IdentifierNameSyntax)
        {
            return false;
        }
        if (symbol is IFieldSymbol { IsReadOnly: true, IsStatic: false, Name: "_gateway" })
        {
            return true;
        }

        return symbol is IParameterSymbol { Name: "gateway" } && method is not null
                && !method.DescendantNodes().OfType<AssignmentExpressionSyntax>().Any(predicate: assignment =>
                        SymbolEqualityComparer.Default.Equals(x: model.GetSymbolInfo(node: assignment.Left).Symbol,
                                y: symbol));
    }

}
