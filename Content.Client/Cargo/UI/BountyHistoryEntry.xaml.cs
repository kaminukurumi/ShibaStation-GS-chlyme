// SPDX-FileCopyrightText: 2025 BarryNorfolk <barrynorfolkman@protonmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.Message;
using Content.Shared.Cargo;
using Content.Shared.Cargo.Prototypes;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Cargo.UI;

[GenerateTypedNameReferences]
public sealed partial class BountyHistoryEntry : BoxContainer
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public BountyHistoryEntry(CargoBountyHistoryData bounty)
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        if (!_prototype.TryIndex(bounty.Bounty, out var bountyPrototype))
            return;

        var items = new List<string>();
        foreach (var entry in bountyPrototype.Entries)
        {
            items.Add(Loc.GetString("bounty-console-manifest-entry",
                ("amount", entry.Amount),
                ("item", Loc.GetString(entry.Name))));
        }

        ManifestLabel.SetMarkup(Loc.GetString("bounty-console-manifest-label", ("item", string.Join(", ", items))));
        RewardLabel.SetMarkup(Loc.GetString("bounty-console-reward-label", ("reward", bountyPrototype.Reward)));
        IdLabel.SetMarkup(Loc.GetString("bounty-console-id-label", ("id", bounty.Id)));

        TimestampLabel.SetMarkup(bounty.Timestamp.ToString(@"hh\:mm\:ss"));

        if (bounty.Result == CargoBountyHistoryData.BountyResult.Completed)
        {
            NoticeLabel.SetMarkup(Loc.GetString("bounty-console-history-notice-completed-label"));
        }
        else
        {
            NoticeLabel.SetMarkup(Loc.GetString("bounty-console-history-notice-skipped-label",
                ("id", bounty.ActorName ?? "")));
        }
    }
}