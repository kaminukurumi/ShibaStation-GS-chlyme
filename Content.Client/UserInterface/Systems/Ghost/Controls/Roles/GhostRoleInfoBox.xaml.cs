// SPDX-FileCopyrightText: 2024 MilenVolf <63782763+MilenVolf@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.UserInterface.Systems.Ghost.Controls.Roles
{
    [GenerateTypedNameReferences]
    public sealed partial class GhostRoleInfoBox : BoxContainer
    {
        public GhostRoleInfoBox(string name, string description)
        {
            RobustXamlLoader.Load(this);

            Title.Text = name;
            Description.SetMessage(description);
        }
    }
}