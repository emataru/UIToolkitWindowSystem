using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class MessageBoxWindow : DialogWindow<DialogResult>
    {
        private readonly string _message;
        private readonly VisualTreeAsset _contentUxml;

        private Button _okButton;

        public MessageBoxWindow(
            string title,
            string message,
            VisualTreeAsset frameUxml,
            VisualTreeAsset contentUxml)
            : base(new WindowOptions
            {
                Title = title,
                Width = 360,
                Height = 180,
                MinWidth = 280,
                MinHeight = 140,
                Closable = true,
                Draggable = true,
                Resizable = false,
                CloseOnEscape = true,
                CenterOnOpen = true
            }, frameUxml)
        {
            _message = message;
            _contentUxml = contentUxml;

            BuildDialogContent();
        }

        private void BuildDialogContent()
        {
            var tree = CloneContentTree(_contentUxml);
            ContentRoot.Add(tree);

            var messageLabel = ContentRoot.Q<Label>("message-label");
            _okButton = ContentRoot.Q<Button>("ok-button");

            if (messageLabel != null)
                messageLabel.text = _message;

            if (_okButton != null)
            {
                _okButton.clicked += () => Complete(DialogResult.OK);
            }
        }

        protected override bool TryHandleSubmitKey()
        {
            Complete(DialogResult.OK);
            return true;
        }

        protected override DialogResult GetCloseFallbackResult()
        {
            return DialogResult.Cancel;
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            _okButton?.Focus();
        }
    }
}