// SPDX-FileCopyrightText: 2021 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 Emisse <99158783+Emisse@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 ElectroJr <leonsfriedrich@gmail.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Chemistry.Reagent;
using Robust.Client.AutoGenerated;
using Robust.Client.Console;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;

namespace Content.Client.Administration.UI.ManageSolutions
{
    /// <summary>
    ///     A debug window that allows you to add a reagent to a solution. Intended to be used with <see
    ///     cref="EditSolutionsWindow"/>
    /// </summary>
    [GenerateTypedNameReferences]
    public sealed partial class AddReagentWindow : DefaultWindow
    {
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly IClientConsoleHost _consoleHost = default!;

        private readonly NetEntity _targetEntity;
        private string _targetSolution;
        private ReagentPrototype? _selectedReagent;

        // FloatSpinBox does not (yet?) play nice with xaml
        private FloatSpinBox _quantitySpin = new(1, 2) { Value = 10, HorizontalExpand = true};

        public AddReagentWindow(NetEntity targetEntity, string targetSolution)
        {
            IoCManager.InjectDependencies(this);
            RobustXamlLoader.Load(this);

            Title = Loc.GetString("admin-add-reagent-window-title", ("solution", targetSolution));

            _targetEntity = targetEntity;
            _targetSolution = targetSolution;

            QuantityBox.AddChild(_quantitySpin);

            ReagentList.OnItemSelected += ReagentListSelected;
            ReagentList.OnItemDeselected += ReagentListDeselected;
            SearchBar.OnTextChanged += (_) => UpdateReagentPrototypes(SearchBar.Text);
            _quantitySpin.OnValueChanged += (_) => UpdateAddButton();
            AddButton.OnPressed += AddReagent;

            UpdateReagentPrototypes();
            UpdateAddButton();
        }

        /// <summary>
        ///     Execute a console command that asks the server to add the selected reagent.
        /// </summary>
        private void AddReagent(BaseButton.ButtonEventArgs obj)
        {
            if (_selectedReagent == null)
                return;

            var quantity = _quantitySpin.Value.ToString("F2");
            var command = $"addreagent {_targetEntity} {_targetSolution} {_selectedReagent.ID} {quantity}";
            _consoleHost.ExecuteCommand(command);
        }

        private void ReagentListSelected(ItemList.ItemListSelectedEventArgs obj)
        {
            _selectedReagent = (ReagentPrototype) obj.ItemList[obj.ItemIndex].Metadata!;
            UpdateAddButton();
        }

        private void ReagentListDeselected(ItemList.ItemListDeselectedEventArgs obj)
        {
            _selectedReagent = null;
            UpdateAddButton();
        }

        public void UpdateSolution(string? selectedSolution)
        {
            if (selectedSolution == null)
            {
                Close();
                Dispose();
                return;
            }

            _targetSolution = selectedSolution;
            Title = Loc.GetString("admin-add-reagent-window-title", ("solution", _targetSolution));
            UpdateAddButton();
        }

        /// <summary>
        ///     Set the Text and enabled/disabled status of the button that actually adds the reagent.
        /// </summary>
        private void UpdateAddButton()
        {
            AddButton.Disabled = true;
            if (_selectedReagent == null)
            {
                AddButton.Text = Loc.GetString("admin-add-reagent-window-add-invalid-reagent");
                return;
            }

            AddButton.Text = Loc.GetString("admin-add-reagent-window-add",
                ("quantity", _quantitySpin.Value.ToString("F2")),
                ("reagent", _selectedReagent.ID));

            AddButton.Disabled = false;
        }

        /// <summary>
        ///     Get a list of all reagent prototypes and show them in an item list.
        /// </summary>
        private void UpdateReagentPrototypes(string? filter = null)
        {
            ReagentList.Clear();
            foreach (var reagent in _prototypeManager.EnumeratePrototypes<ReagentPrototype>())
            {
                if (!string.IsNullOrEmpty(filter) &&
                   !reagent.ID.ToLowerInvariant().Contains(filter.Trim().ToLowerInvariant()))
                {
                    continue;
                }

                ItemList.Item regentItem = new(ReagentList)
                {
                    Metadata = reagent,
                    Text = reagent.ID
                };

                ReagentList.Add(regentItem);
            }
        }
    }
}