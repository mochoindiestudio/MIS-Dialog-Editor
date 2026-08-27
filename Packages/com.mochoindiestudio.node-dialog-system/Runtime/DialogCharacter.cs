using UnityEngine;

namespace MochoIndieStudio.DialogSystem
{
    /// <summary>
    /// A reusable character definition. One <see cref="DialogCharacter"/> asset can be referenced as
    /// the root of many <see cref="DialogTree"/> assets.
    /// </summary>
    [CreateAssetMenu(fileName = "New Dialog Character", menuName = "MIS Dialog System/Character")]
    public sealed class DialogCharacter : ScriptableObject
    {
        [SerializeField]
        private string displayName;

        [SerializeField]
        private Sprite portrait;

        /// <summary>The character's name, as shown in dialog UI.</summary>
        public string DisplayName => displayName;

        /// <summary>The character's 2D portrait sprite, retrievable from anywhere in a dialog tree.</summary>
        public Sprite Portrait => portrait;
    }
}
