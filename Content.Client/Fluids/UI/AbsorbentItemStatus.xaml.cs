// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Content.Client.UserInterface.Controls;
using Content.Shared.Fluids;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Fluids.UI
{
    [GenerateTypedNameReferences]
    public sealed partial class AbsorbentItemStatus : SplitBar
    {
        private readonly IEntityManager _entManager;
        private readonly EntityUid _uid;
        private Dictionary<Color, float> _progress = new();

        public AbsorbentItemStatus(EntityUid uid, IEntityManager entManager)
        {
            RobustXamlLoader.Load(this);
            _uid = uid;
            _entManager = entManager;
        }

        protected override void FrameUpdate(FrameEventArgs args)
        {
            base.FrameUpdate(args);
            if (!_entManager.TryGetComponent<AbsorbentComponent>(_uid, out var absorbent))
                return;

            var oldProgress = _progress.ShallowClone();
            _progress.Clear();

            foreach (var item in absorbent.Progress)
            {
                _progress[item.Key] = item.Value;
            }

            if (oldProgress.OrderBy(x => x.Key.ToArgb()).SequenceEqual(_progress))
                return;

            Bar.Clear();

            foreach (var (key, value) in absorbent.Progress)
            {
                Bar.AddEntry(value, key);
            }
        }
    }
}