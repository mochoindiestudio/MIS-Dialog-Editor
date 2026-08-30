using System.Collections.Generic;
using MochoIndieStudio.DialogSystem;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MochoIndieStudio.DialogSystem.Editor
{
    /// <summary>
    /// View for a <see cref="DialogNode"/>: its main text, plus one output port per
    /// <see cref="DialogResponse"/> (with add/remove controls).
    /// </summary>
    public sealed class DialogNodeView : DialogGraphNodeView
    {
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
        /// authoring dialog / response copy (as opposed to the single-line field used for event ids).</summary>
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

            var eventIdField = new TextField("Event") { value = GetEventId(response) };
            eventIdField.RegisterValueChangedCallback(evt => SetEventId(response, evt.newValue));
            row.Add(eventIdField);

            var removeButton = new Button(() => RemoveResponse(response)) { text = "X" };
            row.Add(removeButton);

            // The port sits at the end of the response's own row so its connector lines up with the
            // response it belongs to, instead of stacking in the node's top-right output area.
            var port = CreatePort(Direction.Output, Port.Capacity.Single, response);
            port.portName = string.Empty;
            row.Add(port);

            responsesContainer.Add(row);
            responseRows[response] = row;
            responsePorts[response] = port;
        }

        private static string GetEventId(DialogResponse response)
        {
            return response.Events.Count > 0 ? response.Events[0].EventId : string.Empty;
        }

        private static void SetEventId(DialogResponse response, string eventId)
        {
            if (string.IsNullOrEmpty(eventId))
            {
                response.Events.Clear();
                return;
            }

            if (response.Events.Count == 0)
            {
                response.Events.Add(new DialogEventTrigger());
            }

            response.Events[0].EventId = eventId;
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
