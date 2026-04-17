using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public abstract class DialogWindow<TResult> : ModalWindowBase, IDialogSubmitHandler
    {
        private UniTaskCompletionSource<TResult> _tcs;
        private bool _completed;

        protected DialogWindow(WindowOptions options, VisualTreeAsset frameUxml)
            : base(options, frameUxml)
        {
        }

        public UniTask<TResult> WaitForResultAsync(CancellationToken cancellationToken = default)
        {
            _tcs ??= new UniTaskCompletionSource<TResult>();

            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(() =>
                {
                    if (_completed)
                        return;

                    _completed = true;
                    _tcs.TrySetCanceled(cancellationToken);

                    if (IsOpen)
                        base.RequestClose();
                });
            }

            return _tcs.Task;
        }

        protected void Complete(TResult result)
        {
            if (_completed)
                return;

            _completed = true;
            _tcs ??= new UniTaskCompletionSource<TResult>();
            _tcs.TrySetResult(result);

            base.RequestClose();
        }

        public bool TrySubmitByKeyboard()
        {
            return TryHandleSubmitKey();
        }

        protected virtual bool TryHandleSubmitKey()
        {
            return false;
        }

        protected abstract TResult GetCloseFallbackResult();

        public override void RequestClose()
        {
            if (!_completed)
            {
                _completed = true;
                _tcs ??= new UniTaskCompletionSource<TResult>();
                _tcs.TrySetResult(GetCloseFallbackResult());
            }

            base.RequestClose();
        }
    }
}