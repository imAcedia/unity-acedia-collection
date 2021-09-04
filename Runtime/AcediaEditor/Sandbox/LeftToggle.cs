#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace AcediaEditor.UIElements
{
    /// LTODO: Sandbox of LeftToggle
    /// <summary> Still in Sandbox-Mode. </summary>
    public class LeftToggle : Toggle
    {
        public LeftToggle() : base()
        {
            Add(labelElement);
            this[0].style.flexGrow = 0f;
            this[0].style.marginRight = 4f;
        }

        public LeftToggle(string label) : base(label)
        {
            Add(labelElement);
            this[0].style.flexGrow = 0f;
            this[0].style.marginRight = 4f;
        }
    }
}
#endif
