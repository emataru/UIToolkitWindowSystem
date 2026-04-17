using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    [CreateAssetMenu(menuName = "UIToolkit Window System/Views/Sample Tool Window")]
    public sealed class SampleToolWindowViewAsset : ScriptableObject
    {
        public VisualTreeAsset ContentUxml;
        public StyleSheet[] StyleSheets;
    }
}