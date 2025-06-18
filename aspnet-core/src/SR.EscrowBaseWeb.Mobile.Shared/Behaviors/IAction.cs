using Xamarin.Forms.Internals;

namespace SR.EscrowBaseWeb.Behaviors
{
    [Preserve(AllMembers = true)]
    public interface IAction
    {
        bool Execute(object sender, object parameter);
    }
}