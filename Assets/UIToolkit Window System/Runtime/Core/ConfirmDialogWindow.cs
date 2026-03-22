using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class ConfirmDialogWindow : DialogWindow<DialogResult>
    {
        private readonly string _message;
        private readonly VisualTreeAsset _uxml;

        private Button _yesButton;
        private Button _noButton;

        public ConfirmDialogWindow(string title, string message, VisualTreeAsset uxml)
            : base(new WindowOptions
            {
                Title = title,
                Width = 380,
                Height = 190,
                MinWidth = 300,
                MinHeight = 150,
                Closable = true,
                Draggable = true,
                Resizable = false,
                CloseOnEscape = true,
                CenterOnOpen = true
            })
        {
            _message = message;
            _uxml = uxml;

            BuildDialogContent();
        }

        private void BuildDialogContent()
        {
            var tree = CloneContentTree(_uxml);
            ContentRoot.Add(tree);

            var messageLabel = ContentRoot.Q<Label>("message-label");
            _yesButton = ContentRoot.Q<Button>("yes-button");
            _noButton = ContentRoot.Q<Button>("no-button");

            if (messageLabel != null)
                messageLabel.text = _message;

            if (_yesButton != null)
            {
                _yesButton.style.minWidth = 70;
                _yesButton.clicked += () => Complete(DialogResult.Yes);
            }

            if (_noButton != null)
            {
                _noButton.style.minWidth = 70;
                _noButton.clicked += () => Complete(DialogResult.No);
            }
        }

        protected override bool TryHandleSubmitKey()
        {
            Complete(DialogResult.Yes);
            return true;
        }

        protected override DialogResult GetCloseFallbackResult()
        {
            return DialogResult.Cancel;
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            _yesButton?.Focus();
        }
    }
}