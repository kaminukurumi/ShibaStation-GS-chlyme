// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 beck-thompson <107373427+beck-thompson@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.Message;
using Content.Client.UserInterface.Controls;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Anomaly.Ui;

[GenerateTypedNameReferences]
public sealed partial class AnomalyScannerMenu : FancyWindow
{
    [Dependency] private readonly IGameTiming _timing = default!;

    public FormattedMessage LastMessage = new();
    public TimeSpan? NextPulseTime;

    public AnomalyScannerMenu()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);
    }

    public void UpdateMenu()
    {
        var msg = new FormattedMessage(LastMessage);

        if (NextPulseTime != null)
        {
            msg.PushNewline();
            msg.PushNewline();
            var time = NextPulseTime.Value - _timing.CurTime;
            var timestring = $"{time.Minutes:00}:{time.Seconds:00}";
            msg.AddMarkupOrThrow(Loc.GetString("anomaly-scanner-pulse-timer", ("time", timestring)));
        }

        TextDisplay.SetMarkup(msg.ToMarkup());
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);

        if (NextPulseTime != null)
            UpdateMenu();
    }
}