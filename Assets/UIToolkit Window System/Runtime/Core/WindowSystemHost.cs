using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class WindowSystemHost : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private CommonWindowViewAssets commonViews;

        private WindowLayerRoot _layers;

        public WindowManager WindowManager { get; private set; }
        public WindowService WindowService { get; private set; }
        public WindowContext Context { get; private set; }

        public CommonWindowViewAssets CommonViews => commonViews;
        public UIDocument UIDocument => uiDocument;
        public VisualElement Root => uiDocument != null ? uiDocument.rootVisualElement : null;

        private void Awake()
        {
            if (uiDocument == null)
            {
                Debug.LogError("WindowSystemHost: UIDocument is not assigned.");
                enabled = false;
                return;
            }

            if (commonViews == null)
            {
                Debug.LogError("WindowSystemHost: CommonWindowViewAssets is not assigned.");
                enabled = false;
                return;
            }

            var root = uiDocument.rootVisualElement;
            if (root == null)
            {
                Debug.LogError("WindowSystemHost: rootVisualElement is null.");
                enabled = false;
                return;
            }

            if (commonViews.WindowFrameStyleSheet != null &&
                !root.styleSheets.Contains(commonViews.WindowFrameStyleSheet))
            {
                root.styleSheets.Add(commonViews.WindowFrameStyleSheet);
            }

            _layers = new WindowLayerRoot(root);
            WindowManager = new WindowManager(_layers);

            Context = new WindowContext(WindowManager, commonViews);
            WindowService = new WindowService(Context);
            Context.WindowService = WindowService;

            Debug.Log("WindowSystemHost initialized.");
        }
    }
}