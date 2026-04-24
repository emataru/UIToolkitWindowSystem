namespace UIToolkitWindowSystem
{
    public sealed class WindowContext
    {
        public WindowManager WindowManager { get; }
        public WindowService WindowService { get; internal set; }
        public CommonWindowViewAssets CommonViews { get; }

        public WindowContext(
            WindowManager windowManager,
            CommonWindowViewAssets commonViews)
        {
            WindowManager = windowManager;
            CommonViews = commonViews;
        }
    }
}