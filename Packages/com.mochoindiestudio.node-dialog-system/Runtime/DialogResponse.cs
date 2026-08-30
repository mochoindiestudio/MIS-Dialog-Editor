using System;
using System.Collections.Generic;
using UnityEngine;

namespace MochoIndieStudio.DialogSystem
{
    /// <summary>
    /// One selectable response inside a <see cref="DialogNode"/>. Selecting it can advance the
    /// conversation to another node and/or raise <see cref="Events"/> for game logic to react to.
    /// </summary>
    [Serializable]
    public sealed class DialogResponse
    {
        [SerializeField]
        private string responseText;

        [SerializeField]
        private string targetNodeId;

        [SerializeField]
        private List<DialogEventTrigger> events = new List<DialogEventTrigger>();

        /// <summary>The response text shown to the player.</summary>
        public string ResponseText
        {
            get => responseText;
            set => responseText = value;
        }

        /// <summary>
        /// The <see cref="DialogGraphNode.Id"/> this response leads to, or null/empty to end the
        /// dialog when selected.
        /// </summary>
        public string TargetNodeId
        {
            get => targetNodeId;
            set => targetNodeId = value;
        }

        /// <summary>Events raised, in order, when this response is selected.</summary>
        public List<DialogEventTrigger> Events => events;
    }
}
