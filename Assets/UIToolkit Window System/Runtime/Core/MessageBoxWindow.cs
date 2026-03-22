using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class MessageBoxWindow : DialogWindow<DialogResult>
    {
        private readonly string _message;
        private Button _okButton;

        public MessageBoxWindow(string title, string message)
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
            })
        {
            _message = message;
            BuildDialogContent();
        }

        private void BuildDialogContent()
        {
            ContentRoot.style.flexDirection = FlexDirection.Column;
            ContentRoot.style.paddingLeft = 12;
            ContentRoot.style.paddingRight = 12;
            ContentRoot.style.paddingTop = 12;
            ContentRoot.style.paddingBottom = 12;

            var messageLabel = new Label(_message);
            messageLabel.style.whiteSpace = WhiteSpace.Normal;
            messageLabel.style.flexGrow = 1;
            ContentRoot.Add(messageLabel);

            var buttonRow = new VisualElement();
            buttonRow.style.flexDirection = FlexDirection.Row;
            buttonRow.style.justifyContent = Justify.FlexEnd;
            buttonRow.style.alignItems = Align.Center;
            buttonRow.style.marginTop = 12;

            _okButton = new Button(() => Complete(DialogResult.OK))
            {
                text = "OK"
            };
            _okButton.style.minWidth = 70;

            buttonRow.Add(_okButton);
            ContentRoot.Add(buttonRow);
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