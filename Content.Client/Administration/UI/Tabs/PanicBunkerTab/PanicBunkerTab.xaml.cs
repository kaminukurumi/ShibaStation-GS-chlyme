// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Hannah Giovanna Dawson <karakkaraz@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Administration.Events;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Console;

namespace Content.Client.Administration.UI.Tabs.PanicBunkerTab;

[GenerateTypedNameReferences]
public sealed partial class PanicBunkerTab : Control
{
    [Dependency] private readonly IConsoleHost _console = default!;

    private string _minAccountAge;
    private string _minOverallMinutes;

    public PanicBunkerTab()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        DisableAutomaticallyButton.ToolTip = Loc.GetString("admin-ui-panic-bunker-disable-automatically-tooltip");

        MinAccountAge.OnTextEntered += args => SendMinAccountAge(args.Text);
        MinAccountAge.OnFocusExit += args => SendMinAccountAge(args.Text);
        _minAccountAge = MinAccountAge.Text;

        MinOverallMinutes.OnTextEntered += args => SendMinOverallMinutes(args.Text);
        MinOverallMinutes.OnFocusExit += args => SendMinOverallMinutes(args.Text);
        _minOverallMinutes = MinOverallMinutes.Text;
    }

    private void SendMinAccountAge(string text)
    {
        if (string.IsNullOrWhiteSpace(text) ||
            text == _minAccountAge ||
            !int.TryParse(text, out var minutes))
        {
            return;
        }

        _console.ExecuteCommand($"panicbunker_min_account_age {minutes}");
    }

    private void SendMinOverallMinutes(string text)
    {
        if (string.IsNullOrWhiteSpace(text) ||
            text == _minOverallMinutes ||
            !int.TryParse(text, out var minutes))
        {
            return;
        }

        _console.ExecuteCommand($"panicbunker_min_overall_minutes {minutes}");
    }

    public void UpdateStatus(PanicBunkerStatus status)
    {
        EnabledButton.Pressed = status.Enabled;
        EnabledButton.Text = Loc.GetString(status.Enabled
            ? "admin-ui-panic-bunker-enabled"
            : "admin-ui-panic-bunker-disabled"
        );
        EnabledButton.ModulateSelfOverride = status.Enabled ? Color.Red : null;

        DisableAutomaticallyButton.Pressed = status.DisableWithAdmins;
        EnableAutomaticallyButton.Pressed = status.EnableWithoutAdmins;
        CountDeadminnedButton.Pressed = status.CountDeadminnedAdmins;
        ShowReasonButton.Pressed = status.ShowReason;

        MinAccountAge.Text = status.MinAccountAgeMinutes.ToString();
        _minAccountAge = MinAccountAge.Text;

        MinOverallMinutes.Text = status.MinOverallMinutes.ToString();
        _minOverallMinutes = MinOverallMinutes.Text;
    }
}