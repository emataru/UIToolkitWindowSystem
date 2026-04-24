using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class MenuBuilder : IDisposable
    {
        private readonly VisualElement _root;
        private VisualElement _menuBar;

        // 構築中のメニューパネル（ネスト用）
        private readonly Stack<VisualElement> _menuPanelStack = new();

        // 各パネルの「階層深さ」（0 = ルート）
        private readonly Dictionary<VisualElement, int> _panelDepth = new();

        // 現在開いているパネル一覧（複数階層）
        private readonly List<VisualElement> _openPanels = new();

        // 登録済みメニュー
        private readonly Dictionary<string, MenuEntry> _entries = new();

        private bool _disposed;

        public MenuBuilder(VisualElement root)
        {
            _root = root ?? throw new ArgumentNullException(nameof(root));

            // メニュー外クリックで閉じる
            root.style.flexGrow = 1;
            root.pickingMode = PickingMode.Position;

            // root 内クリックでメニュー外判定
            _root.RegisterCallback<PointerDownEvent>(OnRootPointerDown);

            // Esc キーで閉じる
            _root.RegisterCallback<KeyDownEvent>(OnRootKeyDown, TrickleDown.TrickleDown);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _root.UnregisterCallback<PointerDownEvent>(OnRootPointerDown);
            _root.UnregisterCallback<KeyDownEvent>(OnRootKeyDown, TrickleDown.TrickleDown);
        }

        #region 登録

        private void Register(string id, VisualElement ve)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id is null/empty", nameof(id));

            _entries[id] = new MenuEntry(ve);
        }

        public MenuEntry GetEntry(string id) => _entries[id];

        public bool TryGetEntry(string id, out MenuEntry entry) =>
            _entries.TryGetValue(id, out entry);

        #endregion

        public void BeginMenuBar()
        {
            if (_menuBar != null)
                throw new InvalidOperationException("MenuBar is already begun.");

            CloseAllMenus();
            _menuPanelStack.Clear();
            _panelDepth.Clear();

            _menuBar = new VisualElement();
            _menuBar.name = "MenuBar";
            _menuBar.AddToClassList("menu-bar");
            _root.Add(_menuBar);
        }

        public void EndMenuBar()
        {
            _menuPanelStack.Clear();
            _menuBar = null;
        }

        private (Button rootButton, VisualElement panel) CreateRootMenu(string label)
        {
            var menuRoot = new VisualElement();
            menuRoot.AddToClassList("menu-root");
            _menuBar.Add(menuRoot);

            var button = new Button { text = label };
            button.AddToClassList("menu-item");
            menuRoot.Add(button);

            var panel = new VisualElement();
            panel.AddToClassList("menu-panel");
            panel.style.display = DisplayStyle.None;
            menuRoot.Add(panel);

            _panelDepth[panel] = 0;

            button.RegisterCallback<ClickEvent>(_ => ToggleMenu(panel));

            button.RegisterCallback<PointerEnterEvent>(_ =>
            {
                if (_openPanels.Count > 0)
                    OpenMenu(panel);
            });

            return (button, panel);
        }

        public bool BeginMenu(string id)
        {
            if (_menuBar == null)
                throw new InvalidOperationException("BeginMenuBar first.");

            var (btn, panel) = CreateRootMenu(id);
            Register(id, btn);

            _menuPanelStack.Push(panel);
            return true;
        }

        private (Button button, VisualElement panel) CreateSubMenu(string label)
        {
            if (_menuPanelStack.Count == 0)
                throw new InvalidOperationException("BeginSubMenu must be inside BeginMenu / EndMenu.");

            var parentPanel = _menuPanelStack.Peek();
            var parentDepth = _panelDepth[parentPanel];
            var depth = parentDepth + 1;

            var submenuRoot = new VisualElement();
            submenuRoot.AddToClassList("submenu-root");
            parentPanel.Add(submenuRoot);

            var button = new Button { text = label + "  >" };
            button.AddToClassList("menu-item");
            submenuRoot.Add(button);

            var panel = new VisualElement();
            panel.AddToClassList("menu-panel");
            panel.AddToClassList("submenu-panel");
            panel.style.display = DisplayStyle.None;
            submenuRoot.Add(panel);

            _panelDepth[panel] = depth;

            button.RegisterCallback<ClickEvent>(_ => ToggleMenu(panel));

            button.RegisterCallback<PointerEnterEvent>(_ =>
            {
                if (_openPanels.Count > 0)
                    OpenMenu(panel);
            });

            return (button, panel);
        }

        public bool BeginSubMenu(string id)
        {
            if (_menuPanelStack.Count == 0)
                throw new InvalidOperationException("BeginSubMenu must be inside BeginMenu / EndMenu.");

            var (btn, panel) = CreateSubMenu(id);
            Register(id, btn);

            _menuPanelStack.Push(panel);
            return true;
        }

        public void EndMenu()
        {
            if (_menuPanelStack.Count == 0)
                throw new InvalidOperationException("No menu to end.");

            _menuPanelStack.Pop();
        }

        public MenuEntry MenuBarItem(string id, Action onClick)
        {
            if (_menuBar == null)
                throw new InvalidOperationException("BeginMenuBar must be called before MenuBarItem.");

            var button = new Button { text = id };
            button.AddToClassList("menu-item");

            button.clicked += () =>
            {
                CloseAllMenus();
                onClick?.Invoke();
            };

            _menuBar.Add(button);
            Register(id, button);
            return GetEntry(id);
        }

        public MenuEntry MenuItem(string id, Action onClick)
        {
            if (_menuPanelStack.Count == 0)
                throw new InvalidOperationException("MenuItem must be inside BeginMenu / EndMenu.");

            var panel = _menuPanelStack.Peek();

            var button = new Button { text = id };
            button.AddToClassList("menu-item");

            button.clicked += () =>
            {
                CloseAllMenus();
                onClick?.Invoke();
            };

            panel.Add(button);
            Register(id, button);
            return GetEntry(id);
        }

        public MenuEntry MenuItem(string id, bool initialChecked, Action<bool> onToggled)
        {
            if (_menuPanelStack.Count == 0)
                throw new InvalidOperationException("MenuItem must be inside BeginMenu / EndMenu.");

            var panel = _menuPanelStack.Peek();
            bool isChecked = initialChecked;

            var button = new Button { text = GetCheckedText(id, isChecked) };
            button.AddToClassList("menu-item");

            button.clicked += () =>
            {
                isChecked = !isChecked;
                button.text = GetCheckedText(id, isChecked);

                CloseAllMenus();
                onToggled?.Invoke(isChecked);
            };

            panel.Add(button);
            Register(id, button);
            return GetEntry(id);
        }

        private static string GetCheckedText(string label, bool isChecked)
        {
            return (isChecked ? "✔ " : "   ") + label;
        }

        public void MenuBarSeparator(float width = 1f, float height = 16f)
        {
            if (_menuBar == null)
                throw new InvalidOperationException("BeginMenuBar must be called before MenuBarSeparator.");

            var sep = new VisualElement();
            sep.AddToClassList("menu-bar-separator");
            sep.style.width = width;
            sep.style.height = height;
            sep.style.alignSelf = Align.Center;

            _menuBar.Add(sep);
        }

        public void MenuSeparator(float height = 1f)
        {
            if (_menuPanelStack.Count == 0)
                throw new InvalidOperationException("MenuSeparator must be inside BeginMenu / EndMenu.");

            var panel = _menuPanelStack.Peek();

            var sep = new VisualElement();
            sep.AddToClassList("menu-item-separator");
            sep.style.height = height;

            panel.Add(sep);
        }

        private void ToggleMenu(VisualElement panel)
        {
            if (_openPanels.Contains(panel))
            {
                CloseFromDepth(_panelDepth[panel]);
            }
            else
            {
                OpenMenu(panel);
            }
        }

        private void OpenMenu(VisualElement panel)
        {
            int depth = _panelDepth[panel];

            for (int i = _openPanels.Count - 1; i >= 0; i--)
            {
                var p = _openPanels[i];
                if (_panelDepth[p] >= depth)
                {
                    p.style.display = DisplayStyle.None;
                    _openPanels.RemoveAt(i);
                }
            }

            panel.style.display = DisplayStyle.Flex;
            _openPanels.Add(panel);
        }

        private void CloseFromDepth(int depth)
        {
            for (int i = _openPanels.Count - 1; i >= 0; i--)
            {
                var p = _openPanels[i];
                if (_panelDepth[p] >= depth)
                {
                    p.style.display = DisplayStyle.None;
                    _openPanels.RemoveAt(i);
                }
            }
        }

        private void CloseAllMenus()
        {
            foreach (var p in _openPanels)
            {
                p.style.display = DisplayStyle.None;
            }

            _openPanels.Clear();
        }

        private void OnRootPointerDown(PointerDownEvent evt)
        {
            if (_openPanels.Count == 0)
                return;

            var targetVe = evt.target as VisualElement;
            if (targetVe == null)
                return;

            // メニューバー内クリックは閉じない
            if (_menuBar != null && (_menuBar == targetVe || _menuBar.Contains(targetVe)))
                return;

            // 開いているパネル内クリックも閉じない
            foreach (var p in _openPanels)
            {
                if (p == targetVe || p.Contains(targetVe))
                    return;
            }

            // それ以外は閉じる
            CloseAllMenus();
        }

        private void OnRootKeyDown(KeyDownEvent evt)
        {
            if (_openPanels.Count == 0)
                return;

            if (evt.keyCode == KeyCode.Escape)
            {
                CloseAllMenus();
                evt.StopPropagation();
            }
        }

        public void CloseMenusFromOutside()
        {
            CloseAllMenus();
        }

        public void SetMenuBarIcon(Texture2D iconTexture, Action onClick = null)
        {
            if (_menuBar == null)
                throw new InvalidOperationException("BeginMenuBar must be called before SetMenuBarIcon.");

            if (iconTexture == null)
                throw new ArgumentNullException(nameof(iconTexture));

            var existing = _menuBar.Q<VisualElement>("MenuBarIcon");
            existing?.RemoveFromHierarchy();

            var iconButton = new Button();
            iconButton.name = "MenuBarIcon";
            iconButton.AddToClassList("menu-bar-icon");
            iconButton.style.backgroundImage = new StyleBackground(iconTexture);

            if (onClick != null)
            {
                iconButton.clicked += () =>
                {
                    CloseMenusFromOutside();
                    onClick.Invoke();
                };
            }

            _menuBar.Insert(0, iconButton);
        }
    }

    public sealed class MenuEntry
    {
        internal VisualElement Element;

        internal MenuEntry(VisualElement element)
        {
            Element = element;
        }

        public void SetVisible(bool visible)
        {
            Element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public bool IsVisible => Element.style.display != DisplayStyle.None;

        public void SetEnabled(bool enabled)
        {
            Element.SetEnabled(enabled);

            if (enabled)
                Element.RemoveFromClassList("menu-item-disabled");
            else
                Element.AddToClassList("menu-item-disabled");
        }

        public bool IsEnabled => Element.enabledSelf;
    }
}