// SPDX-FileCopyrightText: 2024 Chief-Engineer <119664036+Chief-Engineer@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 Repo <47093363+Titian3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <comedian_vs_clown@hotmail.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.Stylesheets;
using Content.Shared.Administration;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Administration.UI.CustomControls;

[GenerateTypedNameReferences]
public sealed partial class PlayerListEntry : BoxContainer
{
    public PlayerListEntry()
    {
        RobustXamlLoader.Load(this);
    }

    public event Action<PlayerInfo>? OnPinStatusChanged;

    public void Setup(PlayerInfo info, Func<PlayerInfo, string, string>? overrideText)
    {
        Update(info, overrideText);
        PlayerEntryPinButton.OnPressed += HandlePinButtonPressed(info);
    }

    private Action<BaseButton.ButtonEventArgs> HandlePinButtonPressed(PlayerInfo info)
    {
        return args =>
        {
            info.IsPinned = !info.IsPinned;
            UpdatePinButtonTexture(info.IsPinned);
            OnPinStatusChanged?.Invoke(info);
        };
    }

    private void Update(PlayerInfo info, Func<PlayerInfo, string, string>? overrideText)
    {
        PlayerEntryLabel.Text = overrideText?.Invoke(info, $"{info.CharacterName} ({info.Username})") ??
                                $"{info.CharacterName} ({info.Username})";

        UpdatePinButtonTexture(info.IsPinned);
    }

    private void UpdatePinButtonTexture(bool isPinned)
    {
        if (isPinned)
        {
            PlayerEntryPinButton?.RemoveStyleClass(StyleNano.StyleClassPinButtonUnpinned);
            PlayerEntryPinButton?.AddStyleClass(StyleNano.StyleClassPinButtonPinned);
        }
        else
        {
            PlayerEntryPinButton?.RemoveStyleClass(StyleNano.StyleClassPinButtonPinned);
            PlayerEntryPinButton?.AddStyleClass(StyleNano.StyleClassPinButtonUnpinned);
        }
    }
}