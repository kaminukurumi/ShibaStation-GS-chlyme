// SPDX-FileCopyrightText: 2024 MilenVolf <63782763+MilenVolf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 Steve <marlumpy@gmail.com>
// SPDX-FileCopyrightText: 2024 chromiumboy <50505512+chromiumboy@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 coderabbitai[bot] <136622811+coderabbitai[bot]@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 marc-pelletier <113944176+marc-pelletier@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.Stylesheets;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Monitor;
using Content.Shared.FixedPoint;
using Content.Shared.Temperature;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Map;
using System.Linq;

namespace Content.Client.Atmos.Consoles;

[GenerateTypedNameReferences]
public sealed partial class AtmosAlarmEntryContainer : BoxContainer
{
    public NetEntity NetEntity;
    public EntityCoordinates? Coordinates;

    private readonly IEntityManager _entManager;
    private readonly IResourceCache _cache;

    private Dictionary<AtmosAlarmType, string> _alarmStrings = new Dictionary<AtmosAlarmType, string>()
    {
        [AtmosAlarmType.Invalid] = "atmos-alerts-window-invalid-state",
        [AtmosAlarmType.Normal] = "atmos-alerts-window-normal-state",
        [AtmosAlarmType.Warning] = "atmos-alerts-window-warning-state",
        [AtmosAlarmType.Danger] = "atmos-alerts-window-danger-state",
    };

    public AtmosAlarmEntryContainer(NetEntity uid, EntityCoordinates? coordinates)
    {
        RobustXamlLoader.Load(this);

        _entManager = IoCManager.Resolve<IEntityManager>();
        _cache = IoCManager.Resolve<IResourceCache>();

        NetEntity = uid;
        Coordinates = coordinates;

        // Load fonts
        var headerFont = new VectorFont(_cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Bold.ttf"), 11);
        var normalFont = new VectorFont(_cache.GetResource<FontResource>("/Fonts/NotoSansDisplay/NotoSansDisplay-Regular.ttf"), 11);
        var smallFont = new VectorFont(_cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf"), 10);

        // Set fonts
        TemperatureHeaderLabel.FontOverride = headerFont;
        PressureHeaderLabel.FontOverride = headerFont;
        OxygenationHeaderLabel.FontOverride = headerFont;
        GasesHeaderLabel.FontOverride = headerFont;

        TemperatureLabel.FontOverride = normalFont;
        PressureLabel.FontOverride = normalFont;
        OxygenationLabel.FontOverride = normalFont;

        NoDataLabel.FontOverride = headerFont;

        SilenceCheckBox.Label.FontOverride = smallFont;
        SilenceCheckBox.Label.FontColorOverride = Color.DarkGray;
    }

    public void UpdateEntry(AtmosAlertsComputerEntry entry, bool isFocus, AtmosAlertsFocusDeviceData? focusData = null)
    {
        NetEntity = entry.NetEntity;
        Coordinates = _entManager.GetCoordinates(entry.Coordinates);

        // Load fonts
        var normalFont = new VectorFont(_cache.GetResource<FontResource>("/Fonts/NotoSansDisplay/NotoSansDisplay-Regular.ttf"), 11);

        // Update alarm state
        if (!_alarmStrings.TryGetValue(entry.AlarmState, out var alarmString))
            alarmString = "atmos-alerts-window-invalid-state";

        AlarmStateLabel.Text = Loc.GetString(alarmString);
        AlarmStateLabel.FontColorOverride = GetAlarmStateColor(entry.AlarmState);

        // Update alarm name
        AlarmNameLabel.Text = Loc.GetString("atmos-alerts-window-alarm-label", ("name", entry.EntityName), ("address", entry.Address));

        // Focus updates
        FocusContainer.Visible = isFocus;

        if (isFocus)
            SetAsFocus();
        else
            RemoveAsFocus();

        if (isFocus && entry.Group == AtmosAlertsComputerGroup.AirAlarm)
        {
            MainDataContainer.Visible = (entry.AlarmState != AtmosAlarmType.Invalid);
            NoDataLabel.Visible = (entry.AlarmState == AtmosAlarmType.Invalid);

            if (focusData != null)
            {
                // Update temperature
                var tempK = (FixedPoint2)focusData.Value.TemperatureData.Item1;
                var tempC = (FixedPoint2)TemperatureHelpers.KelvinToCelsius(tempK.Float());

                TemperatureLabel.Text = Loc.GetString("atmos-alerts-window-temperature-value", ("valueInC", tempC), ("valueInK", tempK));
                TemperatureLabel.FontColorOverride = GetAlarmStateColor(focusData.Value.TemperatureData.Item2);

                // Update pressure
                PressureLabel.Text = Loc.GetString("atmos-alerts-window-pressure-value", ("value", (FixedPoint2)focusData.Value.PressureData.Item1));
                PressureLabel.FontColorOverride = GetAlarmStateColor(focusData.Value.PressureData.Item2);

                // Update oxygenation
                var oxygenPercent = (FixedPoint2)0f;
                var oxygenAlert = AtmosAlarmType.Invalid;

                if (focusData.Value.GasData.TryGetValue(Gas.Oxygen, out var oxygenData))
                {
                    oxygenPercent = oxygenData.Item2 * 100f;
                    oxygenAlert = oxygenData.Item3;
                }

                OxygenationLabel.Text = Loc.GetString("atmos-alerts-window-oxygenation-value", ("value", oxygenPercent));
                OxygenationLabel.FontColorOverride = GetAlarmStateColor(oxygenAlert);

                // Update other present gases
                GasGridContainer.RemoveAllChildren();

                var gasData = focusData.Value.GasData.Where(g => g.Key != Gas.Oxygen);
                var keyValuePairs = gasData.ToList();

                if (keyValuePairs.Count == 0)
                {
                    // No other gases
                    var gasLabel = new Label()
                    {
                        Text = Loc.GetString("atmos-alerts-window-other-gases-value-nil"),
                        FontOverride = normalFont,
                        FontColorOverride = StyleNano.DisabledFore,
                        HorizontalAlignment = HAlignment.Center,
                        VerticalAlignment = VAlignment.Center,
                        HorizontalExpand = true,
                        Margin = new Thickness(0, 2, 0, 0),
                        SetHeight = 24f,
                    };

                    GasGridContainer.AddChild(gasLabel);
                }

                else
                {
                    // Add an entry for each gas
                    foreach ((var gas, (var mol, var percent, var alert)) in keyValuePairs)
                    {
                        FixedPoint2 gasPercent = percent * 100f;
                        var gasAbbreviation = Atmospherics.GasAbbreviations.GetValueOrDefault(gas, Loc.GetString("gas-unknown-abbreviation"));

                        var gasLabel = new Label()
                        {
                            Text = Loc.GetString("atmos-alerts-window-other-gases-value", ("shorthand", gasAbbreviation), ("value", gasPercent)),
                            FontOverride = normalFont,
                            FontColorOverride = GetAlarmStateColor(alert),
                            HorizontalAlignment = HAlignment.Center,
                            VerticalAlignment = VAlignment.Center,
                            HorizontalExpand = true,
                            Margin = new Thickness(0, 2, 0, 0),
                            SetHeight = 24f,
                        };

                        GasGridContainer.AddChild(gasLabel);
                    }
                }
            }
        }
    }

    public void SetAsFocus()
    {
        FocusButton.AddStyleClass(StyleNano.StyleClassButtonColorGreen);
        ArrowTexture.TexturePath = "/Textures/Interface/Nano/inverted_triangle.svg.png";
    }

    public void RemoveAsFocus()
    {
        FocusButton.RemoveStyleClass(StyleNano.StyleClassButtonColorGreen);
        ArrowTexture.TexturePath = "/Textures/Interface/Nano/triangle_right.png";
        FocusContainer.Visible = false;
    }

    private Color GetAlarmStateColor(AtmosAlarmType alarmType)
    {
        switch (alarmType)
        {
            case AtmosAlarmType.Normal:
                return StyleNano.GoodGreenFore;
            case AtmosAlarmType.Warning:
                return StyleNano.ConcerningOrangeFore;
            case AtmosAlarmType.Danger:
                return StyleNano.DangerousRedFore;
        }

        return StyleNano.DisabledFore;
    }
}