// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 James Simonson <jamessimo89@gmail.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 0x6273 <0x40@keemail.me>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.Message;
using Content.Shared.Anomaly;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Timing;
using FancyWindow = Content.Client.UserInterface.Controls.FancyWindow;

namespace Content.Client.Anomaly.Ui;

[GenerateTypedNameReferences]
public sealed partial class AnomalyGeneratorWindow : FancyWindow
{
    [Dependency] private readonly IGameTiming _timing = default!;

    private TimeSpan _cooldownEnd = TimeSpan.Zero;
    private bool _hasEnoughFuel;

    public Action? OnGenerateButtonPressed;

    public AnomalyGeneratorWindow()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        EntityView.SpriteOffset = false;

        GenerateButton.OnPressed += _ => OnGenerateButtonPressed?.Invoke();
    }

    public void SetEntity(EntityUid uid)
    {
        EntityView.SetEntity(uid);
    }

    public void UpdateState(AnomalyGeneratorUserInterfaceState state)
    {
        _cooldownEnd = state.CooldownEndTime;
        _hasEnoughFuel = state.FuelCost <= state.FuelAmount;

        var fuelCompletion = Math.Clamp((float) state.FuelAmount / state.FuelCost, 0f, 1f);

        FuelBar.Value = fuelCompletion;

        var charges = state.FuelAmount / state.FuelCost;
        FuelText.Text = Loc.GetString("anomaly-generator-charges", ("charges", charges));

        UpdateTimer();
        UpdateReady(); // yes this can trigger twice. no i don't care
    }

    public void UpdateTimer()
    {
        if (_timing.CurTime > _cooldownEnd)
        {
            CooldownLabel.SetMarkup(Loc.GetString("anomaly-generator-no-cooldown"));
        }
        else
        {
            var timeLeft = _cooldownEnd - _timing.CurTime;
            var timeString = $"{timeLeft.Minutes:0}:{timeLeft.Seconds:00}";
            CooldownLabel.SetMarkup(Loc.GetString("anomaly-generator-cooldown", ("time", timeString)));
            UpdateReady();
        }
    }

    public void UpdateReady()
    {
        var ready = _hasEnoughFuel && _timing.CurTime > _cooldownEnd;

        var msg = ready
            ? Loc.GetString("anomaly-generator-yes-fire")
            : Loc.GetString("anomaly-generator-no-fire");
        ReadyLabel.SetMarkup(msg);

        GenerateButton.Disabled = !ready;
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);

        UpdateTimer();
    }
}
