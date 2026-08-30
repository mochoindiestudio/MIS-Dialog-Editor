using System.Collections.Generic;
using UnityEngine;

namespace MochoIndieStudio.DialogSystem
{
    /// <summary>
    /// A dialog asset: a tree of <see cref="DialogGraphNode"/>s rooted at a
    /// <see cref="CharacterDialogNode"/>. Every tree is created with its root already in place, so
    /// the "a dialog asset always starts with a character node" rule holds from the moment the asset
    /// exists.
    /// </summary>
    [CreateAssetMenu(fileName = "New Dialog Tree", menuName = "MIS Dialog System/Dialog Tree")]
    [Icon("Packages/com.mochoindiestudio.node-dialog-system/Editor/Icons/icon_dialog_128.png")]
    public sealed class DialogTree : ScriptableObject
    {
        [SerializeField]
        private CharacterDialogNode rootNode;

        [SerializeReference]
        private List<DialogGraphNode> nodes = new List<DialogGraphNode>();

        /// <summary>The tree's root. Always a <see cref="CharacterDialogNode"/>, never a plain <see cref="DialogGraphNode"/>.</summary>
        public CharacterDialogNode RootNode => rootNode;

        /// <summary>Every non-root node in the tree (currently only <see cref="DialogNode"/>s, kept
        /// as the polymorphic base type for future node kinds).</summary>
        public List<DialogGraphNode> Nodes => nodes;

        /// <summary>
        /// Finds a node by its <see cref="DialogGraphNode.Id"/>, searching the root and every entry
        /// in <see cref="Nodes"/>. Returns null if no node has that id. Deliberately a manual loop
        /// (no LINQ) since this can run on the dialog runtime hot path.
        /// </summary>
        public DialogGraphNode GetNode(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            if (rootNode != null && rootNode.Id == id)
            {
                return rootNode;
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] != null && nodes[i].Id == id)
                {
                    return nodes[i];
                }
            }

            return null;
        }

        private void Reset()
        {
            rootNode = new CharacterDialogNode();
        }
    }
}
