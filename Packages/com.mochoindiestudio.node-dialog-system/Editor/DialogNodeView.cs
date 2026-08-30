using System.Collections.Generic;
using MochoIndieStudio.DialogSystem;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MochoIndieStudio.DialogSystem.Editor
{
    /// <summary>
    /// View for a <see cref="DialogNode"/>: its main text, plus one output port per
    /// <see cref="DialogResponse"/> (with add/remove controls). A response's event triggers are
    /// edited in the asset's Inspector, not here.
    /// </summary>
    public sealed class DialogNodeView : DialogGraphNodeView
    {
        private const string DeleteIconPath = "Packages/com.mochoindiestudio.node-dialog-system/Editor/Icons/icon_delete.png";

        private readonly DialogGraphView graphView;
        private readonly VisualElement responsesContainer;
        private readonly Dictionary<DialogResponse, Port> responsePorts = new Dictionary<DialogResponse, Port>();
        private readonly Dictionary<DialogResponse, VisualElement> responseRows = new Dictionary<DialogResponse, VisualElement>();

        public new DialogNode Model => (DialogNode)base.Model;

        public DialogNodeView(DialogNode model, DialogGraphView owningGraphView) : base(model, owningGraphView)
        {
            this.graphView = owningGraphView;
            title = "Dialog Node";
            SetHeaderIcon("Packages/com.mochoindiestudio.node-dialog-system/Editor/Icons/icon_dialog.png");

            // Response output ports live on their own row inside the node body (see AddResponseRow),
            // not in the top-right outputContainer. Collapsing the node would hide that body and with
            // it every response port, orphaning the edges -- so this node kind is never collapsible.
            capabilities &= ~Capabilities.Collapsible;

            InputPort = CreatePort(Direction.Input, Port.Capacity.Multi, model);
            InputPort.portName = "In";
            inputContainer.Add(InputPort);

            var mainTextField = MakeTextArea("Main Text", model.MainText);
            mainTextField.RegisterValueChangedCallback(evt => model.MainText = evt.newValue);
            extensionContainer.Add(mainTextField);

            responsesContainer = new VisualElement();
            extensionContainer.Add(responsesContainer);

            var addResponseButton = new Button(AddResponse) { text = "Add Response" };
            extensionContainer.Add(addResponseButton);

            foreach (var response in model.Responses)
            {
                AddResponseRow(response);
            }

            RefreshExpandedState();
            RefreshPorts();
        }

        /// <summary>Minimum on-screen height for a dialog text area, in pixels. Enough for ~3 lines
        /// so authors see a paragraph of dialog without scrolling.</summary>
        private const float TextAreaMinHeight = 54f;

        /// <summary>Builds a word-wrapping, vertically-growing multiline <see cref="TextField"/> for
        /// authoring dialog / response copy.</summary>
        private static TextField MakeTextArea(string label, string value)
        {
            var field = new TextField(label) { multiline = true, value = value };
            field.style.whiteSpace = WhiteSpace.Normal;
            field.style.minHeight = TextAreaMinHeight;
            var input = field.Q(className: TextField.inputUssClassName);
            if (input != null)
            {
                input.style.unityTextAlign = TextAnchor.UpperLeft;
                input.style.whiteSpace = WhiteSpace.Normal;
            }

            return field;
        }

        private void AddResponse()
        {
            var response = new DialogResponse { ResponseText = "New Response" };
            Model.Responses.Add(response);
            AddResponseRow(response);
            RefreshExpandedState();
            RefreshPorts();
        }

        private void AddResponseRow(DialogResponse response)
        {
            var row = new VisualElement { style = { flexDirection = FlexDirection.Row, alignItems = Align.Center } };

            var responseTextField = MakeTextArea(null, response.ResponseText);
            responseTextField.style.flexGrow = 1;
            responseTextField.RegisterValueChangedCallback(evt => response.ResponseText = evt.newValue);
            row.Add(responseTextField);

            row.Add(MakeDeleteButton(() => RemoveResponse(response)));

            // The port sits at the end of the response's own row so its connector lines up with the
            // response it belongs to, instead of stacking in the node's top-right output area.
            var port = CreatePort(Direction.Output, Port.Capacity.Single, response);
            port.portName = string.Empty;
            row.Add(port);

            responsesContainer.Add(row);
            responseRows[response] = row;
            responsePorts[response] = port;
        }

        /// <summary>A compact icon-only button (the delete glyph from Editor/Icons) used to remove a
        /// response row. Falls back to a text "X" if the icon asset can't be loaded.</summary>
        private static Button MakeDeleteButton(System.Action onClick)
        {
            var button = new Button(onClick)
            {
                tooltip = "Delete response",
                style =
                {
                    width = 20f,
                    height = 20f,
                    paddingLeft = 2f,
                    paddingRight = 2f,
                    alignSelf = Align.Center
                }
            };

            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(DeleteIconPath);
            if (icon != null)
            {
                button.Add(new Image
                {
                    image = icon,
                    scaleMode = ScaleMode.ScaleToFit,
                    style = { flexGrow = 1 }
                });
            }
            else
            {
                button.text = "X";
            }

            return button;
        }

        private void RemoveResponse(DialogResponse response)
        {
            if (responsePorts.TryGetValue(response, out var port))
            {
                graphView.RemoveEdgesConnectedTo(port);
                responsePorts.Remove(response);
            }

            if (responseRows.TryGetValue(response, out var row))
            {
                // The port lives inside this row, so removing the row removes the port with it.
                responsesContainer.Remove(row);
                responseRows.Remove(response);
            }

            Model.Responses.Remove(response);

            RefreshExpandedState();
            RefreshPorts();
        }
    }
}
