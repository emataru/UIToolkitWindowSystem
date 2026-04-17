using UnityEngine;
using Cysharp.Threading.Tasks;

namespace UIToolkitWindowSystem
{
    public sealed class DialogDemoController : WindowFeatureControllerBase
    {
        [SerializeField] private bool showReadyDialogOnStart = false;

        private async void Start()
        {
            if (!showReadyDialogOnStart)
                return;

            if (WindowService == null)
            {
                Debug.LogError("DialogDemoController: WindowService is not ready.");
                return;
            }

            await UniTask.Yield();

            await WindowService.ShowMessageAsync(
                "Ready",
                "WindowSystemHost 経由でダイアログを表示しています。");
        }

        [ContextMenu("Show Message Dialog")]
        public void ShowMessageDialog()
        {
            ShowMessageDialogAsync().Forget();
        }

        [ContextMenu("Show Confirm Dialog")]
        public void ShowConfirmDialog()
        {
            ShowConfirmDialogAsync().Forget();
        }

        private async UniTaskVoid ShowMessageDialogAsync()
        {
            if (WindowService == null)
                return;

            await WindowService.ShowMessageAsync(
                "Message",
                "これは DialogDemoController から開いた MessageBox です。");
        }

        private async UniTaskVoid ShowConfirmDialogAsync()
        {
            if (WindowService == null)
                return;

            var result = await WindowService.ShowConfirmAsync(
                "Confirm",
                "これは DialogDemoController から開いた ConfirmDialog です。");

            Debug.Log($"DialogDemoController confirm result = {result}");
        }
    }
}