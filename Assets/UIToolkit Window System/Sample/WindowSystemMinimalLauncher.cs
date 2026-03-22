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
        Debug.Log("WindowSystemMinimalLauncher.Start");

        if (_uiDocument == null)
        {
            Debug.LogError("UIDocument is null");
            return;
        }

        var root = _uiDocument.rootVisualElement;

        if (_styleSheet != null)
        {
            root.styleSheets.Add(_styleSheet);
            Debug.Log("StyleSheet added");
        }

        var layers = new WindowLayerRoot(root);
        _windowManager = new WindowManager(layers);
        _windowService = new WindowService(_windowManager);

        var centeredWindow = new WindowBase(new WindowOptions
        {
            Title = "Centered Window",
            Width = 420,
            Height = 260,
            Closable = true,
            Draggable = true,
            Resizable = true,
            MinWidth = 240,
            MinHeight = 160,
            CloseOnEscape = true,
            CenterOnOpen = true
        });

        var fixedWindow = new WindowBase(new WindowOptions
        {
            Title = "Fixed Window",
            Width = 360,
            Height = 220,
            Left = 80,
            Top = 70,
            Closable = true,
            Draggable = true,
            Resizable = true,
            MinWidth = 220,
            MinHeight = 140,
            CloseOnEscape = true,
            CenterOnOpen = false
        });

        _windowManager.Open(fixedWindow);
        _windowManager.Open(centeredWindow);

        await UniTask.Yield();

        await _windowService.ShowMessageAsync(
            "Centered MessageBox",
            "この MessageBox は中央表示です。");

        var result = await _windowService.ShowConfirmAsync(
            "Centered Confirm",
            "この ConfirmDialog も中央表示です。");

        Debug.Log($"Confirm result = {result}");
    }
}