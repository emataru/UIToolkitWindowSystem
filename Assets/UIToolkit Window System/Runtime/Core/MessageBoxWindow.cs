using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class MessageBoxWindow : DialogWindow<DialogResult>
    {
        private readonly string _message;
        private Button _okButton;

        public MessageBoxWindow(WindowContext context, string title, string message)
            : base(context, new WindowOptions
            {
                Title = title,
                Width = 360,
                Height = 180,
                MinWidth = 280,
                MinHeight = 140,
                Closable = true,
                Draggable = true,
                Resizable = true,
                CloseOnEscape = true,
                CenterOnOpen = true
            })
        {
            _message = message;
            BuildDialogContent();
        }

        private void BuildDialogContent()
        {
            var tree = CloneContentTree(Context.CommonViews.MessageBoxContentUxml);
            tree.style.flexGrow = 1;
            tree.style.flexShrink = 1;
            tree.style.minHeight = 0;

            ContentRoot.Add(tree);

            var messageLabel = ContentRoot.Q<Label>("message-label");
            _okButton = ContentRoot.Q<Button>("ok-button");

            if (messageLabel != null)
                messageLabel.text = _message;

            if (_okButton != null)
                _okButton.clicked += () => Complete(DialogResult.OK);
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