// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 20kdc <asdd2808@gmail.com>
// SPDX-FileCopyrightText: 2024 chromiumboy <50505512+chromiumboy@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.Pinpointer.UI;
using Content.Client.UserInterface.Controls;
using Content.Shared.Power;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Map;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System.Linq;

namespace Content.Client.Power;

[GenerateTypedNameReferences]
public sealed partial class PowerMonitoringWindow : FancyWindow
{
    [Dependency] private IEntityManager _entManager = default!;
    private readonly SpriteSystem _spriteSystem;
    [Dependency] private IGameTiming _gameTiming = default!;

    private const float BlinkFrequency = 1f;

    private NetEntity? _focusEntity;

    public event Action<NetEntity?, PowerMonitoringConsoleGroup>? SendPowerMonitoringConsoleMessageAction;

    private Dictionary<PowerMonitoringConsoleGroup, (SpriteSpecifier.Texture, Color)> _groupBlips = new()
    {
        { PowerMonitoringConsoleGroup.Generator, (new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/NavMap/beveled_circle.png")), Color.Purple) },
        { PowerMonitoringConsoleGroup.SMES, (new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/NavMap/beveled_hexagon.png")), Color.OrangeRed) },
        { PowerMonitoringConsoleGroup.Substation, (new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/NavMap/beveled_square.png")), Color.Yellow) },
        { PowerMonitoringConsoleGroup.APC, (new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/NavMap/beveled_triangle.png")), Color.LimeGreen) },
    };

    public EntityUid Entity;

    public PowerMonitoringWindow()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        _spriteSystem = _entManager.System<SpriteSystem>();

        // Set trackable entity selected action
        NavMap.TrackedEntitySelectedAction += SetTrackedEntityFromNavMap;

        // Update nav map
        NavMap.ForceNavMapUpdate();

        // Set UI tab titles
        MasterTabContainer.SetTabTitle(0, Loc.GetString("power-monitoring-window-label-sources"));
        MasterTabContainer.SetTabTitle(1, Loc.GetString("power-monitoring-window-label-smes"));
        MasterTabContainer.SetTabTitle(2, Loc.GetString("power-monitoring-window-label-substation"));
        MasterTabContainer.SetTabTitle(3, Loc.GetString("power-monitoring-window-label-apc"));

        // Track when the MasterTabContainer changes its tab
        MasterTabContainer.OnTabChanged += OnTabChanged;

        // Set UI toggles
        ShowHVCable.OnToggled += _ => OnShowCableToggled(PowerMonitoringConsoleLineGroup.HighVoltage);
        ShowMVCable.OnToggled += _ => OnShowCableToggled(PowerMonitoringConsoleLineGroup.MediumVoltage);
        ShowLVCable.OnToggled += _ => OnShowCableToggled(PowerMonitoringConsoleLineGroup.Apc);
    }

    public void SetEntity(EntityUid uid)
    {
        Entity = uid;

        // Pass owner to nav map
        NavMap.Owner = uid;

        // Set nav map grid uid
        var stationName = Loc.GetString("power-monitoring-window-unknown-location");

        if (_entManager.TryGetComponent<TransformComponent>(uid, out var xform))
        {
            NavMap.MapUid = xform.GridUid;

            // Assign station name
            if (_entManager.TryGetComponent<MetaDataComponent>(xform.GridUid, out var stationMetaData))
                stationName = stationMetaData.EntityName;

            var msg = new FormattedMessage();
            msg.AddMarkupOrThrow(Loc.GetString("power-monitoring-window-station-name", ("stationName", stationName)));

            StationName.SetMessage(msg);
        }

        else
        {
            StationName.SetMessage(stationName);
            NavMap.Visible = false;
        }
    }

    private void OnTabChanged(int tab)
    {
        SendPowerMonitoringConsoleMessageAction?.Invoke(_focusEntity, (PowerMonitoringConsoleGroup) tab);
    }

    private void OnShowCableToggled(PowerMonitoringConsoleLineGroup lineGroup)
    {
        if (!NavMap.HiddenLineGroups.Remove(lineGroup))
            NavMap.HiddenLineGroups.Add(lineGroup);
    }

    public void ShowEntites
        (double totalSources,
        double totalBatteryUsage,
        double totalLoads,
        PowerMonitoringConsoleEntry[] allEntries,
        PowerMonitoringConsoleEntry[] focusSources,
        PowerMonitoringConsoleEntry[] focusLoads,
        EntityCoordinates? monitorCoords)
    {
        if (!_entManager.TryGetComponent<PowerMonitoringConsoleComponent>(Entity, out var console))
            return;

        // Update power status text
        TotalSources.Text = Loc.GetString("power-monitoring-window-value", ("value", totalSources));
        TotalBatteryUsage.Text = Loc.GetString("power-monitoring-window-value", ("value", totalBatteryUsage));
        TotalLoads.Text = Loc.GetString("power-monitoring-window-value", ("value", totalLoads));

        // 10+% of station power is being drawn from batteries
        TotalBatteryUsage.FontColorOverride = (totalSources * 0.1111f) < totalBatteryUsage ? new Color(180, 0, 0) : Color.White;

        // Station generator and battery output is less than the current demand
        TotalLoads.FontColorOverride = (totalSources + totalBatteryUsage) < totalLoads &&
            !MathHelper.CloseToPercent(totalSources + totalBatteryUsage, totalLoads, 0.1f) ? new Color(180, 0, 0) : Color.White;

        // Update system warnings
        UpdateWarningLabel(console.Flags);

        // Reset nav map values
        NavMap.TrackedCoordinates.Clear();
        NavMap.TrackedEntities.Clear();

        // Draw entities on the nav map
        var entitiesOfInterest = new List<NetEntity>();

        if (_focusEntity != null)
        {
            entitiesOfInterest.Add(_focusEntity.Value);

            foreach (var entry in focusSources)
                entitiesOfInterest.Add(entry.NetEntity);

            foreach (var entry in focusLoads)
                entitiesOfInterest.Add(entry.NetEntity);
        }

        focusSources.Concat(focusLoads);

        foreach ((var netEntity, var metaData) in console.PowerMonitoringDeviceMetaData)
        {
            if (NavMap.Visible)
                AddTrackedEntityToNavMap(netEntity, metaData, entitiesOfInterest);
        }

        // Show monitor location
        var mon = _entManager.GetNetEntity(Entity);

        if (monitorCoords != null && mon.IsValid())
        {
            var texture = _spriteSystem.Frame0(new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/NavMap/beveled_circle.png")));
            var blip = new NavMapBlip(monitorCoords.Value, texture, Color.Cyan, true, false);
            NavMap.TrackedEntities[mon] = blip;
        }

        // If the entry group doesn't match the current tab, the data is out dated, do not use it
        if (allEntries.Length > 0 && allEntries[0].Group != GetCurrentPowerMonitoringConsoleGroup())
            return;

        // Assign meta data to the console entries and sort them
        allEntries = GetUpdatedPowerMonitoringConsoleEntries(allEntries, console);
        focusSources = GetUpdatedPowerMonitoringConsoleEntries(focusSources, console);
        focusLoads = GetUpdatedPowerMonitoringConsoleEntries(focusLoads, console);

        // Get current console entry container
        BoxContainer currentContainer = SourcesList;
        switch (GetCurrentPowerMonitoringConsoleGroup())
        {
            case PowerMonitoringConsoleGroup.SMES:
                currentContainer = SMESList; break;
            case PowerMonitoringConsoleGroup.Substation:
                currentContainer = SubstationList; break;
            case PowerMonitoringConsoleGroup.APC:
                currentContainer = ApcList; break;
        }

        // Clear excess children from the container
        while (currentContainer.ChildCount > allEntries.Length)
            currentContainer.RemoveChild(currentContainer.GetChild(currentContainer.ChildCount - 1));

        // Update the remaining children
        for (var index = 0; index < allEntries.Length; index++)
        {
            var entry = allEntries[index];

            if (entry.NetEntity == _focusEntity)
                UpdateWindowConsoleEntry(currentContainer, index, entry, focusSources, focusLoads);

            else
                UpdateWindowConsoleEntry(currentContainer, index, entry);
        }

        // Auto-scroll renable
        if (_autoScrollAwaitsUpdate)
        {
            _autoScrollActive = true;
            _autoScrollAwaitsUpdate = false;
        }
    }

    private void AddTrackedEntityToNavMap(NetEntity netEntity, PowerMonitoringDeviceMetaData metaData, List<NetEntity> entitiesOfInterest)
    {
        if (!_groupBlips.TryGetValue(metaData.Group, out var data))
            return;

        var usedEntity = (metaData.CollectionMaster != null) ? metaData.CollectionMaster : netEntity;
        var coords = _entManager.GetCoordinates(metaData.Coordinates);
        var texture = data.Item1;
        var color = data.Item2;
        var blink = usedEntity == _focusEntity;
        var modulator = Color.White;

        if (_focusEntity != null && usedEntity != _focusEntity && !entitiesOfInterest.Contains(usedEntity.Value))
            modulator = Color.DimGray;

        var blip = new NavMapBlip(coords, _spriteSystem.Frame0(texture), color * modulator, blink);
        NavMap.TrackedEntities[netEntity] = blip;
    }

    private void SetTrackedEntityFromNavMap(NetEntity? netEntity)
    {
        if (netEntity == null)
            return;

        if (!_entManager.TryGetComponent<PowerMonitoringConsoleComponent>(Entity, out var console))
            return;

        if (!console.PowerMonitoringDeviceMetaData.TryGetValue(netEntity.Value, out var metaData))
            return;

        // Switch entity for master, if applicable
        // The master will always be in the same group as the entity
        if (metaData.CollectionMaster != null)
            netEntity = metaData.CollectionMaster;

        _focusEntity = netEntity;

        // Switch tabs
        SwitchTabsBasedOnPowerMonitoringConsoleGroup(metaData.Group);

        // Get the scroll position of the selected entity on the selected button the UI
        ActivateAutoScrollToFocus();

        // Send message to console that the focus has changed
        SendPowerMonitoringConsoleMessageAction?.Invoke(_focusEntity, metaData.Group);
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        AutoScrollToFocus();

        // Warning sign pulse
        var lit = _gameTiming.RealTime.TotalSeconds % BlinkFrequency > BlinkFrequency / 2f;
        SystemWarningPanel.Modulate = lit ? Color.White : new Color(178, 178, 178);
    }

    private PowerMonitoringConsoleEntry[] GetUpdatedPowerMonitoringConsoleEntries(PowerMonitoringConsoleEntry[] entries, PowerMonitoringConsoleComponent console)
    {
        for (int i = 0; i < entries.Length; i++)
        {
            var entry = entries[i];

            if (!console.PowerMonitoringDeviceMetaData.TryGetValue(entry.NetEntity, out var metaData))
                continue;

            entries[i].MetaData = metaData;
        }

        // Sort all devices alphabetically by their entity name (not by power usage; otherwise their position on the UI will shift)
        Array.Sort(entries, AlphabeticalSort);

        return entries;
    }

    private int AlphabeticalSort(PowerMonitoringConsoleEntry x, PowerMonitoringConsoleEntry y)
    {
        if (x.MetaData?.EntityName == null)
            return -1;

        if (y.MetaData?.EntityName == null)
            return 1;

        return x.MetaData.Value.EntityName.CompareTo(y.MetaData.Value.EntityName);
    }
}

public struct PowerMonitoringConsoleTrackable
{
    public EntityUid EntityUid;
    public PowerMonitoringConsoleGroup Group;

    public PowerMonitoringConsoleTrackable(EntityUid uid, PowerMonitoringConsoleGroup group)
    {
        EntityUid = uid;
        Group = group;
    }
}