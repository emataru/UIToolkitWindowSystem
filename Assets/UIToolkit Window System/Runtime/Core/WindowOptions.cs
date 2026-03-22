using UnityEngine;

namespace UIToolkitWindowSystem
{
    public sealed class WindowOptions
    {
        public string Title { get; set; } = "Window";

        public float Width { get; set; } = 320;
        public float Height { get; set; } = 200;
        public float Left { get; set; } = 100;
        public float Top { get; set; } = 100;

        public float MinWidth { get; set; } = 180;
        public float MinHeight { get; set; } = 120;

        public bool Closable { get; set; } = true;
        public bool Draggable { get; set; } = false;
        public bool Resizable { get; set; } = false;

        public bool CloseOnEscape { get; set; } = false;
        public bool CenterOnOpen { get; set; } = false;
    }
}