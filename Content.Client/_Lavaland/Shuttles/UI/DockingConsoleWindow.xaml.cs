// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aidenkrz <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2025 Aineias1 <dmitri.s.kiselev@gmail.com>
// SPDX-FileCopyrightText: 2025 FaDeOkno <143940725+FaDeOkno@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 McBosserson <148172569+McBosserson@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Milon <plmilonpl@gmail.com>
// SPDX-FileCopyrightText: 2025 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 Rouden <149893554+Roudenn@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 TheBorzoiMustConsume <197824988+TheBorzoiMustConsume@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Unlumination <144041835+Unlumy@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 coderabbitai[bot] <136622811+coderabbitai[bot]@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2025 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 gluesniffler <linebarrelerenthusiast@gmail.com>
// SPDX-FileCopyrightText: 2025 username <113782077+whateverusername0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 whateverusername0 <whateveremail>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.UserInterface.Controls;
using Content.Shared.Access.Systems;
using Content.Shared._Lavaland.Shuttles;
using Content.Shared._Lavaland.Shuttles.Components;
using Content.Shared.Shuttles.Systems;
using Content.Shared.Timing;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Timing;

namespace Content.Client._Lavaland.Shuttles.UI;

[GenerateTypedNameReferences]
public sealed partial class DockingConsoleWindow : FancyWindow
{
    [Dependency] private readonly IEntityManager _entMan = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    private readonly AccessReaderSystem _access;

    public event Action<int>? OnFTL;
    public event Action<bool>? OnShuttleCall;

    private readonly EntityUid _owner;
    private readonly StyleBoxFlat _ftlStyle;

    private FTLState _state;
    private int? _selected;
    private StartEndTime _ftlTime;

    public DockingConsoleWindow(EntityUid owner)
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        _access = _entMan.System<AccessReaderSystem>();
        _owner = owner;

        FTLButton.Disabled = false;
        ShuttleCallButton.Disabled = true;

        _ftlStyle = new StyleBoxFlat(Color.LimeGreen);
        FTLBar.ForegroundStyleBoxOverride = _ftlStyle;

        if (!_entMan.TryGetComponent<DockingConsoleComponent>(owner, out var comp))
            return;

        Title = Loc.GetString(comp.WindowTitle);

        if (!comp.HasShuttle)
        {
            MapFTLState.Text = Loc.GetString("docking-console-no-shuttle");
            _ftlStyle.BackgroundColor = Color.FromHex("#B02E26");

            FTLButton.Disabled = true;
            ShuttleCallButton.Disabled = false;

            ShuttleCallButton.OnPressed += _ =>
            {
                OnShuttleCall?.Invoke(true);
            };

            return;
        }

        Destinations.OnItemSelected += args =>
        {
            _selected = args.ItemIndex;
            UpdateButton();
        };

        Destinations.OnItemDeselected += _ =>
        {
            _selected = null;
            UpdateButton();
        };

        FTLButton.OnPressed += _ =>
        {
            if (_selected is {} index)
                OnFTL?.Invoke(index);
        };
    }

    public void UpdateState(DockingConsoleState state)
    {
        _state = state.FTLState;
        _ftlTime = state.FTLTime;

        MapFTLState.Text = Loc.GetString($"shuttle-console-ftl-state-{_state.ToString()}");
        _ftlStyle.BackgroundColor = Color.FromHex(_state switch
        {
            FTLState.Available => "#80C71F",
            FTLState.Starting => "#169C9C",
            FTLState.Travelling => "#8932B8",
            FTLState.Arriving => "#F9801D",
            _ => "#B02E26" // cooldown and fallback
        });

        UpdateButton();

        if (Destinations.Count == state.Destinations.Count)
            return;

        Destinations.Clear();
        foreach (var dest in state.Destinations)
        {
            Destinations.AddItem(dest.Name);
        }
    }

    private void UpdateButton()
    {
        MiningFtlState state = MiningFtlState.Unknown;

        if (!HasAccess())
            state = MiningFtlState.NoAccess;
        else if (_selected == null)
            state = MiningFtlState.NoSelection;
        else
        {
            state = _state switch
            {
                FTLState.Available => MiningFtlState.Ready,
                FTLState.Cooldown => MiningFtlState.RechargingFtl,
                FTLState.Starting or FTLState.Travelling or FTLState.Arriving => MiningFtlState.InFtl,
                _ => state,
            };
        }

        FTLButton.Disabled = state != MiningFtlState.Ready;
        MapFTLMessage.Text = Loc.GetString($"docking-console-ftl-message-{state.ToString()}");
    }

    private bool HasAccess()
    {
        return _player.LocalSession?.AttachedEntity is {} player && _access.IsAllowed(player, _owner);
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);
        var progress = _ftlTime.ProgressAt(_timing.CurTime);
        FTLBar.Value = float.IsFinite(progress) ? progress : 1;
    }

    private enum MiningFtlState
    {
        Unknown,
        Ready,
        NoSelection,
        NoAccess,
        InFtl,
        RechargingFtl,
    }
}