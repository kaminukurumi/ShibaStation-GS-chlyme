// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2021 metalgearsloth <comedian_vs_clown@hotmail.com>
// SPDX-FileCopyrightText: 2022 eoineoineoin <eoin.mcloughlin+gh@gmail.com>
// SPDX-FileCopyrightText: 2022 Eoin Mcloughlin <helloworld@eoinrul.es>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 ShadowCommander <10494922+ShadowCommander@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 ike709 <ike709@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 ike709 <ike709@github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Content.Client.Guidebook;
using Content.Shared.Guidebook;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;

namespace Content.Client.UserInterface.Controls
{
    [GenerateTypedNameReferences]
    [Virtual]
    public partial class FancyWindow : BaseWindow
    {
        [Dependency] private readonly IEntitySystemManager _sysMan = default!;
        private GuidebookSystem? _guidebookSystem;
        private const int DRAG_MARGIN_SIZE = 7;
        public const string StyleClassWindowHelpButton = "windowHelpButton";

        public FancyWindow()
        {
            RobustXamlLoader.Load(this);

            CloseButton.OnPressed += _ => Close();
            HelpButton.OnPressed += _ => Help();
            XamlChildren = ContentsContainer.Children;
        }

        public string? Title
        {
            get => WindowTitle.Text;
            set => WindowTitle.Text = value;
        }

        private List<ProtoId<GuideEntryPrototype>>? _helpGuidebookIds;
        public List<ProtoId<GuideEntryPrototype>>? HelpGuidebookIds
        {
            get => _helpGuidebookIds;
            set
            {
                _helpGuidebookIds = value;
                HelpButton.Disabled = _helpGuidebookIds == null;
                HelpButton.Visible = !HelpButton.Disabled;
            }
        }

        public void Help()
        {
            if (HelpGuidebookIds is null)
                return;
            _guidebookSystem ??= _sysMan.GetEntitySystem<GuidebookSystem>();
            _guidebookSystem.OpenHelp(HelpGuidebookIds);
        }

        protected override DragMode GetDragModeFor(Vector2 relativeMousePos)
        {
            var mode = DragMode.Move;

            if (Resizable)
            {
                if (relativeMousePos.Y < DRAG_MARGIN_SIZE)
                {
                    mode = DragMode.Top;
                }
                else if (relativeMousePos.Y > Size.Y - DRAG_MARGIN_SIZE)
                {
                    mode = DragMode.Bottom;
                }

                if (relativeMousePos.X < DRAG_MARGIN_SIZE)
                {
                    mode |= DragMode.Left;
                }
                else if (relativeMousePos.X > Size.X - DRAG_MARGIN_SIZE)
                {
                    mode |= DragMode.Right;
                }
            }

            return mode;
        }
    }
}