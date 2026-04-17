using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class SampleToolWindowController : WindowFeatureControllerBase
    {
        [SerializeField] private bool openOnStart = false;
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

            if (Context == null)
            {
                Debug.LogError("SampleToolWindowController: Window context is not ready.");
                return;
            }

            if (_window != null && _window.IsOpen)
            {
                Context.WindowManager.Focus(_window);
                return;
            }

            _window = new SampleToolWindow(
                Context,
                sampleToolContentUxml,
                sampleToolStyleSheets);

            Context.WindowManager.Open(_window);
        }

        public void FocusSampleWindow()
        {
            if (_window != null && _window.IsOpen)
            {
                Context.WindowManager.Focus(_window);
            }
        }

        public void CloseSampleWindow()
        {
            if (_window != null && _window.IsOpen)
            {
                _window.RequestClose();
            }
        }
    }
}