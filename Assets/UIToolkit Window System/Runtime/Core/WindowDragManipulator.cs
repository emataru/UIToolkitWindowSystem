using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class WindowDragManipulator
    {
        private readonly VisualElement _target;
        private readonly VisualElement _handle;

        private bool _dragging;
        private int _pointerId = -1;
        private Vector2 _startPointer;
        private Vector2 _startPosition;

        public WindowDragManipulator(VisualElement target, VisualElement handle)
        {
            _target = target;
            _handle = handle;

            _handle.RegisterCallback<PointerDownEvent>(OnPointerDown);
            _handle.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            _handle.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button != 0)
                return;

            _dragging = true;
            _pointerId = evt.pointerId;
            _startPointer = evt.position;
            _startPosition = new Vector2(_target.resolvedStyle.left, _target.resolvedStyle.top);

            _handle.CapturePointer(_pointerId);
            evt.StopPropagation();
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (!_dragging || evt.pointerId != _pointerId)
                return;

            var delta = evt.position - (Vector3)_startPointer;
            _target.style.left = _startPosition.x + delta.x;
            _target.style.top = _startPosition.y + delta.y;

            evt.StopPropagation();
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (!_dragging || evt.pointerId != _pointerId)
                return;

            _dragging = false;
            _handle.ReleasePointer(_pointerId);
            _pointerId = -1;

            evt.StopPropagation();
        }
    }
}