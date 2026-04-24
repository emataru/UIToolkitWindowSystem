using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public enum WindowThemeKind
    {
        Dark,
        Light
    }

    [CreateAssetMenu(menuName = "UIToolkit Window System/Theme Assets")]
    public sealed class WindowThemeAssets : ScriptableObject
    {
        public StyleSheet[] DarkThemeStyleSheets;
        public StyleSheet[] LightThemeStyleSheets;
    }
}