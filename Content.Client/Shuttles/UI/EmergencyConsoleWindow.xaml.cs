// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <metalgearsloth@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Client.Computer;
using Content.Client.UserInterface.Controls;
using Content.Shared.Shuttles.BUIStates;
using Content.Shared.Shuttles.Events;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Timing;

namespace Content.Client.Shuttles.UI;

[GenerateTypedNameReferences]
public sealed partial class EmergencyConsoleWindow : FancyWindow,
    IComputerWindow<EmergencyConsoleBoundUserInterfaceState>
{
    private readonly IGameTiming _timing;
    private TimeSpan? _earlyLaunchTime;

    public EmergencyConsoleWindow()
    {
        _timing = IoCManager.Resolve<IGameTiming>();
        RobustXamlLoader.Load(this);
    }

    public void SetupComputerWindow(ComputerBoundUserInterfaceBase cb)
    {
        RepealAllButton.OnPressed += args =>
        {
            OnRepealAllPressed(cb, args);
        };
        AuthorizeButton.OnPressed += args =>
        {
            OnAuthorizePressed(cb, args);
        };
        RepealButton.OnPressed += args =>
        {
            OnRepealPressed(cb, args);
        };
    }

    private void OnRepealAllPressed(ComputerBoundUserInterfaceBase cb, BaseButton.ButtonEventArgs obj)
    {
        cb.SendMessage(new EmergencyShuttleRepealAllMessage());
    }

    private void OnRepealPressed(ComputerBoundUserInterfaceBase cb, BaseButton.ButtonEventArgs obj)
    {
        cb.SendMessage(new EmergencyShuttleRepealMessage());
    }

    private void OnAuthorizePressed(ComputerBoundUserInterfaceBase cb, BaseButton.ButtonEventArgs obj)
    {
        cb.SendMessage(new EmergencyShuttleAuthorizeMessage());
    }

    public void UpdateState(EmergencyConsoleBoundUserInterfaceState scc)
    {
        // TODO: Loc and cvar for this.
        _earlyLaunchTime = scc.EarlyLaunchTime;

        AuthorizationsContainer.DisposeAllChildren();
        var remainingAuths = scc.AuthorizationsRequired - scc.Authorizations.Count;
        AuthorizationCount.Text = Loc.GetString("emergency-shuttle-ui-remaining", ("remaining", remainingAuths));

        foreach (var auth in scc.Authorizations)
        {
            AuthorizationsContainer.AddChild(new Label
            {
                Text = auth,
                FontColorOverride = Color.Lime,
            });
        }
    }

    protected override void Draw(DrawingHandleScreen handle)
    {
        base.Draw(handle);
        if (_earlyLaunchTime == null)
        {
            Countdown.Text = "00:10";
        }
        else
        {
            var remaining = _earlyLaunchTime.Value - _timing.CurTime;

            if (remaining < TimeSpan.Zero)
                remaining = TimeSpan.Zero;

            Countdown.Text = $"{remaining.Minutes:00}:{remaining.Seconds:00}";
        }
    }
}