using System;
using MochoIndieStudio.Signals.Authoring;
using UnityEngine;

namespace MochoIndieStudio.DialogSystem
{
    /// <summary>
    /// A minimal, engine/UI-agnostic hook a response can fire. The package never interprets
    /// <see cref="EventId"/> itself -- the consuming game defines the meaningful IDs and listens for
    /// them via <see cref="DialogRunner.OnResponseEvent"/> (or the shared MIS Signals bus, when
    /// <see cref="DialogRunner.PublishEventsToSignalBus"/> is on).
    /// </summary>
    [Serializable]
    public sealed class DialogEventTrigger
    {
        [Tooltip("Identifier the consuming game switches on. Pick from the list (ids declared by " +
                 "[SignalIdProvider] classes and SignalCatalog assets) or type one.")]
        [SignalId]
        [SerializeField]
        private string eventId;

        [SerializeField]
        private string payload;

        /// <summary>Identifier the consuming game switches on to decide what to do (e.g. "quest_start").</summary>
        public string EventId
        {
            get => eventId;
            set => eventId = value;
        }

        /// <summary>Optional free-form data accompanying the event (e.g. a quest ID).</summary>
        public string Payload
        {
            get => payload;
            set => payload = value;
        }
    }
}
