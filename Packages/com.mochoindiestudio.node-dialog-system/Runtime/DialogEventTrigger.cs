using System;
using UnityEngine;

namespace MochoIndieStudio.DialogSystem
{
    /// <summary>
    /// A minimal, engine/UI-agnostic hook a response can fire. The package never interprets
    /// <see cref="EventId"/> itself -- the consuming game defines the meaningful IDs and listens for
    /// them via <see cref="DialogRunner.OnResponseEvent"/>.
    /// </summary>
    [Serializable]
    public sealed class DialogEventTrigger
    {
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
