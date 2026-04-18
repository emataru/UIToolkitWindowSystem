using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class SampleMenuController : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private StyleSheet[] styleSheets;

        [Header("Target Controllers")]
        [SerializeField] private SampleToolWindowController sampleToolWindowController;
        [SerializeField] private DialogController dialogController;
        [SerializeField] private WindowThemeController themeController;

        private MenuBuilder _menu;

        private void Start()
        {
            if (uiDocument == null)
            {
                Debug.LogError("MenuWindowController: UIDocument is not assigned.");
                enabled = false;
                return;
            }

            var root = uiDocument.rootVisualElement;

            foreach (var styleSheet in styleSheets)
            {
                if (styleSheet != null && !root.styleSheets.Contains(styleSheet))
                {
                    root.styleSheets.Add(styleSheet);
                }
            }

            BuildMenu();
        }

        private void OnDestroy()
        {
            _menu?.Dispose();
        }

        private void BuildMenu()
        {
            var root = uiDocument.rootVisualElement;

            _menu = new MenuBuilder(root);

            _menu.BeginMenuBar();

            if (_menu.BeginMenu("File"))
            {
                _menu.MenuItem("Open Sample Tool", OnOpenSampleTool);
                _menu.MenuItem("Show Message", OnShowMessage);
                _menu.MenuItem("Show Confirm", OnShowConfirm);

                _menu.MenuSeparator();
                _menu.MenuItem("Exit", OnExit);

                _menu.EndMenu();
            }

            if (_menu.BeginMenu("View"))
            {
                _menu.MenuItem("Focus Sample Tool", OnFocusSampleTool);
                _menu.MenuItem("Close Sample Tool", OnCloseSampleTool);

                _menu.EndMenu();
            }

            if(_menu.BeginMenu("Theme"))
            {
                _menu.MenuItem("Dark Theme", () => themeController.ApplyTheme(WindowThemeKind.Dark));
                _menu.MenuItem("Light Theme", () => themeController.ApplyTheme(WindowThemeKind.Light));
                _menu.EndMenu();
            }

            _menu.EndMenuBar();
        }

        private void OnOpenSampleTool()
        {
            sampleToolWindowController?.OpenSampleWindow();
        }

        private void OnFocusSampleTool()
        {
            sampleToolWindowController?.FocusSampleWindow();
        }

        private void OnCloseSampleTool()
        {
            sampleToolWindowController?.CloseSampleWindow();
        }

        private void OnShowMessage()
        {
            dialogController?.ShowMessage("Message", "メニューから表示した MessageBox です。");
        }

        private void OnShowConfirm()
        {
            dialogController?.ShowConfirm("Confirm", "メニューから表示した ConfirmDialog です。");
        }

        private void OnExit()
        {
            Debug.Log("Exit clicked");
        }
    }
}