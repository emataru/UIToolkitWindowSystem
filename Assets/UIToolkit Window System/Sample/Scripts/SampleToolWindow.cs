using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class SampleToolWindow : WindowBase
    {
        private readonly WindowService _windowService;
        private readonly VisualTreeAsset _contentUxml;

        private TextField _nameField;
        private Toggle _dangerToggle;
        private Button _messageButton;
        private Button _confirmButton;

        public SampleToolWindow(
            WindowService windowService,
            VisualTreeAsset frameUxml,
            SampleToolWindowViewAsset viewAsset)
            : base(new WindowOptions
            {
                Title = "Sample Tool Window",
                Width = 460,
                Height = 320,
                MinWidth = 320,
                MinHeight = 220,
                Closable = true,
                Draggable = true,
                Resizable = true,
                CloseOnEscape = false,
                CenterOnOpen = true
            }, frameUxml)
        {
            _windowService = windowService;
            _contentUxml = viewAsset.ContentUxml;

            AddContentStyleSheets(viewAsset.StyleSheets);
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

            await _windowService.ShowMessageAsync(
                "Hello",
                $"こんにちは {name} さん。\nこれは通常ウィンドウから開いた MessageBox です。");
        }

        private async UniTaskVoid ShowConfirmAsync()
        {
            string name = _nameField?.value ?? "Player";
            bool enabled = _dangerToggle?.value ?? false;

            if (!enabled)
            {
                await _windowService.ShowMessageAsync(
                    "Info",
                    "Enable Dangerous Action が OFF なので確認をスキップしました。");
                return;
            }

            var result = await _windowService.ShowConfirmAsync(
                "Confirm",
                $"{name} さん、危険な処理を実行しますか？");

            await _windowService.ShowMessageAsync("Result", $"選択結果: {result}");
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            _nameField?.Focus();
        }
    }
}