// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Aidenkrz <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Chief-Engineer <119664036+Chief-Engineer@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Repo <47093363+Titian3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 ShadowCommander <10494922+ShadowCommander@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <comedian_vs_clown@hotmail.com>
// SPDX-FileCopyrightText: 2024 nikthechampiongr <32041239+nikthechampiongr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 beck-thompson <107373427+beck-thompson@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Administration.UI.Bwoink
{
    /// <summary>
    /// This window connects to a BwoinkSystem channel. BwoinkSystem manages the rest.
    /// </summary>
    [GenerateTypedNameReferences]
    public sealed partial class BwoinkWindow : DefaultWindow
    {
        public BwoinkWindow()
        {
            RobustXamlLoader.Load(this);

            Bwoink.ChannelSelector.OnSelectionChanged += sel =>
            {
                if (sel is null)
                {
                    Title = Loc.GetString("bwoink-title-none-selected");
                    return;
                }

                Title = $"{sel.CharacterName} / {sel.Username} | {Loc.GetString("generic-playtime-title")}: ";

                Title += sel.OverallPlaytime != null ? sel.PlaytimeString : Loc.GetString("generic-unknown-title");
            };

            OnOpen += () =>
            {
                Bwoink.ChannelSelector.StopFiltering();
                Bwoink.PopulateList();
            };
        }
    }
}