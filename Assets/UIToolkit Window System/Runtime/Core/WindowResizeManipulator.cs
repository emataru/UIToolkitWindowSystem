using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class WindowResizeManipulator
    {
        private readonly VisualElement _target;
        private readonly VisualElement _handle;
        private readonly float _minWidth;
        private readonly float _minHeight;

        private bool _resizing;
        private int _pointerId = -1;
        private Vector2 _startPointer;
        private Vector2 _startSize;

        public WindowResizeManipulator(
            VisualElement target,
            VisualElement handle,
            float minWidth,
            float minHeight)
        {
            _target = target;
            _handle = handle;
            _minWidth = minWidth;
            _minHeight = minHeight;

            _handle.RegisterCallback<PointerDownEvent>(OnPointerDown);
            _handle.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            _handle.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button != 0)
                return;

            _resizing = true;
            _pointerId = evt.pointerId;
            _startPointer = evt.position;
            _startSize = new Vector2(_target.resolvedStyle.width, _target.resolvedStyle.height);

            _handle.CapturePointer(_pointerId);
            evt.StopPropagation();
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (!_resizing || evt.pointerId != _pointerId)
                return;

            var delta = evt.position - (Vector3)_startPointer;

            float width = Mathf.Max(_minWidth, _startSize.x + delta.x);
            float height = Mathf.Max(_minHeight, _startSize.y + delta.y);

            _target.style.width = width;
            _target.style.height = height;

            evt.StopPropagation();
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (!_resizing || evt.pointerId != _pointerId)
                return;

            _resizing = false;
            _handle.ReleasePointer(_pointerId);
            _pointerId = -1;

            evt.StopPropagation();
        }
    }
}