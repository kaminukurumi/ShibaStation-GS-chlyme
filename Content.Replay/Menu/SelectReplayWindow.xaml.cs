// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Utility;

namespace Content.Replay.Menu;

[GenerateTypedNameReferences]
public sealed partial class SelectReplayWindow : DefaultWindow
{
    private readonly ReplayMainScreen _screen;

    public SelectReplayWindow(ReplayMainScreen screen)
    {
        RobustXamlLoader.Load(this);
        _screen = screen;
        ReplayList.OnItemSelected += OnItemSelected;
    }

    private void OnItemSelected(ItemList.ItemListSelectedEventArgs obj)
    {
        var path = (ResPath?) obj.ItemList[obj.ItemIndex].Metadata;
        _screen.SelectReplay(path);
    }

    public void Repopulate(List<(string Name, ResPath Path)> replays)
    {
        ReplayList.Clear();
        foreach (var (name, path) in replays)
        {
            ReplayList.AddItem(name).Metadata = path;
        }

        if (replays.Count > 0)
        {
            NoneLabel.Visible = false;
            ReplayList.Visible = true;
        }
        else
        {
            NoneLabel.Visible = true;
            ReplayList.Visible = false;
        }
    }

    public void UpdateSelected(ResPath? replay)
    {
        if (replay == null)
        {
            ReplayList.ClearSelected();
            return;
        }

        foreach (var item in ReplayList)
        {
            var path = (ResPath?) item.Metadata;
            item.Selected = path == replay;
        }
    }
}