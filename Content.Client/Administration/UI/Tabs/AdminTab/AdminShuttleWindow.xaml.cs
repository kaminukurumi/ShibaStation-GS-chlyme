// SPDX-FileCopyrightText: 2021 moonheart08 <moonheart08@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Localizations;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Administration.UI.Tabs.AdminTab
{
    [GenerateTypedNameReferences]
    public sealed partial class AdminShuttleWindow : DefaultWindow
    {
        public AdminShuttleWindow()
        {
            RobustXamlLoader.Load(this);
            IoCManager.InjectDependencies(this);

            _callShuttleTime.OnTextChanged += CallShuttleTimeOnOnTextChanged;
        }

        private void CallShuttleTimeOnOnTextChanged(LineEdit.LineEditEventArgs obj)
        {
            var loc = IoCManager.Resolve<ILocalizationManager>();
            _callShuttleButton.Disabled = !TimeSpan.TryParseExact(obj.Text, ContentLocalizationManager.TimeSpanMinutesFormats, loc.DefaultCulture, out _);
            _callShuttleButton.Command = $"callshuttle {obj.Text}";
        }
    }
}