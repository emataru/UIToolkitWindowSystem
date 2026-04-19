using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class SampleToolWindow : WindowBase
    {
        private readonly WindowContext _context;
        private readonly VisualTreeAsset _contentUxml;

        private TextField _nameField;
        private Toggle _dangerToggle;
        private Button _messageButton;
        private Button _confirmButton;

        public SampleToolWindow(
            WindowContext context,
            VisualTreeAsset contentUxml,
            params StyleSheet[] styleSheets)
            : base(new WindowOptions
            {
                Title = "Sample Tool Window",
                Width = 460,
                Height = 700,
                MinWidth = 320,
                MinHeight = 220,
                Closable = true,
                Draggable = true,
                Resizable = true,
                CloseOnEscape = false,
                CenterOnOpen = true
            }, context.CommonViews.WindowFrameUxml)
        {
            _context = context;
            _contentUxml = contentUxml;

            AddContentStyleSheets(styleSheets);
            BuildWindowContent();
        }

        private void BuildWindowContent()
        {
            var tree = CloneContentTree(_contentUxml);
            ContentRoot.Add(tree);

            _nameField = ContentRoot.Q<TextField>("name-field");
            _dangerToggle = ContentRoot.Q<Toggle>("danger-toggle");
            _messageButton = ContentRoot.Q<Button>("message-button");
            _confirmButton = ContentRoot.Q<Button>("confirm-button");

            if (_messageButton != null)
                _messageButton.clicked += () => ShowMessageAsync().Forget();

            if (_confirmButton != null)
                _confirmButton.clicked += () => ShowConfirmAsync().Forget();
        }

        private async UniTaskVoid ShowMessageAsync()
        {
            string name = _nameField?.value ?? "Player";

            await _context.WindowService.ShowMessageAsync(
                "Hello",
                $"こんにちは {name} さん。\nこれは通常ウィンドウから開いた MessageBox です。");
        }

        private async UniTaskVoid ShowConfirmAsync()
        {
            string name = _nameField?.value ?? "Player";
            bool enabled = _dangerToggle?.value ?? false;

            if (!enabled)
            {
                await _context.WindowService.ShowMessageAsync(
                    "Info",
                    "Enable Dangerous Action が OFF なので確認をスキップしました。");
                return;
            }

            var result = await _context.WindowService.ShowConfirmAsync(
                "Confirm",
                $"{name} さん、危険な処理を実行しますか？");

            await _context.WindowService.ShowMessageAsync("Result", $"選択結果: {result}");
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            _nameField?.Focus();
        }
    }
}