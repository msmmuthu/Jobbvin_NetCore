namespace Jobbvin.Client.Services
{
    public class LoaderService
    {
        public event Action OnShow;
        public event Action OnHide;

        public void Show()

        {
            OnShow?.Invoke();
        }

        public void Hide()

        {
            OnHide?.Invoke();
        }
    }
}
