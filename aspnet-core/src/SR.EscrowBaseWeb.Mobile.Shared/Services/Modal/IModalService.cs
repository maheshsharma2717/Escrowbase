using System.Threading.Tasks;
using SR.EscrowBaseWeb.Views;
using Xamarin.Forms;

namespace SR.EscrowBaseWeb.Services.Modal
{
    public interface IModalService
    {
        Task ShowModalAsync(Page page);

        Task ShowModalAsync<TView>(object navigationParameter) where TView : IXamarinView;

        Task<Page> CloseModalAsync();
    }
}
