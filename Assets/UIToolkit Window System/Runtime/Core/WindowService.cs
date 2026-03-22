using System.Threading;
using Cysharp.Threading.Tasks;

namespace UIToolkitWindowSystem
{
    public sealed class WindowService
    {
        private readonly WindowManager _windowManager;
        private readonly WindowViewAssets _viewAssets;

        public WindowService(WindowManager windowManager, WindowViewAssets viewAssets)
        {
            _windowManager = windowManager;
            _viewAssets = viewAssets;
        }

        public async UniTask<DialogResult> ShowMessageAsync(
            string title,
            string message,
            CancellationToken cancellationToken = default)
        {
            var dialog = new MessageBoxWindow(title, message, _viewAssets.MessageBoxUxml);
            _windowManager.OpenModal(dialog);
            return await dialog.WaitForResultAsync(cancellationToken);
        }

        public async UniTask<DialogResult> ShowConfirmAsync(
            string title,
            string message,
            CancellationToken cancellationToken = default)
        {
            var dialog = new ConfirmDialogWindow(title, message, _viewAssets.ConfirmDialogUxml);
            _windowManager.OpenModal(dialog);
            return await dialog.WaitForResultAsync(cancellationToken);
        }

        public async UniTask<bool> ShowConfirmYesNoAsync(
            string title,
            string message,
            CancellationToken cancellationToken = default)
        {
            var result = await ShowConfirmAsync(title, message, cancellationToken);
            return result == DialogResult.Yes;
        }
    }
}