using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class WindowManager
    {
        private readonly WindowLayerRoot _layers;

        private readonly List<WindowBase> _normalWindows = new();
        private readonly Stack<ModalWindowBase> _modalStack = new();
        private readonly HashSet<WindowBase> _pendingCenterWindows = new();

        private VisualElement _modalBlocker;

        public WindowBase FocusedWindow { get; private set; }

        public WindowManager(WindowLayerRoot layers)
        {
            _layers = layers;
            Debug.Log("WindowManager created");

            _layers.Root.focusable = true;
            _layers.Root.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
            _layers.Root.RegisterCallback<GeometryChangedEvent>(OnRootGeometryChanged);
            _layers.Root.Focus();
        }

        public T Open<T>(T window) where T : WindowBase
        {
            if (window is ModalWindowBase modal)
            {
                return (T)(WindowBase)OpenModal(modal);
            }

            window.Attach(this);
            _layers.NormalWindowLayer.Add(window.Root);
            _normalWindows.Add(window);

            ApplyOpenPlacement(window);

            window.InternalOpen();

            Focus(window);
            _layers.Root.Focus();

            Debug.Log($"Open normal: {window.Options.Title}");
            return window;
        }

        public T OpenModal<T>(T window) where T : ModalWindowBase
        {
            window.Attach(this);

            EnsureModalBlocker();

            _layers.ModalWindowLayer.Add(window.Root);
            _modalStack.Push(window);

            ApplyOpenPlacement(window);

            window.InternalOpen();

            Focus(window);
            _layers.Root.Focus();

            Debug.Log($"Open modal: {window.Options.Title}");
            return window;
        }

        public void Close(WindowBase window)
        {
            if (window == null)
                return;

            _pendingCenterWindows.Remove(window);

            if (window is ModalWindowBase modal)
            {
                CloseModal(modal);
                return;
            }

            if (!_normalWindows.Remove(window))
                return;

            Debug.Log($"Close normal: {window.Options.Title}");

            if (FocusedWindow == window)
            {
                FocusedWindow.InternalSetFocused(false);
                FocusedWindow = null;
            }

            window.Root.RemoveFromHierarchy();
            window.InternalClose();

            var next = _modalStack.Count > 0
                ? _modalStack.Peek()
                : _normalWindows.LastOrDefault();

            if (next != null)
                Focus(next);

            _layers.Root.Focus();
        }

        private void CloseModal(ModalWindowBase modal)
        {
            _pendingCenterWindows.Remove(modal);

            if (!_modalStack.Contains(modal))
                return;

            Debug.Log($"Close modal: {modal.Options.Title}");

            var remaining = _modalStack.Reverse().Where(x => x != modal).Reverse().ToList();
            _modalStack.Clear();
            foreach (var item in remaining)
                _modalStack.Push(item);

            if (FocusedWindow == modal)
            {
                FocusedWindow.InternalSetFocused(false);
                FocusedWindow = null;
            }

            modal.Root.RemoveFromHierarchy();
            modal.InternalClose();

            RefreshModalBlocker();

            var next = _modalStack.Count > 0
                ? _modalStack.Peek()
                : _normalWindows.LastOrDefault();

            if (next != null)
                Focus(next);

            _layers.Root.Focus();
        }

        public void Focus(WindowBase window)
        {
            if (window == null)
                return;

            if (window is ModalWindowBase)
            {
                if (_modalStack.Count == 0 || _modalStack.Peek() != window)
                    return;
            }
            else
            {
                if (_modalStack.Count > 0)
                    return;

                if (!_normalWindows.Contains(window))
                    return;
            }

            if (FocusedWindow == window)
            {
                window.Root.BringToFront();
                _layers.Root.Focus();
                return;
            }

            FocusedWindow?.InternalSetFocused(false);

            FocusedWindow = window;
            FocusedWindow.InternalSetFocused(true);
            FocusedWindow.Root.BringToFront();

            _layers.Root.Focus();

            Debug.Log($"Focus: {window.Options.Title}");
        }

        public bool TryCloseByEscape()
        {
            if (_modalStack.Count > 0)
            {
                var topModal = _modalStack.Peek();
                if (!topModal.Options.CloseOnEscape)
                    return false;

                Debug.Log($"ESC close modal: {topModal.Options.Title}");
                topModal.RequestClose();
                return true;
            }

            if (FocusedWindow != null && FocusedWindow.Options.CloseOnEscape)
            {
                Debug.Log($"ESC close focused window: {FocusedWindow.Options.Title}");
                FocusedWindow.RequestClose();
                return true;
            }

            return false;
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Escape)
            {
                if (TryCloseByEscape())
                {
                    evt.StopPropagation();
                    evt.PreventDefault();
                }
                return;
            }

            if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
            {
                if (TrySubmitFocusedDialog())
                {
                    evt.StopPropagation();
                    evt.PreventDefault();
                }
            }
        }

        private bool TrySubmitFocusedDialog()
        {
            if (FocusedWindow == null)
                return false;

            if (FocusedWindow is DialogWindow<DialogResult> dialogResultWindow)
            {
                return dialogResultWindow.TrySubmitByKeyboard();
            }

            return false;
        }

        private void ApplyOpenPlacement(WindowBase window)
        {
            if (!window.Options.CenterOnOpen)
                return;

            if (!TryCenterWindow(window))
            {
                _pendingCenterWindows.Add(window);
                Debug.Log($"Center deferred: {window.Options.Title}");
            }
        }

        private void OnRootGeometryChanged(GeometryChangedEvent evt)
        {
            if (_pendingCenterWindows.Count == 0)
                return;

            if (!IsRootSizeValid())
                return;

            var pending = _pendingCenterWindows.ToList();
            _pendingCenterWindows.Clear();

            foreach (var window in pending)
            {
                if (window.IsOpen == false && window.Root.parent == null)
                    continue;

                if (!TryCenterWindow(window))
                {
                    _pendingCenterWindows.Add(window);
                }
            }
        }

        private bool TryCenterWindow(WindowBase window)
        {
            float rootWidth = _layers.Root.resolvedStyle.width;
            float rootHeight = _layers.Root.resolvedStyle.height;

            if (float.IsNaN(rootWidth) || float.IsNaN(rootHeight) || rootWidth <= 0 || rootHeight <= 0)
                return false;

            float width = GetWindowWidth(window);
            float height = GetWindowHeight(window);

            if (float.IsNaN(width) || float.IsNaN(height) || width <= 0 || height <= 0)
            {
                width = window.Options.Width;
                height = window.Options.Height;
            }

            float x = Mathf.Max(0f, (rootWidth - width) * 0.5f);
            float y = Mathf.Max(0f, (rootHeight - height) * 0.5f);

            window.SetPosition(x, y);

            Debug.Log($"Centered: {window.Options.Title} => ({x}, {y})");
            return true;
        }

        private bool IsRootSizeValid()
        {
            float w = _layers.Root.resolvedStyle.width;
            float h = _layers.Root.resolvedStyle.height;
            return !float.IsNaN(w) && !float.IsNaN(h) && w > 0 && h > 0;
        }

        private static float GetWindowWidth(WindowBase window)
        {
            float width = window.Root.resolvedStyle.width;
            if (float.IsNaN(width) || width <= 0)
                width = window.Options.Width;
            return width;
        }

        private static float GetWindowHeight(WindowBase window)
        {
            float height = window.Root.resolvedStyle.height;
            if (float.IsNaN(height) || height <= 0)
                height = window.Options.Height;
            return height;
        }

        private void EnsureModalBlocker()
        {
            if (_modalBlocker != null)
                return;

            _modalBlocker = new VisualElement();
            _modalBlocker.name = "modal-blocker";
            _modalBlocker.AddToClassList("modal-blocker");
            _modalBlocker.style.position = Position.Absolute;
            _modalBlocker.style.left = 0;
            _modalBlocker.style.top = 0;
            _modalBlocker.style.right = 0;
            _modalBlocker.style.bottom = 0;
            _modalBlocker.pickingMode = PickingMode.Position;

            _modalBlocker.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (_modalStack.Count > 0)
                {
                    var top = _modalStack.Peek();
                    Focus(top);
                    evt.StopPropagation();
                }
            });

            _layers.ModalBlockerLayer.Add(_modalBlocker);
            Debug.Log("Modal blocker created");
        }

        private void RefreshModalBlocker()
        {
            if (_modalStack.Count == 0)
            {
                if (_modalBlocker != null)
                {
                    _modalBlocker.RemoveFromHierarchy();
                    _modalBlocker = null;
                    Debug.Log("Modal blocker removed");
                }
            }
            else
            {
                EnsureModalBlocker();
            }
        }
    }
}