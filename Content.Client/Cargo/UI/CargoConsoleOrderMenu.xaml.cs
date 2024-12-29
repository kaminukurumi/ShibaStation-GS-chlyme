using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Cargo.UI
{
    [GenerateTypedNameReferences]
    sealed partial class CargoConsoleOrderMenu : DefaultWindow
    {
        public CargoConsoleOrderMenu()
        {
            RobustXamlLoader.Load(this);
            IoCManager.InjectDependencies(this);

            Amount.SetButtons(new List<int> { -3, -2, -1 }, new List<int> { 1, 2, 3 });
            Amount.IsValid = n => n > 0;
        }
    }
}
