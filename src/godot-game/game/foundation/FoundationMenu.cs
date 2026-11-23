using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Godot;
using KingdomZombies.Core;
using KingdomZombies.Multiplayer.Abstractions;
using KingdomZombies.Proofs;

namespace KingdomZombies.Foundation;

/// <summary>Owns native profile and browser controls around independent production services.</summary>
internal sealed class FoundationMenu
{
    internal FoundationMenu(Control root, MainThreadGateway gateway, FoundationServices services,
            Action join, Action back, Func<FoundationCoordinator?> coordinator,
            Action<FoundationCoordinator?> replace, FoundationInput input, FoundationRuntimeState runtime)
    {
        _root = root;
        _gateway = gateway;
        _services = services;
        _join = join;
        _back = back;
        _coordinator = coordinator;
        _replace = replace;
        _input = input;
        _runtime = runtime;
    }

    internal string? RequestedAddress { get; private set; }
    internal string RequestedSessionId { get; private set; } = "";

    internal void JoinSelected()
    {
        if (_services.Browser.Selected is SessionAdvertisement selected && selected.Availability is "Available"
                && selected.Occupied < 4)
        {
            _origin = "Browser/Footer/JoinSession";
            Submit(address: selected.Address + ":" + selected.Port.ToString(provider: CultureInfo.InvariantCulture),
                    expectedSessionId: selected.SessionId);
        }
    }

    internal void Attach()
    {
        _gateway.RequireThread();
        _root.GetNode<Button>(path: "Menu/NewProfile").Pressed += NewProfile;
        _root.GetNode<OptionButton>(path: "Menu/Profiles").ItemSelected += ProfileChosen;
        _root.GetNode<LineEdit>(path: "Menu/Name").TextSubmitted += NameSubmitted;
        _root.GetNode<LineEdit>(path: "Menu/Name").FocusExited += NameFocusLost;
        _root.GetNode<LineEdit>(path: "Browser/Address").TextSubmitted += AddressSubmitted;
        _root.GetNode<LineEdit>(path: "Browser/Address").FocusEntered += HideDetail;
        _root.GetNode<Button>(path: "Browser/Refresh").Pressed += Refresh;
        _root.GetNode<Button>(path: "Browser/Refresh").FocusEntered += HideDetail;
        _root.GetNode<Button>(path: "Browser/Footer/JoinSession").Pressed += JoinSelected;
        _root.GetNode<Button>(path: "Browser/Footer/Cancel").Pressed += Cancel;
        _root.GetNode<Button>(path: "Browser/Footer/Back").Pressed += Back;
        foreach (Control control in _root.FindChildren(pattern: "*", type: "Control", recursive: true,
                owned: false).OfType<Control>())
        {
            control.FocusEntered += FocusChanged;
            _focusControls.Add(item: control);
        }
        _services.Identity.Load();
        Profiles();
    }

    internal void Detach()
    {
        _gateway.RequireThread();
        _root.GetNode<Button>(path: "Menu/NewProfile").Pressed -= NewProfile;
        _root.GetNode<OptionButton>(path: "Menu/Profiles").ItemSelected -= ProfileChosen;
        _root.GetNode<LineEdit>(path: "Menu/Name").TextSubmitted -= NameSubmitted;
        _root.GetNode<LineEdit>(path: "Menu/Name").FocusExited -= NameFocusLost;
        _root.GetNode<LineEdit>(path: "Browser/Address").TextSubmitted -= AddressSubmitted;
        _root.GetNode<LineEdit>(path: "Browser/Address").FocusEntered -= HideDetail;
        _root.GetNode<Button>(path: "Browser/Refresh").Pressed -= Refresh;
        _root.GetNode<Button>(path: "Browser/Refresh").FocusEntered -= HideDetail;
        _root.GetNode<Button>(path: "Browser/Footer/JoinSession").Pressed -= JoinSelected;
        _root.GetNode<Button>(path: "Browser/Footer/Cancel").Pressed -= Cancel;
        _root.GetNode<Button>(path: "Browser/Footer/Back").Pressed -= Back;
        foreach (Control control in _focusControls)
        {
            control.FocusEntered -= FocusChanged;
        }

        _focusControls.Clear();
        _services.Discovery.Dispose();
    }

    internal bool CommitName()
    {
        _gateway.RequireThread();
        string value = _root.GetNode<LineEdit>(path: "Menu/Name").Text;
        bool valid = _services.Identity.Selected is not null && (StringComparer.Ordinal.Equals(
                x: value, y: _services.Identity.Selected.DisplayName) && _services.Identity.FailureReason.Length == 0
                || _services.Identity.Rename(displayName: value));
        Feedback(message: valid ? "" : _services.Identity.FailureReason);
        if (valid)
        {
            Profiles();
        }

        return valid;
    }

    internal void Browse()
    {
        _gateway.RequireThread();
        _attemptError = false;
        _root.GetNode<Label>(path: "Browser/Feedback").Text = "";
        _services.Discovery.Refresh();
        _root.GetNode<Label>(path: "Browser/Summary").Text = Summary();
        _root.GetNode<Control>(path: "Browser/Refresh").GrabFocus();
    }

    internal void Tick(long nowMilliseconds)
    {
        _gateway.RequireThread();
        Advertise();
        _services.Update(nowMilliseconds: nowMilliseconds);
        if (_coordinator()?.Ready == true)
        {
            _attemptError = false;
            _root.GetNode<Label>(path: "Browser/Feedback").Text = "";
        }

        if (!_root.GetNode<Control>(path: "Browser").Visible)
        {
            return;
        }

        bool joining = _coordinator() is FoundationCoordinator { Ready: false };
        Lock(joining: joining);
        if (!joining)
        {
            Rows();
            if (!_attemptError)
            {
                _root.GetNode<Label>(path: "Browser/Discovery").Text = _services.Browser.Feedback;
            }
        }
        else
        {
            if (_coordinator()?.InstalledRevision >= 0)
            {
                _root.GetNode<Label>(path: "Browser/Feedback").Text = "Receiving session state…";
            }
        }
    }

    internal void Attempt(string origin)
    {
        _gateway.RequireThread();
        _origin = origin;
        HideDetail();
        _attemptError = false;
        Lock(joining: true);
        _root.GetNode<Control>(path: "Browser/Footer/Cancel").GrabFocus();
    }

    internal void Recover(string message)
    {
        _gateway.RequireThread();
        _attemptError = message.Length > 0;
        Lock(joining: false);
        _root.GetNode<Label>(path: "Browser/Feedback").Text = FoundationCopy.Error(reason: message);
        Control focus = _root.GetNode<Control>(path: message.Contains(value: "profile",
                comparisonType: StringComparison.OrdinalIgnoreCase) ? "Browser/Footer/Back" : _origin);
        if (focus is Button button && button.Disabled)
        {
            focus = _root.GetNode<Control>(path: "Browser/Address");
        }

        focus.GrabFocus();
    }

    internal void Activate(Button button)
    {
        _gateway.RequireThread();
        if (!button.Disabled && button.IsVisibleInTree())
        {
            button.EmitSignal(signal: BaseButton.SignalName.Pressed);
        }
    }

    internal bool PopupOpen()
    {
        _gateway.RequireThread();
        return _root.GetNode<OptionButton>(path: "Menu/Profiles").GetPopup().Visible;
    }

    internal void CancelAttempt()
    {
        RequestedSessionId = "";
        _replace(obj: null);
        _input.Clear();
    }

    internal void Escape()
    {
        _gateway.RequireThread();
        if (_coordinator()?.Ready == true)
        {
            _back();
        }
        else if (_coordinator() is not null)
        {
            Cancel();
        }
        else if (_root.GetNode<Control>(path: "Browser").Visible)
        {
            Back();
        }
        else
        {
            _back();
        }
    }

    private readonly Control _root;
    private readonly MainThreadGateway _gateway;
    private readonly FoundationServices _services;
    private readonly Action _join;
    private readonly Action _back;
    private readonly Func<FoundationCoordinator?> _coordinator;
    private readonly Action<FoundationCoordinator?> _replace;
    private readonly FoundationInput _input;
    private readonly FoundationRuntimeState _runtime;
    private readonly List<Button> _rows = [];
    private readonly List<Control> _focusControls = [];
    private string _rowsKey = "";
    private string _origin = "Browser/JoinAddress";
    private bool _attemptError;

    private void Profiles()
    {
        _gateway.RequireThread();
        OptionButton selector = _root.GetNode<OptionButton>(path: "Menu/Profiles");
        selector.Clear();
        int index = 0;
        foreach (PlayerProfile profile in _services.Identity.Profiles)
        {
            string text = "Profile " + profile.Ordinal.ToString(provider: CultureInfo.InvariantCulture)
                    + " · " + profile.DisplayName;
            selector.AddItem(label: text);
            selector.SetItemTooltip(idx: index, tooltip: text);
            if (StringComparer.Ordinal.Equals(x: profile.PlayerId, y: _services.Identity.Selected?.PlayerId))
            {
                selector.Select(idx: index);
            }

            index++;
        }

        _root.GetNode<LineEdit>(path: "Menu/Name").Text = _services.Identity.Selected?.DisplayName ?? "";
        Feedback(message: _services.Identity.FailureReason);
    }

    private void Feedback(string message)
    {
        _gateway.RequireThread();
        _root.GetNode<Label>(path: "Menu/Error").Text = message;
        _root.GetNode<Label>(path: "Menu/Error").Visible = message.Length > 0;
    }

    private void NewProfile()
    {
        _services.Identity.Create();
        Profiles();
    }

    private void ProfileChosen(long index)
    {
        if (index >= 0 && index < _services.Identity.Profiles.Count)
        {
            _services.Identity.Choose(playerId: _services.Identity.Profiles[(int)index].PlayerId);
        }

        Profiles();
    }

    private void NameSubmitted(string text)
    {
        CommitName();
    }

    private void NameFocusLost()
    {
        CommitName();
    }

    private void FocusChanged()
    {
        _input.Clear();
    }

    private void AddressSubmitted(string text)
    {
        Submit(address: text);
    }

    private void Refresh()
    {
        _gateway.RequireThread();
        _attemptError = false;
        _root.GetNode<Label>(path: "Browser/Feedback").Text = "";
        _services.Browser.Refresh();
        _services.Discovery.Refresh();
        HideDetail();
    }

    private void Cancel()
    {
        CancelAttempt();
        Recover(message: "Join cancelled. Choose a session or enter a host address.");
    }

    private void Back()
    {
        _gateway.RequireThread();
        _back();
        _root.GetNode<Control>(path: "Menu/Join").GrabFocus();
    }

    private string Summary()
    {
        PlayerProfile? profile = _services.Identity.Selected;
        return profile is null ? "Choose a local profile" : "Playing as " + profile.DisplayName + " · Profile "
                + profile.Ordinal.ToString(provider: CultureInfo.InvariantCulture);
    }

    private void Submit(string address, string expectedSessionId = "")
    {
        RequestedAddress = address;
        RequestedSessionId = expectedSessionId;
        try
        {
            _join();
        }
        finally
        {
            RequestedAddress = null;
            RequestedSessionId = "";
        }
    }

    private void Advertise()
    {
        if (_runtime.Hosting && _coordinator()?.Snapshot is FoundationSnapshot snapshot
                && _services.Identity.Selected is PlayerProfile profile)
        {
            int count = snapshot.Participants.Length;
            int port = 47778;
#if DEBUG
            port = FoundationAutomation.Port(fallback: port);
#endif
            _services.Discovery.Advertise(advertisement: new SessionAdvertisement(sessionId: snapshot.SessionId,
                    sessionName: FoundationDisplay.Label(value: profile.DisplayName + "'s camp"),
                    hostLabel: FoundationDisplay.Label(value: profile.DisplayName), address: "",
                    port: port, occupied: count, availability: count >= 4 ? "Full" : "Available", protocol: 1,
                    content: FoundationProtocol.Content));
        }
    }

    private void Lock(bool joining)
    {
        _gateway.RequireThread();
        _root.GetNode<LineEdit>(path: "Browser/Address").Editable = !joining;
        foreach (string path in new[] { "Browser/Refresh", "Browser/JoinAddress", "Browser/Footer/Back" })
        {
            _root.GetNode<Button>(path: path).Disabled = joining;
        }

        _root.GetNode<Button>(path: "Browser/Footer/JoinSession").Visible = !joining;
        _root.GetNode<Button>(path: "Browser/JoinAddress").Visible = !joining;
        _root.GetNode<Button>(path: "Browser/Footer/Cancel").Visible = joining;
        foreach (Button row in _rows)
        {
            row.Disabled = joining;
        }
    }

    private void Rows()
    {
        _gateway.RequireThread();
        string key = string.Join(separator: "|", values: _services.Browser.Rows.Select(selector: row =>
                row.SessionId + row.SessionName + row.HostLabel + row.Address + row.Occupied + row.Availability));
        if (!StringComparer.Ordinal.Equals(x: key, y: _rowsKey))
        {
            Control? focused = _root.GetViewport().GuiGetFocusOwner();
            string previous = focused?.GetMeta(name: "session", @default: "").AsString() ?? "";
            foreach (Button row in _rows)
            {
                row.GetParent().RemoveChild(node: row);
                row.QueueFree();
            }

            _rows.Clear();
            _rowsKey = key;
            foreach (SessionAdvertisement advertisement in _services.Browser.Rows)
            {
                AddRow(advertisement: advertisement);
            }

            bool restored = false;
            foreach (Button row in _rows)
            {
                if (StringComparer.Ordinal.Equals(x: previous, y: row.GetMeta(name: "session").AsString()))
                {
                    row.GrabFocus();
                    restored = true;
                }
            }

            if (previous.Length > 0 && !restored)
            {
                _root.GetNode<Control>(path: "Browser/Refresh").GrabFocus();
            }

        }

        foreach (Button row in _rows)
        {
            bool selected = StringComparer.Ordinal.Equals(x: row.GetMeta(name: "session").AsString(),
                    y: _services.Browser.SelectedId);
            if (row.GetMeta(name: "selected", @default: !selected).AsBool() == selected)
            {
                continue;
            }

            row.SetMeta(name: "selected", value: selected);
            var style = (StyleBoxFlat)row.GetThemeStylebox(name: "normal").Duplicate();
            style.BorderColor = selected ? new Color(r: 0.941f, g: 0.769f, b: 0.416f)
                    : new Color(r: 0.957f, g: 0.949f, b: 0.91f);
            style.BorderWidthTop = 0;
            style.BorderWidthBottom = 0;
            style.BorderWidthRight = 0;
            row.AddThemeStyleboxOverride(name: "normal", stylebox: style);
        }

        _root.GetNode<Button>(path: "Browser/Footer/JoinSession").Disabled = _services.Browser.Selected is null
                || _services.Browser.Selected.Availability is not "Available"
                || _services.Browser.Selected.Occupied >= 4;
    }

    private void AddRow(SessionAdvertisement advertisement)
    {
        _gateway.RequireThread();
        var row = new Button
        {
            Name = "Session" + _rows.Count.ToString(provider: CultureInfo.InvariantCulture),
            CustomMinimumSize = new Vector2(x: 0, y: 64), Alignment = HorizontalAlignment.Left,
            TooltipText = advertisement.SessionName + "\n" + advertisement.HostLabel + "\n" + advertisement.Address
                    + ":" + advertisement.Port.ToString(provider: CultureInfo.InvariantCulture),
            ClipText = true,
        };
        RowLabel(row: row, text: advertisement.SessionName, top: 0, size: 24, right: false);
        RowLabel(row: row, text: advertisement.HostLabel.Length > 0 ? advertisement.HostLabel : advertisement.Address,
                top: 36, size: 16, right: false);
        RowLabel(row: row, text: advertisement.Occupied.ToString(provider: CultureInfo.InvariantCulture) + "/4",
                top: 0, size: 24, right: true);
        RowLabel(row: row, text: advertisement.Availability, top: 36, size: 16, right: true);
        row.SetMeta(name: "session", value: advertisement.SessionId);
        row.Pressed += () => SelectRow(session: advertisement.SessionId);
        row.FocusEntered += () => Detail(advertisement: advertisement);
        _root.GetNode<Control>(path: "Browser/Scroll/Rows").AddChild(node: row);
        _rows.Add(item: row);
    }

    private void SelectRow(string session)
    {
        _services.Browser.Select(sessionId: session);
        Rows();
    }

    private void Detail(SessionAdvertisement advertisement)
    {
        _gateway.RequireThread();
        _input.Clear();
        _services.Browser.Select(sessionId: advertisement.SessionId);
        _root.GetNode<Label>(path: "Browser/Detail").Visible = true;
        _root.GetNode<Label>(path: "Browser/Detail").Text = advertisement.SessionName + " · " + advertisement.Address
                + ":" + advertisement.Port.ToString(provider: CultureInfo.InvariantCulture);
        _root.GetNode<ScrollContainer>(path: "Browser/Scroll").EnsureControlVisible(control:
                _root.GetViewport().GuiGetFocusOwner());
    }

    private void HideDetail()
    {
        _gateway.RequireThread();
        _root.GetNode<Label>(path: "Browser/Detail").Visible = false;
    }

    private void RowLabel(Button row, string text, int top, int size, bool right)
    {
        _gateway.RequireThread();
        var label = new Label
        {
            Text = text, MouseFilter = Control.MouseFilterEnum.Ignore, AnchorRight = 1,
            AnchorLeft = right ? 1 : 0, OffsetLeft = right ? -120 : 16, OffsetRight = right ? -16 : -136,
            OffsetTop = top, OffsetBottom = top + (size == 24 ? 36 : 24),
            HorizontalAlignment = right ? HorizontalAlignment.Right : HorizontalAlignment.Left,
            TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis,
        };
        label.AddThemeFontSizeOverride(name: "font_size", fontSize: size);
        row.AddChild(node: label);
    }
}
