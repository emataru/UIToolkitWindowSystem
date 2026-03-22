using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitWindowSystem
{
    public sealed class WindowLayerRoot
    {
        public VisualElement Root { get; }
        public VisualElement NormalWindowLayer { get; }
        public VisualElement ModalBlockerLayer { get; }
        public VisualElement ModalWindowLayer { get; }

        public WindowLayerRoot(VisualElement root)
        {
            Root = root;

            NormalWindowLayer = CreateLayer("normal-window-layer");
            ModalBlockerLayer = CreateLayer("modal-blocker-layer");
            ModalWindowLayer = CreateLayer("modal-window-layer");

            Root.Add(NormalWindowLayer);
            Root.Add(ModalBlockerLayer);
            Root.Add(ModalWindowLayer);

            Debug.Log("WindowLayerRoot created");
        }

        private static VisualElement CreateLayer(string name)
        {
            var layer = new VisualElement();
            layer.name = name;
            layer.AddToClassList("window-layer");
            layer.style.position = Position.Absolute;
            layer.style.left = 0;
            layer.style.top = 0;
            layer.style.right = 0;
            layer.style.bottom = 0;

            // レイヤー自体は入力を拾わない
            layer.pickingMode = PickingMode.Ignore;

            return layer;
        }
    }
}