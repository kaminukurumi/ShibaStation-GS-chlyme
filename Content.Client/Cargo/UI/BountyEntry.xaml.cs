// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 Killerqu00 <47712032+Killerqu00@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 0x6273 <0x40@keemail.me>
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
public sealed partial class BountyEntry : BoxContainer
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public Action? OnLabelButtonPressed;
    public Action? OnSkipButtonPressed;

    public TimeSpan EndTime;
    public TimeSpan UntilNextSkip;

    public BountyEntry(CargoBountyData bounty, TimeSpan untilNextSkip)
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        UntilNextSkip = untilNextSkip;

        if (!_prototype.TryIndex<CargoBountyPrototype>(bounty.Bounty, out var bountyPrototype))
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
        DescriptionLabel.SetMarkup(Loc.GetString("bounty-console-description-label", ("description", Loc.GetString(bountyPrototype.Description))));
        IdLabel.SetMarkup(Loc.GetString("bounty-console-id-label", ("id", bounty.Id)));

        PrintButton.OnPressed += _ => OnLabelButtonPressed?.Invoke();
        SkipButton.OnPressed += _ => OnSkipButtonPressed?.Invoke();
    }

    private void UpdateSkipButton(float deltaSeconds)
    {
        UntilNextSkip -= TimeSpan.FromSeconds(deltaSeconds);
        if (UntilNextSkip > TimeSpan.Zero)
        {
            SkipButton.Label.Text = UntilNextSkip.ToString("mm\\:ss");
            SkipButton.Disabled = true;
            return;
        }

        SkipButton.Label.Text = Loc.GetString("bounty-console-skip-button-text");
        SkipButton.Disabled = false;
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);
        UpdateSkipButton(args.DeltaSeconds);
    }
}