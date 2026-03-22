using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class ConfirmDialogWindow : DialogWindow<DialogResult>
    {
        private readonly string _message;
        private Button _yesButton;
        private Button _noButton;

        public ConfirmDialogWindow(string title, string message)
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

            _yesButton = new Button(() => Complete(DialogResult.Yes))
            {
                text = "Yes"
            };
            _yesButton.style.minWidth = 70;

            _noButton = new Button(() => Complete(DialogResult.No))
            {
                text = "No"
            };
            _noButton.style.minWidth = 70;

            buttonRow.Add(_yesButton);
            buttonRow.Add(_noButton);
            ContentRoot.Add(buttonRow);
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