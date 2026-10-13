namespace PCControl.Service.Navigation
{
    public interface IAppNavigationService
    {
        event Action<object>? Navigated;

        void Navigate(string pageName);
    }
}
