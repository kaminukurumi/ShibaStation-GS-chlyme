// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Misandry <mary@thughunt.ing>
// SPDX-FileCopyrightText: 2025 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using System.Numerics;
using Content.Client.Administration.Managers;
using Content.Shared._durkcode.ServerCurrency;
using Robust.Client.AutoGenerated;
using Robust.Client.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;
using Timer = Robust.Shared.Timing.Timer;

namespace Content.Client._durkcode.ServerCurrency.UI
{
    [GenerateTypedNameReferences]
    public sealed partial class CurrencyWindow : DefaultWindow
    {
        [Dependency] private readonly ServerCurrencySystem _serverCur = default!;
        [Dependency] private readonly IClientAdminManager _adminManager = default!;
        [Dependency] private readonly IClientConsoleHost _consoleHost = default!;
        [Dependency] private readonly IPrototypeManager _protoManager = default!;
        public event Action<ProtoId<TokenListingPrototype>>? OnBuy;
        private bool isAdmin = false;
        private Dictionary<Button, (DateTime LastClick, TokenListingPrototype Listing)> _buttonClickTimes = new();
        private const double DoubleClickTimeWindow = 1.5; // seconds

        public CurrencyWindow()
        {
            RobustXamlLoader.Load(this);
            IoCManager.InjectDependencies(this);

            UpdatePlayerBalance();

            isAdmin = _adminManager.CanCommand("balance:add");

            if (!isAdmin)
                Admin.Visible = false;

            GiftButton.OnPressed += _ => Transfer(GiftPlayer.Text, int.Parse((string)GiftAmmount.Text));

            AdminAddButton.OnPressed += _ => AdminAdd(AdminAddPlayer.Text, int.Parse((string)AdminAddAmmount.Text));

            _serverCur.BalanceChange += UpdatePlayerBalance;

            PopulateTokenButtons();
            UpdateButtonStates();
        }

        private void PopulateTokenButtons()
        {
            TokenListingsContainer.DisposeAllChildren();
            _buttonClickTimes.Clear();

            var tokenListings = _protoManager.EnumeratePrototypes<TokenListingPrototype>()
                .OrderByDescending(x => x.Price);

            foreach (var listing in tokenListings)
            {
                var button = new Button
                {
                    Text = Loc.GetString(listing.Name, ("price", listing.Price)),
                    MinHeight = 40,
                    ToolTip = Loc.GetString(listing.Description)
                };

                button.OnPressed += _ =>
                {
                    if (_buttonClickTimes.TryGetValue(button, out var clickData))
                    {
                        var timeSinceLastClick = (DateTime.Now - clickData.LastClick).TotalSeconds;
                        if (timeSinceLastClick <= DoubleClickTimeWindow)
                        {
                            // Double click occurred
                            OnBuy?.Invoke(listing.ID);
                            ShowConfirmation(Loc.GetString(listing.Label));
                            _buttonClickTimes.Remove(button);
                            button.Text = Loc.GetString(listing.Name, ("price", listing.Price));
                            return;
                        }
                    }

                    // First click
                    _buttonClickTimes[button] = (DateTime.Now, listing);
                    button.Text = Loc.GetString("gs-balanceui-shop-click-confirm");

                    // Reset button text after delay
                    Timer.Spawn(TimeSpan.FromSeconds(DoubleClickTimeWindow), () =>
                    {
                        if (_buttonClickTimes.ContainsKey(button))
                        {
                            button.Text = Loc.GetString(listing.Name, ("price", listing.Price));
                            _buttonClickTimes.Remove(button);
                            UpdateButtonStates();
                        }
                    });
                };

                TokenListingsContainer.AddChild(button);
                TokenListingsContainer.AddChild(new Control { MinSize = new Vector2(0, 5) });
            }
        }

        private void Transfer(string player, int value)
        {
            if (player == null || value == 0)
                return;

            _consoleHost.ExecuteCommand("gift " + player + " " + value);

            UpdatePlayerBalance();
        }

        private void AdminAdd(string player, int value)
        {
            if (!isAdmin || player == null || value == 0)
                return;

            _consoleHost.ExecuteCommand("balance:add " + player + " " + value);

            UpdatePlayerBalance();
        }

        private void UpdatePlayerBalance() // Goobstation - Goob Coin
        {
            var balance = _serverCur.GetBalance();
            Header.Text = _serverCur.Stringify(balance);
            UpdateButtonStates(balance);
        }

        private void UpdateButtonStates(int? balance = null)
        {
            if (balance == null)
                balance = _serverCur.GetBalance();

            Header.Text = _serverCur.Stringify(balance.Value);
            foreach (var child in TokenListingsContainer.Children)
            {
                if (child is not Button button)
                    continue;

                var listing = _protoManager.EnumeratePrototypes<TokenListingPrototype>()
                    .FirstOrDefault(x => Loc.GetString(x.Name, ("price", x.Price)) == button.Text);


                if (listing != null)
                    button.Disabled = balance < listing.Price;
            }
        }

        private void ShowConfirmation(string message)
        {
            ConfirmationMessage.Text = Loc.GetString("gs-balanceui-shop-purchased", ("item", message));
            ConfirmationMessage.Visible = true;
            Timer.Spawn(TimeSpan.FromSeconds(3), () =>
            {
                ConfirmationMessage.Visible = false;
                UpdateButtonStates();
            });
        }
    }
}