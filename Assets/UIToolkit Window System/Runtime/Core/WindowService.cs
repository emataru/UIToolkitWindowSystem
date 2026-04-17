using System.Threading;
using Cysharp.Threading.Tasks;

namespace UIToolkitWindowSystem
{
    public sealed class WindowService
    {
        private readonly WindowContext _context;

        public WindowService(WindowContext context)
        {
            _context = context;
        }

        public async UniTask<DialogResult> ShowMessageAsync(
            string title,
            string message,
            CancellationToken cancellationToken = default)
        {
            var dialog = new MessageBoxWindow(_context, title, message);
            _context.WindowManager.OpenModal(dialog);
            return await dialog.WaitForResultAsync(cancellationToken);
        }

        public async UniTask<DialogResult> ShowConfirmAsync(
            string title,
            string message,
            CancellationToken cancellationToken = default)
        {
            var dialog = new ConfirmDialogWindow(_context, title, message);
            _context.WindowManager.OpenModal(dialog);
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