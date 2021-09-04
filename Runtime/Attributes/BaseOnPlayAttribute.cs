using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Acedia
{
    /// <summary>
    /// Base class for attributes that need to know if the editor is in play-mode.
    /// </summary>
    /// <inheritdoc cref="BaseIfAttribute"/>
    public abstract class BaseOnPlayAttribute : BaseIfAttribute, IMultipleAttribute
    {
        /// <inheritdoc cref="BaseOnPlayAttribute"/>
        public BaseOnPlayAttribute() : base("")
        {

        }

#if UNITY_EDITOR
        public override bool CompareValue(SerializedProperty property)
            => EditorApplication.isPlayingOrWillChangePlaymode != invert;
#endif
    }
}