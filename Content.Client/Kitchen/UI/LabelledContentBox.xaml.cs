// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2021 Paul <ritter.paul1+git@googlemail.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Kitchen.UI
{
    [GenerateTypedNameReferences]
    public sealed partial class LabelledContentBox : BoxContainer
    {
        public string? LabelText { get => Label.Text; set => Label.Text = value; }
        public string? ButtonText { get => EjectButton.Text; set => EjectButton.Text = value; }
    }
}