using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    [CreateAssetMenu(menuName = "UIToolkit Window System/Window View Assets")]
    public sealed class WindowViewAssets : ScriptableObject
    {
        public VisualTreeAsset MessageBoxUxml;
        public VisualTreeAsset ConfirmDialogUxml;
        public VisualTreeAsset SampleToolWindowUxml;
    }
}