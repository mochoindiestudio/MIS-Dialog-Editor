using System;
using System.Collections.Generic;
using MochoIndieStudio.Signals;

namespace MochoIndieStudio.DialogSystem
{
    /// <summary>
    /// Reads a <see cref="DialogTree"/> asset and exposes it as pull/subscribe data for whatever UI
    /// the consuming game uses -- this class never renders anything itself. Deterministic: the same
    /// tree plus the same sequence of <see cref="SelectResponse"/> calls always resolves the same
    /// node path.
    /// </summary>
    public sealed class DialogRunner
    {
        private static readonly IReadOnlyList<DialogResponse> EmptyResponses = new List<DialogResponse>();

        private DialogTree tree;
        private DialogNode currentNode;

        /// <summary>
        /// When <c>true</c>, every <see cref="DialogEventTrigger"/> on a selected response is also
        /// reported to the shared <see cref="MisSignals"/> bus
        /// (<c>MisSignals.Report(trigger.EventId, trigger.Payload)</c>) — in addition to being raised
        /// on <see cref="OnResponseEvent"/>. This lets a MIS Quest System objective (or anything else
        /// on the bus) react to a dialog choice without the game wiring the two packages together.
        /// Default <c>false</c>: the package stays a pure data/event source unless you opt in.
        /// </summary>
        public bool PublishEventsToSignalBus { get; set; }

        /// <summary>The character this conversation belongs to, resolved from the tree's root and
        /// valid for the whole conversation regardless of the current node's depth.</summary>
        public DialogCharacter CurrentCharacter { get; private set; }

        /// <summary>The main text of the current node, or empty if no dialog is running.</summary>
        public string CurrentText => currentNode != null ? currentNode.MainText : string.Empty;

        /// <summary>The responses available at the current node, or empty if no dialog is running.</summary>
        public IReadOnlyList<DialogResponse> CurrentResponses => currentNode != null ? currentNode.Responses : EmptyResponses;

        /// <summary>True while a dialog is in progress.</summary>
        public bool IsRunning => currentNode != null;

        /// <summary>Raised once, when <see cref="Start"/> begins a new conversation.</summary>
        public event Action OnDialogStarted;

        /// <summary>Raised every time the current node changes as a result of <see cref="SelectResponse"/>.</summary>
        public event Action OnDialogAdvanced;

        /// <summary>Raised when the conversation ends, either because a response had no target node or <see cref="End"/> was called.</summary>
        public event Action OnDialogEnded;

        /// <summary>Raised for every <see cref="DialogEventTrigger"/> on a selected response, in order.</summary>
        public event Action<DialogEventTrigger> OnResponseEvent;

        /// <summary>Begins a conversation from <paramref name="dialogTree"/>'s root character node.</summary>
        public void Start(DialogTree dialogTree)
        {
            if (dialogTree == null)
            {
                throw new ArgumentNullException(nameof(dialogTree));
            }

            tree = dialogTree;
            CurrentCharacter = tree.RootNode.Character;
            currentNode = tree.GetNode(tree.RootNode.NextNodeId) as DialogNode;

            OnDialogStarted?.Invoke();

            if (currentNode == null)
            {
                End();
            }
        }

        /// <summary>
        /// Selects the response at <paramref name="responseIndex"/> in <see cref="CurrentResponses"/>:
        /// raises its events, then advances to its target node (or ends the dialog if it has none).
        /// </summary>
        public void SelectResponse(int responseIndex)
        {
            if (currentNode == null)
            {
                return;
            }

            var responses = currentNode.Responses;
            if (responseIndex < 0 || responseIndex >= responses.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(responseIndex));
            }

            var response = responses[responseIndex];
            for (int i = 0; i < response.Events.Count; i++)
            {
                DialogEventTrigger trigger = response.Events[i];
                OnResponseEvent?.Invoke(trigger);

                if (PublishEventsToSignalBus && trigger != null && !string.IsNullOrEmpty(trigger.EventId))
                {
                    MisSignals.Report(trigger.EventId, trigger.Payload);
                }
            }

            currentNode = tree.GetNode(response.TargetNodeId) as DialogNode;

            if (currentNode == null)
            {
                End();
            }
            else
            {
                OnDialogAdvanced?.Invoke();
            }
        }

        /// <summary>Ends the conversation early.</summary>
        public void End()
        {
            currentNode = null;
            CurrentCharacter = null;
            tree = null;
            OnDialogEnded?.Invoke();
        }
    }
}
