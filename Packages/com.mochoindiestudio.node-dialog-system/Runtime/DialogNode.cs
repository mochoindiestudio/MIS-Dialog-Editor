using System;
using System.Collections.Generic;
using UnityEngine;

namespace MochoIndieStudio.DialogSystem
{
    /// <summary>
    /// A single line of dialog: a main text plus the collection of responses the player can pick
    /// from. Each response can lead to another <see cref="DialogNode"/>, recursively, to arbitrary depth.
    /// </summary>
    [Serializable]
    public sealed class DialogNode : DialogGraphNode
    {
        [SerializeField]
        private string mainText;

        [SerializeField]
        private List<DialogResponse> responses = new List<DialogResponse>();

        /// <summary>The main text shown for this node.</summary>
        public string MainText
        {
            get => mainText;
            set => mainText = value;
        }

        /// <summary>The responses the player can choose from at this node.</summary>
        public List<DialogResponse> Responses => responses;
    }
}
