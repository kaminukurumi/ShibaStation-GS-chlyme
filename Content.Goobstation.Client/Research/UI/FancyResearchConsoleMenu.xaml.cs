// SPDX-FileCopyrightText: 2025 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 coderabbitai[bot] <136622811+coderabbitai[bot]@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2025 FaDeOkno <logkedr18@gmail.com>
// SPDX-FileCopyrightText: 2025 FaDeOkno <143940725+FaDeOkno@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using System.Numerics;
using Content.Client.Research;
using Content.Client.UserInterface.Controls;
using Content.Goobstation.Common.Research;
using Content.Goobstation.Shared.Research;
using Content.Shared.Access.Systems;
using Content.Shared.Research.Components;
using Content.Shared.Research.Prototypes;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Input;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Goobstation.Client.Research.UI;

[GenerateTypedNameReferences]
public sealed partial class FancyResearchConsoleMenu : FancyWindow
{
    public Action<string>? OnTechnologyCardPressed;
    public Action? OnServerButtonPressed;

    [Dependency] private readonly IEntityManager _entity = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    private readonly ResearchSystem _research;
    private readonly SpriteSystem _sprite;
    private readonly AccessReaderSystem _accessReader;

    /// <summary>
    /// Console entity
    /// </summary>
    public EntityUid Entity;

    /// <summary>
    /// Currently selected tech
    /// Exsists for better UI refreshing
    /// </summary>
    public ProtoId<TechnologyPrototype>? CurrentTech;

    /// <summary>
    /// All technologies and their availablity
    /// </summary>
    public Dictionary<string, ResearchAvailability> List = new();

    /// <summary>
    /// Cached research points
    /// </summary>
    public int Points = 0;

    /// <summary>
    /// Is tech currently being dragged
    /// </summary>
    private bool _draggin;

    /// <summary>
    /// Global position that all tech relates to.
    /// For dragging mostly
    /// </summary>
    private Vector2 _position = new Vector2(45, 250);

    public FancyResearchConsoleMenu()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);
        _research = _entity.System<ResearchSystem>();
        _sprite = _entity.System<SpriteSystem>();
        _accessReader = _entity.System<AccessReaderSystem>();
        StaticSprite.SetFromSpriteSpecifier(new SpriteSpecifier.Rsi(new("_Goobstation/Interface/rnd-static.rsi"), "static"));

        ServerButton.OnPressed += _ => OnServerButtonPressed?.Invoke();
        DragContainer.OnKeyBindDown += OnKeybindDown;
        DragContainer.OnKeyBindUp += OnKeybindUp;
        RecenterButton.OnPressed += _ => Recenter();

        UpdatePanels(List);
        Recenter();
    }

    public void SetEntity(EntityUid entity)
        => Entity = entity;

    public void UpdatePanels(Dictionary<string, ResearchAvailability> dict)
    {
        DragContainer.RemoveAllChildren();
        List = dict;

        foreach (var tech in List)
        {
            var proto = _prototype.Index<TechnologyPrototype>(tech.Key);

            var control = new FancyResearchConsoleItem(proto, _sprite, tech.Value);
            DragContainer.AddChild(control);

            // Set position for all tech, relating to _position
            LayoutContainer.SetPosition(control, _position + proto.Position * 150);
            control.SelectAction += SelectTech;

            if (tech.Key == CurrentTech)
                SelectTech(proto, tech.Value);
        }
    }

    public void UpdateInformationPanel(int points)
    {
        Points = points;

        var amountMsg = new FormattedMessage();
        amountMsg.AddMarkupOrThrow(Loc.GetString("research-console-menu-research-points-text",
            ("points", points)));
        ResearchAmountLabel.SetMessage(amountMsg);

        if (!_entity.TryGetComponent(Entity, out TechnologyDatabaseComponent? database))
            return;

        TierDisplayContainer.RemoveAllChildren();
        foreach (var disciplineId in database.SupportedDisciplines)
        {
            var discipline = _prototype.Index<TechDisciplinePrototype>(disciplineId);
            var tier = _research.GetTierCompletionPercentage(database, discipline, _prototype);

            // i'm building the small-ass control here to spare me some mild annoyance in making a new file
            var texture = new TextureRect
            {
                TextureScale = new Vector2(2, 2),
                VerticalAlignment = VAlignment.Center
            };
            var label = new RichTextLabel();
            texture.Texture = _sprite.Frame0(discipline.Icon);
            label.SetMessage(Loc.GetString("research-console-tier-percentage", ("perc", tier)));

            var control = new BoxContainer
            {
                Children =
                {
                    texture,
                    label,
                    new Control
                    {
                        MinWidth = 10
                    }
                }
            };
            TierDisplayContainer.AddChild(control);
        }
    }

    #region Drag handle
    protected override void MouseMove(GUIMouseMoveEventArgs args)
    {
        base.MouseMove(args);

        if (!_draggin)
            return;

        _position += args.Relative;

        // Move all tech
        foreach (var child in DragContainer.Children)
        {
            LayoutContainer.SetPosition(child, child.Position + args.Relative);
        }
    }

    /// <summary>
    /// Raised when LMB is pressed at <see cref="DragContainer"/>
    /// </summary>
    private void OnKeybindDown(GUIBoundKeyEventArgs args)
    {
        if (args.Function == EngineKeyFunctions.Use)
            _draggin = true;
    }

    /// <summary>
    /// Raised when LMB is unpressed at <see cref="DragContainer"/>
    /// </summary>
    private void OnKeybindUp(GUIBoundKeyEventArgs args)
    {
        if (args.Function == EngineKeyFunctions.Use)
            _draggin = false;
    }

    protected override DragMode GetDragModeFor(Vector2 relativeMousePos)
        => _draggin ? DragMode.None : base.GetDragModeFor(relativeMousePos);
    #endregion

    /// <summary>
    /// Selects a tech prototype and opens info panel
    /// </summary>
    /// <param name="proto">Tech proto</param>
    /// <param name="availability">Tech availablity</param>
    public void SelectTech(TechnologyPrototype proto, ResearchAvailability availability)
    {
        InfoContainer.RemoveAllChildren();
        if (!_player.LocalEntity.HasValue)
            return;

        CurrentTech = proto.ID;
        var control = new FancyTechnologyInfoPanel(proto, _accessReader.IsAllowed(_player.LocalEntity.Value, Entity), availability, _sprite);
        control.BuyAction += args => OnTechnologyCardPressed?.Invoke(args.ID);
        InfoContainer.AddChild(control);
    }

    /// <summary>
    /// Sets <see cref="_position"/> to its default value
    /// </summary>
    public void Recenter()
    {
        _position = new(45, 250);
        foreach (var item in DragContainer.Children)
        {
            if (item is not FancyResearchConsoleItem research)
                continue;

            LayoutContainer.SetPosition(item, _position + research.Prototype.Position * 150);
        }
    }

    public override void Close()
    {
        base.Close();

        DragContainer.RemoveAllChildren();
        InfoContainer.RemoveAllChildren();
    }

    private sealed partial class DisciplineButton(TechDisciplinePrototype proto) : Button
    {
        public TechDisciplinePrototype Proto = proto;
    }
}