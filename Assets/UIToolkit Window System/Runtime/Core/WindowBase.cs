using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public class WindowBase
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public WindowOptions Options { get; }

        public VisualElement Root { get; }
        public VisualElement TitleBar { get; }
        public Label TitleLabel { get; }
        public Button CloseButton { get; }
        public VisualElement ContentRoot { get; }
        public VisualElement ResizeHandle { get; }

        public bool IsOpen { get; private set; }
        public bool IsFocused { get; private set; }
        public WindowManager Manager { get; private set; }

        private readonly WindowDragManipulator _dragManipulator;
        private readonly WindowResizeManipulator _resizeManipulator;

        public WindowBase(WindowOptions options, VisualTreeAsset frameUxml)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            if (frameUxml == null) throw new ArgumentNullException(nameof(frameUxml));

            // まず CloneTree する
            var tree = frameUxml.CloneTree();

            // 実際のウィンドウ本体を取得
            Root = tree.Q<VisualElement>("window-root");
            if (Root == null)
            {
                throw new InvalidOperationException("WindowFrame.uxml must contain 'window-root'.");
            }

            Root.name = $"window-{Id}";

            TitleBar = Root.Q<VisualElement>("title-bar");
            TitleLabel = Root.Q<Label>("title-label");
            CloseButton = Root.Q<Button>("close-button");
            ContentRoot = Root.Q<VisualElement>("content-root");
            ResizeHandle = Root.Q<VisualElement>("resize-handle");

            if (TitleBar == null || TitleLabel == null || CloseButton == null || ContentRoot == null || ResizeHandle == null)
            {
                throw new InvalidOperationException(
                    "WindowFrame.uxml must contain title-bar, title-label, close-button, content-root, resize-handle.");
            }

            Root.style.position = Position.Absolute;
            Root.style.left = options.Left;
            Root.style.top = options.Top;
            Root.style.width = options.Width;
            Root.style.height = options.Height;
            Root.style.minWidth = options.MinWidth;
            Root.style.minHeight = options.MinHeight;

            TitleLabel.text = options.Title;
            CloseButton.style.display = options.Closable ? DisplayStyle.Flex : DisplayStyle.None;
            ResizeHandle.style.display = options.Resizable ? DisplayStyle.Flex : DisplayStyle.None;

            CloseButton.clicked += RequestClose;
            Root.RegisterCallback<PointerDownEvent>(_ => Manager?.Focus(this));

            if (options.Draggable)
            {
                _dragManipulator = new WindowDragManipulator(Root, TitleBar);
            }

            if (options.Resizable)
            {
                _resizeManipulator = new WindowResizeManipulator(
                    Root,
                    ResizeHandle,
                    options.MinWidth,
                    options.MinHeight);
            }
        }

        protected VisualElement CloneContentTree(VisualTreeAsset visualTreeAsset)
        {
            if (visualTreeAsset == null)
                throw new ArgumentNullException(nameof(visualTreeAsset));

            return visualTreeAsset.CloneTree();
        }

        protected void AddContentStyleSheets(params StyleSheet[] styleSheets)
        {
            if (styleSheets == null)
                return;

            foreach (var sheet in styleSheets)
            {
                if (sheet != null && !Root.styleSheets.Contains(sheet))
                {
                    Root.styleSheets.Add(sheet);
                }
            }
        }

        internal void Attach(WindowManager manager)
        {
            Manager = manager;
        }

        internal void InternalOpen()
        {
            IsOpen = true;
            OnOpened();
        }

        internal void InternalClose()
        {
            IsOpen = false;
            OnClosed();
        }

        internal void InternalSetFocused(bool focused)
        {
            if (IsFocused == focused)
                return;

            IsFocused = focused;

            if (focused)
                Root.AddToClassList("focused");
            else
                Root.RemoveFromClassList("focused");
        }

        public void SetPosition(float x, float y)
        {
            Root.style.left = x;
            Root.style.top = y;
        }

        public void SetSize(float width, float height)
        {
            Root.style.width = width;
            Root.style.height = height;
        }

        public virtual void RequestClose()
        {
            Manager?.Close(this);
        }

        protected virtual void OnOpened()
        {
        }

        protected virtual void OnClosed()
        {
        }
    }
}