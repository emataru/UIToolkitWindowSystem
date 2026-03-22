using UnityEngine;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;
using UIToolkitWindowSystem;

public sealed class WindowSystemMinimalLauncher : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private StyleSheet _styleSheet;
    [SerializeField] private WindowViewAssets _viewAssets;

    private WindowManager _windowManager;
    private WindowService _windowService;

    private async void Start()
    {
        if (_uiDocument == null)
        {
            Debug.LogError("UIDocument is null");
            return;
        }

        if (_viewAssets == null)
        {
            Debug.LogError("WindowViewAssets is null");
            return;
        }

        var root = _uiDocument.rootVisualElement;

        if (_styleSheet != null)
            root.styleSheets.Add(_styleSheet);

        var layers = new WindowLayerRoot(root);
        _windowManager = new WindowManager(layers);
        _windowService = new WindowService(_windowManager, _viewAssets);

        var sampleWindow = new SampleToolWindow(_windowService, _viewAssets.SampleToolWindowUxml);
        _windowManager.Open(sampleWindow);

        await UniTask.Yield();

        await _windowService.ShowMessageAsync(
            "Ready",
            "通常ウィンドウの中身も UXML 化しました。");
    }
}