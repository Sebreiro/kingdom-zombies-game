using Godot;
using KingdomZombies.Proofs;

namespace KingdomZombies.Inspection;

/// <summary>Routes dismissal before GUI handling without stealing native text or activation keys.</summary>
internal sealed class InspectionInput
{
    internal InspectionInput(Node root, MainThreadGateway gateway)
    {
        _root = root;
        _gateway = gateway;
    }

    internal void Dismiss(InputEvent input)
    {
        _gateway.RequireThread();
        if (input is not InputEventKey { Pressed: true, Echo: false, Keycode: Key.Escape })
        {
            return;
        }

        foreach (Node node in _root.FindChildren(pattern: "*", type: "OptionButton", recursive: true, owned: false))
        {
            var selector = (OptionButton)node;
            if (selector.GetPopup().Visible)
            {
                selector.GetPopup().Hide();
                selector.GrabFocus();
                _root.GetViewport().SetInputAsHandled();
                return;
            }
        }

        Control drawer = _root.GetNode<Control>(path: Preview + "Metadata");
        if (drawer.Visible)
        {
            drawer.Hide();
            _root.GetNode<Button>(path: Preview + "Actions/Details").Text = "Show details";
            _root.GetNode<Button>(path: Preview + "Actions/Details").GrabFocus();
        }
        else
        {
            _root.GetNode<LineEdit>(path: "Shell/Workspace/Controls/Browser/Search").GrabFocus();
        }

        _root.GetViewport().SetInputAsHandled();
    }

    internal void ReconcileFocus()
    {
        _gateway.RequireThread();
        Control? focus = _root.GetViewport().GuiGetFocusOwner();
        if (focus is not null && (!focus.IsVisibleInTree() || focus is BaseButton { Disabled: true }))
        {
            focus.ReleaseFocus();
            _root.GetNode<LineEdit>(path: "Shell/Workspace/Controls/Browser/Search").GrabFocus();
        }
    }

    private const string Preview = "Shell/Workspace/Preview/";
    private readonly Node _root;
    private readonly MainThreadGateway _gateway;
}
