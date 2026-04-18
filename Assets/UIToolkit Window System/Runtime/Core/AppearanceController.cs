using UIToolkitWindowSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class AppearanceController
    {
        private readonly VisualElement _root;
        private readonly ThemeStyleSheetResolver _themeResolver;
        private readonly FontStyleSheetResolver _fontResolver;
        private readonly FontScaleStyleSheetResolver _fontScaleResolver;

        private StyleSheet _currentTheme;
        private StyleSheet _currentFont;
        private StyleSheet _currentFontScale;

        public AppearanceController(
            VisualElement root,
            ThemeStyleSheetResolver themeResolver,
            FontStyleSheetResolver fontResolver,
            FontScaleStyleSheetResolver fontScaleResolver)
        {
            _root = root;
            _themeResolver = themeResolver;
            _fontResolver = fontResolver;
            _fontScaleResolver = fontScaleResolver;
        }

        public void ApplyAll(UiAppearanceSettings settings)
        {
            ApplyTheme(settings.Theme);
            ApplyFont(settings.Font);
            ApplyFontScale(settings.FontScale);
        }

        public void ApplyTheme(WindowThemeKind theme)
        {
            var sheet = _themeResolver.Resolve(theme);
            ReplaceStyleSheet(ref _currentTheme, sheet);
        }

        public void ApplyFont(FontPreset font)
        {
            var sheet = _fontResolver.Resolve(font);
            ReplaceStyleSheet(ref _currentFont, sheet);
        }

        public void ApplyFontScale(FontScalePreset scale)
        {
            var sheet = _fontScaleResolver.Resolve(scale);
            ReplaceStyleSheet(ref _currentFontScale, sheet);
        }

        private void ReplaceStyleSheet(ref StyleSheet current, StyleSheet next)
        {
            if (current != null && _root.styleSheets.Contains(current))
            {
                _root.styleSheets.Remove(current);
            }

            current = next;

            if (next != null && !_root.styleSheets.Contains(next))
            {
                _root.styleSheets.Add(next);
            }
        }
    }
}