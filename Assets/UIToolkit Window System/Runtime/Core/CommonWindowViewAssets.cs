using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    [CreateAssetMenu(menuName = "UIToolkit Window System/Common Window View Assets")]
    public sealed class CommonWindowViewAssets : ScriptableObject
    {
        public VisualTreeAsset WindowFrameUxml;
        public StyleSheet WindowFrameStyleSheet;

        public VisualTreeAsset MessageBoxContentUxml;
        public VisualTreeAsset ConfirmDialogContentUxml;
    }
}