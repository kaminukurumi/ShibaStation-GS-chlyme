// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Cargo.UI
{
    [GenerateTypedNameReferences]
    sealed partial class CargoConsoleOrderMenu : DefaultWindow
    {
        public CargoConsoleOrderMenu()
        {
            RobustXamlLoader.Load(this);
            IoCManager.InjectDependencies(this);

            Amount.SetButtons(new List<int> { -3, -2, -1 }, new List<int> { 1, 2, 3 });
            Amount.IsValid = n => n > 0;
        }
    }
}