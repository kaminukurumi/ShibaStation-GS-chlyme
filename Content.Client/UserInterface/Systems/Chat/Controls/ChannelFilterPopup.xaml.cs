﻿using Content.Goobstation.Common.CCVar;
using Content.Shared.Chat;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using static Robust.Client.UserInterface.Controls.BaseButton;
using Robust.Shared.Utility; // Goob - start
using Robust.Shared.Configuration;
using static Robust.Client.UserInterface.Controls.TextEdit;

namespace Content.Client.UserInterface.Systems.Chat.Controls;

[GenerateTypedNameReferences]
public sealed partial class ChannelFilterPopup : Popup
{
    // order in which the available channel filters show up when available
    private static readonly ChatChannel[] ChannelFilterOrder =
    {
        ChatChannel.Local,
        ChatChannel.Whisper,
        ChatChannel.Emotes,
        ChatChannel.Radio,
        ChatChannel.Telepathic, //Nyano - Summary: adds telepathic chat to where it belongs in order in the chat.
        ChatChannel.CollectiveMind, // Goobstation - Starlight collective mind port
        ChatChannel.Notifications,
        ChatChannel.LOOC,
        ChatChannel.OOC,
        ChatChannel.Dead,
        ChatChannel.Admin,
        ChatChannel.AdminAlert,
        ChatChannel.AdminChat,
        ChatChannel.Server,
    };

    private readonly Dictionary<ChatChannel, ChannelFilterCheckbox> _filterStates = new();

    public event Action<ChatChannel, bool>? OnChannelFilter;

    public ChannelFilterPopup()
    {
        RobustXamlLoader.Load(this);

    // Goobstation highlights - start

        HighlightButton.OnPressed += HighlightsEntered;
        // Add a placeholder text to the highlights TextEdit.
        HighlightEdit.Placeholder = new Rope.Leaf(Loc.GetString("hud-chatbox-highlights-placeholder"));

        // Load highlights if any were saved.
        var cfg = IoCManager.Resolve<IConfigurationManager>();
        string highlights = cfg.GetCVar(GoobCVars.ChatHighlights);

        if (!string.IsNullOrEmpty(highlights))
        {
            UpdateHighlights(highlights);
        }
    }
    public event Action<string>? OnNewHighlights;

    public void UpdateHighlights(string highlights)
    {
        HighlightEdit.TextRope = new Rope.Leaf(highlights);
    }

    private void HighlightsEntered(ButtonEventArgs _args)
    {
        OnNewHighlights?.Invoke(Rope.Collapse(HighlightEdit.TextRope));

    // Goobstation highlights - end

    }

    public bool IsActive(ChatChannel channel)
    {
        return _filterStates.TryGetValue(channel, out var checkbox) && checkbox.Pressed;
    }

    public ChatChannel GetActive()
    {
        ChatChannel active = 0;

        foreach (var (key, value) in _filterStates)
        {
            if (value.IsHidden || !value.Pressed)
            {
                continue;
            }

            active |= key;
        }

        return active;
    }

    public void SetChannels(ChatChannel channels)
    {
        foreach (var channel in ChannelFilterOrder)
        {
            if (!_filterStates.TryGetValue(channel, out var checkbox))
            {
                checkbox = new ChannelFilterCheckbox(channel);
                _filterStates.Add(channel, checkbox);
                checkbox.OnPressed += CheckboxPressed;
                checkbox.Pressed = true;
            }

            if ((channels & channel) == 0)
            {
                if (checkbox.Parent == FilterVBox)
                {
                    FilterVBox.RemoveChild(checkbox);
                }
            }
            else if (checkbox.IsHidden)
            {
                FilterVBox.AddChild(checkbox);
            }
        }
    }

    private void CheckboxPressed(ButtonEventArgs args)
    {
        var checkbox = (ChannelFilterCheckbox) args.Button;
        OnChannelFilter?.Invoke(checkbox.Channel, checkbox.Pressed);
    }

    public void UpdateUnread(ChatChannel channel, int? unread)
    {
        if (_filterStates.TryGetValue(channel, out var checkbox))
            checkbox.UpdateUnreadCount(unread);
    }
}
