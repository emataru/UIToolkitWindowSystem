using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class SampleToolWindowController : WindowFeatureControllerBase
    {
        [SerializeField] private bool openOnStart = true;
        [SerializeField] private VisualTreeAsset sampleToolContentUxml;
        [SerializeField] private StyleSheet[] sampleToolStyleSheets;

        private SampleToolWindow _window;

        private void Start()
        {
            if (!openOnStart)
                return;

            OpenSampleWindow();
        }

        public void OpenSampleWindow()
        {
            if (sampleToolContentUxml == null)
            {
                Debug.LogError("SampleToolWindowController: content UXML is not assigned.");
                return;
            }

            if (WindowManager == null || WindowService == null || CommonViews == null)
            {
                Debug.LogError("SampleToolWindowController: Window system is not ready.");
                return;
            }

            if (CommonViews.WindowFrameUxml == null)
            {
                Debug.LogError("SampleToolWindowController: Common WindowFrameUxml is not assigned.");
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
                sampleToolContentUxml,
                sampleToolStyleSheets);

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