using UnityEngine;

namespace UIToolkitWindowSystem
{
    public sealed class SampleToolWindowController : WindowFeatureControllerBase
    {
        [SerializeField] private SampleToolWindowViewAsset sampleToolWindowView;
        [SerializeField] private bool openOnStart = true;

        private SampleToolWindow _window;

        private void Start()
        {
            if (!openOnStart)
                return;

            OpenSampleWindow();
        }

        public void OpenSampleWindow()
        {
            if (sampleToolWindowView == null)
            {
                Debug.LogError("SampleToolWindowController: SampleToolWindowViewAsset is not assigned.");
                return;
            }

            if (WindowManager == null || WindowService == null || CommonViews == null)
            {
                Debug.LogError("SampleToolWindowController: Window system is not ready.");
                return;
            }

            if (_window != null && _window.IsOpen)
            {
                WindowManager.Focus(_window);
                return;
            }

            _window = new SampleToolWindow(
                WindowService,
                CommonViews.WindowFrameUxml,
                sampleToolWindowView);

            WindowManager.Open(_window);
        }

        public void CloseSampleWindow()
        {
            if (_window != null && _window.IsOpen)
            {
                _window.RequestClose();
            }
        }

        public void FocusSampleWindow()
        {
            if (_window != null && _window.IsOpen)
            {
                WindowManager.Focus(_window);
            }
        }
    }
}