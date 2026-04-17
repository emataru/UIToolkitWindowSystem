using UnityEngine;
using Cysharp.Threading.Tasks;

namespace UIToolkitWindowSystem
{
    public sealed class DialogController : WindowFeatureControllerBase
    {
        public void ShowMessage(string title, string message)
        {
            ShowMessageAsync(title, message).Forget();
        }

        public void ShowConfirm(string title, string message)
        {
            ShowConfirmAsync(title, message).Forget();
        }

        private async UniTaskVoid ShowMessageAsync(string title, string message)
        {
            if (Context == null)
                return;

            await Context.WindowService.ShowMessageAsync(title, message);
        }

        private async UniTaskVoid ShowConfirmAsync(string title, string message)
        {
            if (Context == null)
                return;

            var result = await Context.WindowService.ShowConfirmAsync(title, message);
            Debug.Log($"DialogController confirm result = {result}");
        }
    }
}