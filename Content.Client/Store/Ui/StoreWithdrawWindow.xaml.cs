// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 TGRCDev <tgrc@tgrc.dev>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Content.Shared.FixedPoint;
using Content.Shared.Store;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Prototypes;

namespace Content.Client.Store.Ui;

/// <summary>
///     Window to select amount TC to withdraw from Uplink account
///     Used as sub-window in Uplink UI
/// </summary>
[GenerateTypedNameReferences]
public sealed partial class StoreWithdrawWindow : DefaultWindow
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    private Dictionary<CurrencyPrototype, FixedPoint2> _validCurrencies = new();
    private HashSet<CurrencyWithdrawButton> _buttons = new();
    public event Action<BaseButton.ButtonEventArgs, string, int>? OnWithdrawAttempt;

    public StoreWithdrawWindow()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);
    }

    public void CreateCurrencyButtons(Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> balance)
    {
        _validCurrencies.Clear();
        foreach (var currency in balance)
        {
            if (!_prototypeManager.TryIndex(currency.Key, out var proto))
                continue;

            _validCurrencies.Add(proto, currency.Value);
        }

        //this shouldn't ever happen but w/e
        if (_validCurrencies.Count < 1)
            return;

        ButtonContainer.Children.Clear();
        _buttons.Clear();
        foreach (var currency in _validCurrencies)
        {
            if (!currency.Key.CanWithdraw)
                continue;

            var button = new CurrencyWithdrawButton()
            {
                Id = currency.Key.ID,
                Amount = currency.Value,
                MinHeight = 20,
                Text = Loc.GetString("store-withdraw-button-ui", ("currency",Loc.GetString(currency.Key.DisplayName, ("amount", currency.Value)))),
                Disabled = false,
            };
            button.OnPressed += args =>
            {
                OnWithdrawAttempt?.Invoke(args, button.Id, WithdrawSlider.Value);
                Close();
            };

            _buttons.Add(button);
            ButtonContainer.AddChild(button);
        }

        var maxWithdrawAmount = _validCurrencies.Values.Max().Int();

        // setup withdraw slider
        WithdrawSlider.MinValue = 1;
        WithdrawSlider.MaxValue = maxWithdrawAmount;

        WithdrawSlider.OnValueChanged += OnValueChanged;
        OnValueChanged(WithdrawSlider.Value);
    }

    public void OnValueChanged(int i)
    {
        foreach (var button in _buttons)
        {
            button.Disabled = button.Amount < WithdrawSlider.Value;
        }
    }

    private sealed class CurrencyWithdrawButton : Button
    {
        public string? Id;
        public FixedPoint2 Amount = FixedPoint2.Zero;
    }
}