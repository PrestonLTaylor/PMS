using PMS.Client.Views;

namespace PMS.Client;

public sealed partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(ProductLookupById), typeof(ProductLookupById));
    }
}
