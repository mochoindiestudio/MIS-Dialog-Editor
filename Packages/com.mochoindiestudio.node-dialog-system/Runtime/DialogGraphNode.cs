using System;
using UnityEngine;

namespace MochoIndieStudio.DialogSystem
{
    /// <summary>
    /// Base type for every node that can live inside a <see cref="DialogTree"/>'s graph. Concrete
    /// nodes are stored polymorphically via <see cref="UnityEngine.SerializeReference"/>.
    /// </summary>
    [Serializable]
    public abstract class DialogGraphNode
    {
        [SerializeField]
        private string id;

        [SerializeField]
        private Vector2 editorPosition;

        protected DialogGraphNode()
        {
            id = Guid.NewGuid().ToString("N");
        }

        /// <summary>Stable identifier used by other nodes/responses to reference this node.</summary>
        public string Id => id;

        /// <summary>Node position in the graph editor's canvas. Editor-only concern, stored with the
        /// data so no separate layout file is needed.</summary>
        public Vector2 EditorPosition
        {
            get => editorPosition;
            set => editorPosition = value;
        }
    }
}
