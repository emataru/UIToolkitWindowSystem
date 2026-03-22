using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class WindowSystemInstaller : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;

        private WindowManager _windowManager;
        private WindowService _windowService;

        private void Awake()
        {
            var root = _uiDocument.rootVisualElement;
            var layers = new WindowLayerRoot(root);

            _windowManager = new WindowManager(layers);
            _windowService = new WindowService(_windowManager);
        }

        private async void Start()
        {
            await _windowService.ShowMessageAsync("起動", "Window system ready.");

            bool yes = await _windowService.ShowConfirmYesNoAsync("確認", "本当に続行しますか？");
            Debug.Log($"Confirm result = {yes}");
        }
    }
}