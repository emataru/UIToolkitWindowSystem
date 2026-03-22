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

        public WindowBase(WindowOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));

            Root = new VisualElement();
            Root.name = $"window-{Id}";
            Root.AddToClassList("window");
            Root.style.position = Position.Absolute;
            Root.style.left = options.Left;
            Root.style.top = options.Top;
            Root.style.width = options.Width;
            Root.style.height = options.Height;
            Root.style.minWidth = options.MinWidth;
            Root.style.minHeight = options.MinHeight;
            Root.style.display = DisplayStyle.Flex;
            Root.style.flexDirection = FlexDirection.Column;

            TitleBar = new VisualElement();
            TitleBar.name = "title-bar";
            TitleBar.AddToClassList("window-titlebar");

            TitleLabel = new Label(options.Title);
            TitleLabel.name = "title-label";
            TitleLabel.AddToClassList("window-title");

            var spacer = new VisualElement();
            spacer.style.flexGrow = 1f;

            CloseButton = new Button(() =>
            {
                Debug.Log($"Close clicked: {Options.Title}");
                RequestClose();
            })
            {
                text = "×"
            };
            CloseButton.name = "close-button";
            CloseButton.AddToClassList("window-close-button");
            CloseButton.style.display = options.Closable ? DisplayStyle.Flex : DisplayStyle.None;

            TitleBar.Add(TitleLabel);
            TitleBar.Add(spacer);
            TitleBar.Add(CloseButton);

            ContentRoot = new VisualElement();
            ContentRoot.name = "content-root";
            ContentRoot.AddToClassList("window-content");
            ContentRoot.style.flexGrow = 1f;

            ResizeHandle = new VisualElement();
            ResizeHandle.name = "resize-handle";
            ResizeHandle.AddToClassList("window-resize-handle");
            ResizeHandle.style.display = options.Resizable ? DisplayStyle.Flex : DisplayStyle.None;

            Root.Add(TitleBar);
            Root.Add(ContentRoot);
            Root.Add(ResizeHandle);

            Root.RegisterCallback<PointerDownEvent>(OnPointerDownWindow);

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

        private void OnPointerDownWindow(PointerDownEvent evt)
        {
            Manager?.Focus(this);
        }

        internal void Attach(WindowManager manager)
        {
            Manager = manager;
        }

        internal void InternalOpen()
        {
            IsOpen = true;
            Debug.Log($"Window opened: {Options.Title}");
            OnOpened();
        }

        internal void InternalClose()
        {
            IsOpen = false;
            Debug.Log($"Window closed: {Options.Title}");
            OnClosed();
        }

        protected virtual void OnOpened()
        {
        }

        protected virtual void OnClosed()
        {
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

            Debug.Log($"Focus changed: {Options.Title} => {focused}");
        }

        public virtual void RequestClose()
        {
            Debug.Log($"RequestClose: {Options.Title}, manager={(Manager != null)}");
            Manager?.Close(this);
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
    }

    public class ModalWindowBase : WindowBase
    {
        public ModalWindowBase(WindowOptions options) : base(options)
        {
        }
    }
}