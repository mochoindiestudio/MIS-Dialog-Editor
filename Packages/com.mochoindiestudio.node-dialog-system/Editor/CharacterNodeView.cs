using MochoIndieStudio.DialogSystem;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace MochoIndieStudio.DialogSystem.Editor
{
    /// <summary>
    /// View for the tree's root <see cref="CharacterDialogNode"/>. Visually distinguished (root
    /// class name + fixed title) so it's obvious at a glance that this is where the conversation --
    /// and the character it belongs to -- starts.
    /// </summary>
    public sealed class CharacterNodeView : DialogGraphNodeView
    {
        /// <summary>The single output port leading to the first <see cref="DialogNode"/>.</summary>
        public Port OutputPort { get; }

        public CharacterNodeView(CharacterDialogNode model) : base(model)
        {
            title = "Character (Root)";
            AddToClassList("dialog-root-node");

            var characterField = new ObjectField("Character")
            {
                objectType = typeof(DialogCharacter),
                value = model.Character
            };
            characterField.RegisterValueChangedCallback(evt => model.Character = evt.newValue as DialogCharacter);
            extensionContainer.Add(characterField);

            OutputPort = CreatePort(Direction.Output, Port.Capacity.Single, model);
            OutputPort.portName = "Next";
            outputContainer.Add(OutputPort);

            RefreshExpandedState();
            RefreshPorts();
        }
    }
}
