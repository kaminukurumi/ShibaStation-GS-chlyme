// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 metalgearsloth <comedian_vs_clown@hotmail.com>
// SPDX-FileCopyrightText: 2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Graphics;

namespace Content.Client.Tabletop.UI
{
    [GenerateTypedNameReferences]
    public sealed partial class TabletopWindow : DefaultWindow
    {
        public TabletopWindow(IEye? eye, Vector2i size)
        {
            RobustXamlLoader.Load(this);

            ScalingVp.Eye = eye;
            ScalingVp.ViewportSize = size;

            FlipButton.OnButtonUp += Flip;
            OpenCentered();
        }

        private void Flip(BaseButton.ButtonEventArgs args)
        {
            // Flip the view 180 degrees
            if (ScalingVp.Eye is { } eye)
            {
                eye.Rotation = eye.Rotation.Opposite();

                // Flip alignmento of the button
                FlipButton.HorizontalAlignment = FlipButton.HorizontalAlignment == HAlignment.Right
                    ? HAlignment.Left
                    : HAlignment.Right;
            }
        }
    }
}