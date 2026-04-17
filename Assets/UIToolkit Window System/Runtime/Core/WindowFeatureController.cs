using UnityEngine;

namespace UIToolkitWindowSystem
{
    public abstract class WindowFeatureControllerBase : MonoBehaviour
    {
        [SerializeField] private WindowSystemHost host;

        protected WindowSystemHost Host => host;
        protected WindowManager WindowManager => host != null ? host.WindowManager : null;
        protected WindowService WindowService => host != null ? host.WindowService : null;
        protected CommonWindowViewAssets CommonViews => host != null ? host.CommonViews : null;

        protected virtual void Awake()
        {
            if (host == null)
            {
                host = FindFirstObjectByType<WindowSystemHost>();
            }

            if (host == null)
            {
                Debug.LogError($"{GetType().Name}: WindowSystemHost was not found.");
                enabled = false;
            }
        }
    }
}