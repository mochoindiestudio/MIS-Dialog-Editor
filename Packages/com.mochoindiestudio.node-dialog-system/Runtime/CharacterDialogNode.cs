using System;
using UnityEngine;

namespace MochoIndieStudio.DialogSystem
{
    /// <summary>
    /// The root node of every <see cref="DialogTree"/>. Anchors the conversation to a single
    /// <see cref="DialogCharacter"/>, so the character's info (name, portrait) stays reachable from
    /// any depth in the tree by walking back to the root.
    /// </summary>
    [Serializable]
    public sealed class CharacterDialogNode : DialogGraphNode
    {
        [SerializeField]
        private DialogCharacter character;

        [SerializeField]
        private string nextNodeId;

        /// <summary>The character this dialog tree belongs to.</summary>
        public DialogCharacter Character
        {
            get => character;
            set => character = value;
        }

        /// <summary>The <see cref="DialogGraphNode.Id"/> of the first <see cref="DialogNode"/> in the conversation.</summary>
        public string NextNodeId
        {
            get => nextNodeId;
            set => nextNodeId = value;
        }
    }
}
