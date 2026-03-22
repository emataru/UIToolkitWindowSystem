using UnityEngine;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;
using UIToolkitWindowSystem;

public sealed class WindowSystemMinimalLauncher : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private StyleSheet _styleSheet;

    private WindowManager _windowManager;
    private WindowService _windowService;

    private async void Start()
    {
        if (_uiDocument == null)
        {
            Debug.LogError("UIDocument is null");
            return;
        }

        var root = _uiDocument.rootVisualElement;

        if (_styleSheet != null)
            root.styleSheets.Add(_styleSheet);

        var layers = new WindowLayerRoot(root);
        _windowManager = new WindowManager(layers);
        _windowService = new WindowService(_windowManager);

        var windowA = new WindowBase(new WindowOptions
        {
            Title = "Window A",
            Width = 420,
            Height = 260,
            Closable = true,
            Draggable = true,
            Resizable = true,
            CloseOnEscape = true,
            CenterOnOpen = true
        });

        _windowManager.Open(windowA);

        await UniTask.Yield();

        await _windowService.ShowMessageAsync(
            "MessageBox",
            "Enter で OK、ESC で Cancel です。");

        var result = await _windowService.ShowConfirmAsync(
            "Confirm",
            "Enter で Yes、ESC で Cancel です。");

        Debug.Log($"Confirm result = {result}");
    }
}