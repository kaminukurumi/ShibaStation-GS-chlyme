// SPDX-FileCopyrightText: 2025 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 JohnOakman <sremy2012@hotmail.fr>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.UserInterface.Controls;
using Content.Shared.Administration;
using Content.Shared._Shitmed.Autodoc;
using Content.Shared._Shitmed.Autodoc.Components;
using Content.Shared._Shitmed.Autodoc.Systems;
using Robust.Client.AutoGenerated;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Timing;
using Robust.Client.UserInterface;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Utility;
using System.IO;
using YamlDotNet.RepresentationModel;
using Robust.Shared.Serialization.Markdown;

namespace Content.Client._Shitmed.Autodoc;

[GenerateTypedNameReferences]
public sealed partial class AutodocWindow : FancyWindow
{
    [Dependency] private readonly IEntityManager _entMan = default!;
    [Dependency] private readonly IFileDialogManager _dialogMan = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly ISerializationManager _serMan = default!;
    [Dependency] private readonly ILogManager _logMan = default!;
    private SharedAutodocSystem _autodoc;

    private EntityUid _owner;
    private bool _active;
    private int _programCount = 0;
    private ISawmill _sawmill;

    public event Action<string>? OnCreateProgram;
    public event Action<int>? OnToggleProgramSafety;
    public event Action<int>? OnRemoveProgram;
    public event Action<int, IAutodocStep, int>? OnAddStep;
    public event Action<int, int>? OnRemoveStep;
    public event Action<int>? OnStart;
    public event Action? OnStop;
    public event Action<AutodocProgram>? OnImportProgram;

    private DialogWindow? _dialog;
    private AutodocProgramWindow? _currentProgram;

    public AutodocWindow(EntityUid owner, IEntityManager entMan, IPlayerManager player)
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        _entMan = entMan;
        _player = player;
        _autodoc = entMan.System<SharedAutodocSystem>();
        _sawmill = _logMan.GetSawmill("autodoc-ui");

        _owner = owner;

        OnClose += () =>
        {
            _dialog?.Close();
            _currentProgram?.Close();
        };

        ImportProgramButton.OnPressed += _ =>
        {
            ImportProgram();
        };

        CreateProgramButton.OnPressed += _ =>
        {
            if (_dialog != null)
            {
                _dialog.MoveToFront();
                return;
            }

            if (!_entMan.TryGetComponent<AutodocComponent>(_owner, out var comp))
                return;

            var field = "title";
            var prompt = Loc.GetString("autodoc-program-title");
            var placeholder = Loc.GetString("autodoc-program-title-placeholder", ("number", comp.Programs.Count + 1));
            var entry = new QuickDialogEntry(field, QuickDialogEntryType.ShortText, prompt, placeholder);
            var entries = new List<QuickDialogEntry> { entry };
            _dialog = new DialogWindow(CreateProgramButton.Text!, entries);
            _dialog.OnConfirmed += responses =>
            {
                var title = responses[field].Trim();
                if (title.Length < 1 || title.Length > comp.MaxProgramTitleLength)
                    return;

                OnCreateProgram?.Invoke(title);
            };

            // prevent MoveToFront being called on a closed window and double closing
            _dialog.OnClose += () => _dialog = null;
        };

        AbortButton.AddStyleClass("Caution");
        AbortButton.OnPressed += _ => OnStop?.Invoke();

        UpdateActive();
        UpdatePrograms();
    }

    public void UpdateActive()
    {
        if (!_entMan.TryGetComponent<AutodocComponent>(_owner, out var comp))
            return;

        // UI must be in the inactive state by default, since this wont run when inactive at startup
        var active = _entMan.HasComponent<ActiveAutodocComponent>(_owner);
        if (active == _active)
            return;

        _active = active;

        CreateProgramButton.Disabled = active || _programCount >= comp.MaxPrograms;
        AbortButton.Disabled = !active;
        foreach (var button in Programs.Children)
        {
            ((Button) button).Disabled = active;
        }

        if (!active)
            return;

        // close windows that can only be open when inactive
        _dialog?.Close();
        _currentProgram?.Close();
    }

    private void UpdatePrograms()
    {
        if (!_entMan.TryGetComponent<AutodocComponent>(_owner, out var comp))
            return;

        var count = comp.Programs.Count;
        if (count == _programCount)
            return;

        _programCount = count;

        CreateProgramButton.Disabled = _active || _programCount >= comp.MaxPrograms;

        Programs.RemoveAllChildren();
        for (int i = 0; i < comp.Programs.Count; i++)
        {
            var button = new Button()
            {
                Text = comp.Programs[i].Title
            };
            var index = i;
            button.OnPressed += _ => OpenProgram(index);
            button.Disabled = _active;
            Programs.AddChild(button);
        }
    }

    private async void ImportProgram()
    {
        if (await _dialogMan.OpenFile(new FileDialogFilters(new FileDialogFilters.Group("yml"))) is not {} file)
            return;

        try
        {
            using var reader = new StreamReader(file, EncodingHelpers.UTF8);
            var yamlStream = new YamlStream();
            yamlStream.Load(reader);
            var root = yamlStream.Documents[0].RootNode;
            var program = _serMan.Read<AutodocProgram>(root.ToDataNode(), notNullableOverride: true);
            OnImportProgram?.Invoke(program);
        }
        catch (Exception e)
        {
            _sawmill.Error($"Error when importing program: {e}");
        }

    }

    private void OpenProgram(int index)
    {
        if (!_entMan.TryGetComponent<AutodocComponent>(_owner, out var comp))
            return;

        // no editing multiple programs at once
        if (_currentProgram is {} existing)
            existing.Close();

        var window = new AutodocProgramWindow(_owner, comp.Programs[index]);
        window.OnToggleSafety += () => OnToggleProgramSafety?.Invoke(index);
        window.OnRemoveProgram += () =>
        {
            OnRemoveProgram?.Invoke(index);
            Programs.RemoveChild(index);
        };
        window.OnAddStep += (step, stepIndex) => OnAddStep?.Invoke(index, step, stepIndex);
        window.OnRemoveStep += step => OnRemoveStep?.Invoke(index, step);
        window.OnStart += () =>
        {
            if (_active)
                return;

            OnStart?.Invoke(index);

            // predict it starting the program
            _entMan.EnsureComponent<ActiveAutodocComponent>(_owner);
        };
        window.OnClose += () => _currentProgram = null;
        _currentProgram = window;

        window.OpenCentered();
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);

        UpdateActive();
        UpdatePrograms();
    }
}