// SPDX-FileCopyrightText: 2024 Firewatch <54725557+musicmanvr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Mr. 27 <45323883+Dutch-VanDerLinde@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Mr. 27 <koolthunder019@gmail.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Content.Shared.Clothing;
using Content.Shared.Preferences;
using Content.Shared.Preferences.Loadouts;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.Lobby.UI.Loadouts;

[GenerateTypedNameReferences]
public sealed partial class LoadoutGroupContainer : BoxContainer
{
    private readonly LoadoutGroupPrototype _groupProto;

    public event Action<ProtoId<LoadoutPrototype>>? OnLoadoutPressed;
    public event Action<ProtoId<LoadoutPrototype>>? OnLoadoutUnpressed;

    public LoadoutGroupContainer(HumanoidCharacterProfile profile, RoleLoadout loadout, LoadoutGroupPrototype groupProto, ICommonSession session, IDependencyCollection collection)
    {
        RobustXamlLoader.Load(this);
        _groupProto = groupProto;

        RefreshLoadouts(profile, loadout, session, collection);
    }

    /// <summary>
    /// Updates button availabilities and buttons.
    /// </summary>
    public void RefreshLoadouts(HumanoidCharacterProfile profile, RoleLoadout loadout, ICommonSession session, IDependencyCollection collection)
    {
        var protoMan = collection.Resolve<IPrototypeManager>();
        var loadoutSystem = collection.Resolve<IEntityManager>().System<LoadoutSystem>();
        RestrictionsContainer.DisposeAllChildren();

        if (_groupProto.MinLimit > 0)
        {
            RestrictionsContainer.AddChild(new Label()
            {
                Text = Loc.GetString("loadouts-min-limit", ("count", _groupProto.MinLimit)),
                Margin = new Thickness(5, 0, 5, 5),
            });
        }

        if (_groupProto.MaxLimit > 0)
        {
            RestrictionsContainer.AddChild(new Label()
            {
                Text = Loc.GetString("loadouts-max-limit", ("count", _groupProto.MaxLimit)),
                Margin = new Thickness(5, 0, 5, 5),
            });
        }

        if (protoMan.TryIndex(loadout.Role, out var roleProto) && roleProto.Points != null && loadout.Points != null)
        {
            RestrictionsContainer.AddChild(new Label()
            {
                Text = Loc.GetString("loadouts-points-limit", ("count", loadout.Points.Value), ("max", roleProto.Points.Value)),
                Margin = new Thickness(5, 0, 5, 5),
            });
        }

        LoadoutsContainer.DisposeAllChildren();
        // Didn't use options because this is more robust in future.

        var selected = loadout.SelectedLoadouts[_groupProto.ID];

        foreach (var loadoutProto in _groupProto.Loadouts)
        {
            if (!protoMan.TryIndex(loadoutProto, out var loadProto))
                continue;

            var matchingLoadout = selected.FirstOrDefault(e => e.Prototype == loadoutProto);
            var pressed = matchingLoadout != null;

            var enabled = loadout.IsValid(profile, session, loadoutProto, collection, out var reason);
            var loadoutContainer = new LoadoutContainer(loadoutProto, !enabled, reason);
            loadoutContainer.Select.Pressed = pressed;
            loadoutContainer.Text = loadoutSystem.GetName(loadProto);

            loadoutContainer.Select.OnPressed += args =>
            {
                if (args.Button.Pressed)
                    OnLoadoutPressed?.Invoke(loadoutProto);
                else
                    OnLoadoutUnpressed?.Invoke(loadoutProto);
            };

            LoadoutsContainer.AddChild(loadoutContainer);
        }
    }
}