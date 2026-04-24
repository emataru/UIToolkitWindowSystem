using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class WindowThemeController : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private WindowThemeAssets themeAssets;
        [SerializeField] private WindowThemeKind initialTheme = WindowThemeKind.Dark;

        private WindowThemeKind _currentTheme;

        private async void Start()
        {
            await UniTask.Yield();
            ApplyTheme(initialTheme);
        }

        public void ApplyTheme(WindowThemeKind theme)
        {
            if (uiDocument == null || themeAssets == null)
                return;

            var root = uiDocument.rootVisualElement;

            RemoveAllThemeSheets(root);

            var sheets = theme switch
            {
                WindowThemeKind.Dark => themeAssets.DarkThemeStyleSheets,
                WindowThemeKind.Light => themeAssets.LightThemeStyleSheets,
                _ => null
            };

            if (sheets != null)
            {
                foreach (var sheet in sheets)
                {
                    if (sheet != null && !root.styleSheets.Contains(sheet))
                    {
                        root.styleSheets.Add(sheet);
                    }
                }
            }

            _currentTheme = theme;
        }

        private void RemoveAllThemeSheets(VisualElement root)
        {
            RemoveSheets(root, themeAssets.DarkThemeStyleSheets);
            RemoveSheets(root, themeAssets.LightThemeStyleSheets);
        }

        private static void RemoveSheets(VisualElement root, StyleSheet[] sheets)
        {
            if (sheets == null)
                return;

            foreach (var sheet in sheets)
            {
                if (sheet != null && root.styleSheets.Contains(sheet))
                {
                    root.styleSheets.Remove(sheet);
                }
            }
        }
    }
}