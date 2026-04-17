using UnityEngine;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;
using UIToolkitWindowSystem;

public sealed class WindowSystemMinimalLauncher : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private CommonWindowViewAssets _commonViews;
    [SerializeField] private SampleToolWindowViewAsset _sampleToolWindowView;

    private WindowManager _windowManager;
    private WindowService _windowService;

    private async void Start()
    {
        if (_uiDocument == null)
        {
            Debug.LogError("UIDocument is null");
            return;
        }

        if (_commonViews == null)
        {
            Debug.LogError("CommonWindowViewAssets is null");
            return;
        }

        if (_sampleToolWindowView == null)
        {
            Debug.LogError("SampleToolWindowViewAsset is null");
            return;
        }

        var root = _uiDocument.rootVisualElement;

        if (_commonViews.WindowFrameStyleSheet != null &&
            !root.styleSheets.Contains(_commonViews.WindowFrameStyleSheet))
        {
            root.styleSheets.Add(_commonViews.WindowFrameStyleSheet);
        }

        var layers = new WindowLayerRoot(root);
        _windowManager = new WindowManager(layers);
        _windowService = new WindowService(_windowManager, _commonViews);

        var sampleWindow = new SampleToolWindow(
            _windowService,
            _commonViews.WindowFrameUxml,
            _sampleToolWindowView);

        _windowManager.Open(sampleWindow);

        await UniTask.Yield();

        await _windowService.ShowMessageAsync(
            "Ready",
            "WindowFrame.uxml ベースで起動しました。");
    }
}