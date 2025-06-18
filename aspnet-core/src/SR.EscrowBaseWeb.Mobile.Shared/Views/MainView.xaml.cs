using Xamarin.Forms;

namespace SR.EscrowBaseWeb.Views
{
    public partial class MainView : MasterDetailPage, IXamarinView
    {
        public MainView()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
