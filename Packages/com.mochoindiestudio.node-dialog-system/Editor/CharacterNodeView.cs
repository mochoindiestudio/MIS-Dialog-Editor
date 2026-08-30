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
        private const float PortraitPreviewSize = 64f;

        private readonly Image portraitPreview;

        /// <summary>The single output port leading to the first <see cref="DialogNode"/>.</summary>
        public Port OutputPort { get; }

        public CharacterNodeView(CharacterDialogNode model, DialogGraphView owningGraphView) : base(model, owningGraphView)
        {
            title = "Character (Root)";
            AddToClassList("dialog-root-node");
            SetHeaderIcon("Packages/com.mochoindiestudio.node-dialog-system/Editor/Icons/icon_character.png");

            var characterField = new ObjectField("Character")
            {
                objectType = typeof(DialogCharacter),
                value = model.Character
            };
            characterField.RegisterValueChangedCallback(evt =>
            {
                model.Character = evt.newValue as DialogCharacter;
                RefreshPortraitPreview();
            });
            extensionContainer.Add(characterField);

            portraitPreview = new Image
            {
                scaleMode = UnityEngine.ScaleMode.ScaleToFit,
                style =
                {
                    width = PortraitPreviewSize,
                    height = PortraitPreviewSize,
                    alignSelf = Align.Center,
                    marginTop = 4,
                    marginBottom = 4
                }
            };
            extensionContainer.Add(portraitPreview);
            RefreshPortraitPreview();

            OutputPort = CreatePort(Direction.Output, Port.Capacity.Single, model);
            OutputPort.portName = "Next";
            outputContainer.Add(OutputPort);

            RefreshExpandedState();
            RefreshPorts();
        }

        private void RefreshPortraitPreview()
        {
            var portrait = ((CharacterDialogNode)Model).Character != null ? ((CharacterDialogNode)Model).Character.Portrait : null;
            portraitPreview.image = portrait != null ? portrait.texture : null;
            portraitPreview.style.display = portrait != null ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
