// SPDX-FileCopyrightText: 2021 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2021 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 E F R <602406+Efruit@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Galactic Chimp <63882831+GalacticChimp@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers <pieterjan.briers@gmail.com>
// SPDX-FileCopyrightText: 2023 Ygg01 <y.laughing.man.y@gmail.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Winkarst <74284083+Winkarst-cpu@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2025 Aviu00 <93730715+Aviu00@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 DrSmugleaf <10968691+DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 DrSmugleaf <drsmugleaf@gmail.com>
// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 Ichaie <167008606+Ichaie@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Ilya246 <57039557+Ilya246@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 JORJ949 <159719201+JORJ949@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 MortalBaguette <169563638+MortalBaguette@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Panela <107573283+AgentePanela@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 Poips <Hanakohashbrown@gmail.com>
// SPDX-FileCopyrightText: 2025 PuroSlavKing <103608145+PuroSlavKing@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Solstice <solsticeofthewinter@gmail.com>
// SPDX-FileCopyrightText: 2025 Whisper <121047731+QuietlyWhisper@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 blobadoodle <me@bloba.dev>
// SPDX-FileCopyrightText: 2025 coderabbitai[bot] <136622811+coderabbitai[bot]@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2025 github-actions[bot] <41898282+github-actions[bot]@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 kamkoi <poiiiple1@gmail.com>
// SPDX-FileCopyrightText: 2025 shibe <95730644+shibechef@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 tetra <169831122+Foralemes@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using System.Numerics;
using Content.Client._RMC14.LinkAccount;
using Content.Client.Stylesheets;
using Content.Shared.CCVar;
using Robust.Client.AutoGenerated;
using Robust.Client.Credits;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;
using static Robust.Client.UserInterface.Controls.BoxContainer;

namespace Content.Client.Credits
{
    [GenerateTypedNameReferences]
    public sealed partial class CreditsWindow : DefaultWindow
    {
        [Dependency] private readonly IEntityManager _entities = default!;
        [Dependency] private readonly IResourceManager _resourceManager = default!;
        [Dependency] private readonly IConfigurationManager _cfg = default!;
        [Dependency] private readonly LinkAccountManager _linkAccount = default!;

        private static readonly Dictionary<string, int> PatronTierPriority = new()
        {
            ["Central Command"] = 1,
            ["Captain"] = 2,
            ["Station AI"] = 3,
            ["Janitor"] = 4,
            ["Assistant"] = 5,
        };

        public CreditsWindow()
        {
            IoCManager.InjectDependencies(this);
            RobustXamlLoader.Load(this);

            TabContainer.SetTabTitle(Ss14ContributorsTab, Loc.GetString("credits-window-ss14contributorslist-tab"));
            TabContainer.SetTabTitle(PatronsTab, Loc.GetString("credits-window-patrons-tab"));
            TabContainer.SetTabTitle(LicensesTab, Loc.GetString("credits-window-licenses-tab"));

            PopulateContributors(Ss14ContributorsContainer);
            PopulatePatrons(PatronsContainer);
            PopulateLicenses(LicensesContainer);
        }

        private void PopulateLicenses(BoxContainer licensesContainer)
        {
            foreach (var entry in CreditsManager.GetLicenses(_resourceManager).OrderBy(p => p.Name))
            {
                licensesContainer.AddChild(new Label {StyleClasses = {StyleBase.StyleClassLabelHeading}, Text = entry.Name});

                // We split these line by line because otherwise
                // the LGPL causes Clyde to go out of bounds in the rendering code.
                foreach (var line in entry.License.Split("\n"))
                {
                    licensesContainer.AddChild(new Label {Text = line, FontColorOverride = new Color(200, 200, 200)});
                }
            }
        }

        private void PopulatePatrons(BoxContainer patronsContainer)
        {
            var patrons = LoadPatrons();

            var linkPatreon = _cfg.GetCVar(CCVars.InfoLinksPatreon);
            if (linkPatreon != "")
            {
                Button patronButton;
                patronsContainer.AddChild(patronButton = new Button
                {
                    Text = Loc.GetString("credits-window-become-patron-button"),
                    HorizontalAlignment = HAlignment.Center
                });

                patronButton.OnPressed +=
                    _ => IoCManager.Resolve<IUriOpener>().OpenUri(linkPatreon);
            }

            var first = true;
            foreach (var tier in patrons.GroupBy(p => p.Tier).OrderBy(p => PatronTierPriority[p.Key]))
            {
                if (!first)
                {
                    patronsContainer.AddChild(new Control {MinSize = new Vector2(0, 10)});
                }

                first = false;
                patronsContainer.AddChild(new Label {StyleClasses = {StyleBase.StyleClassLabelHeading}, Text = $"{tier.Key}"});

                var msg = string.Join(", ", tier.OrderBy(p => p.Name).Select(p => p.Name));

                var label = new RichTextLabel();
                label.SetMessage(msg);

                patronsContainer.AddChild(label);
            }
        }

        private IEnumerable<PatronEntry> LoadPatrons()
        {
            return _linkAccount.GetPatrons().Select(p => new PatronEntry(p.Name, p.Tier));
            var yamlStream = _resourceManager.ContentFileReadYaml(new ("/Credits/Patrons.yml"));
            var sequence = (YamlSequenceNode) yamlStream.Documents[0].RootNode;

            return sequence
                .Cast<YamlMappingNode>()
                .Select(m => new PatronEntry(m["Name"].AsString(), m["Tier"].AsString()));
        }

        private void PopulateContributors(BoxContainer ss14ContributorsContainer)
        {
            Button contributeButton;

            ss14ContributorsContainer.AddChild(new BoxContainer
            {
                Orientation = LayoutOrientation.Horizontal,
                HorizontalAlignment = HAlignment.Center,
                SeparationOverride = 20,
                Children =
                {
                    new Label {Text = Loc.GetString("credits-window-contributor-encouragement-label") },
                    (contributeButton = new Button {Text = Loc.GetString("credits-window-contribute-button")})
                }
            });

            var first = true;

            void AddSection(string title, string path, bool markup = false)
            {
                if (!first)
                {
                    ss14ContributorsContainer.AddChild(new Control {MinSize = new Vector2(0, 10)});
                }

                first = false;
                ss14ContributorsContainer.AddChild(new Label {StyleClasses = {StyleBase.StyleClassLabelHeading}, Text = title});

                var label = new RichTextLabel();
                var text = _resourceManager.ContentFileReadAllText($"/Credits/{path}");
                if (markup)
                {
                    label.SetMessage(FormattedMessage.FromMarkupOrThrow(text.Trim()));
                }
                else
                {
                    label.SetMessage(text);
                }

                ss14ContributorsContainer.AddChild(label);
            }

            AddSection(Loc.GetString("credits-window-contributors-section-title"), "GitHub.txt");
            AddSection(Loc.GetString("credits-window-codebases-section-title"), "SpaceStation13.txt");
            AddSection(Loc.GetString("credits-window-original-remake-team-section-title"), "OriginalRemake.txt");
            AddSection(Loc.GetString("credits-window-special-thanks-section-title"), "SpecialThanks.txt", true);

            var linkGithub = _cfg.GetCVar(CCVars.InfoLinksGithub);

            contributeButton.OnPressed += _ =>
                IoCManager.Resolve<IUriOpener>().OpenUri(linkGithub);

            if (linkGithub == "")
                contributeButton.Visible = false;
        }

        private sealed class PatronEntry
        {
            public string Name { get; }
            public string Tier { get; }

            public PatronEntry(string name, string tier)
            {
                Name = name;
                Tier = tier;
            }
        }
    }
}