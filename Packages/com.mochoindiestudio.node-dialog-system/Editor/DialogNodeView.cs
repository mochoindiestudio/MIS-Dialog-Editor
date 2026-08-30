using System.Collections.Generic;
using MochoIndieStudio.DialogSystem;
using UnityEditor.Experimental.GraphView;
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

            InputPort = CreatePort(Direction.Input, Port.Capacity.Multi, model);
            InputPort.portName = "In";
            inputContainer.Add(InputPort);

            var mainTextField = new TextField("Main Text") { multiline = true, value = model.MainText };
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
            var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };

            var responseTextField = new TextField { value = response.ResponseText, style = { flexGrow = 1 } };
            responseTextField.RegisterValueChangedCallback(evt => response.ResponseText = evt.newValue);
            row.Add(responseTextField);

            var eventIdField = new TextField("Event") { value = GetEventId(response) };
            eventIdField.RegisterValueChangedCallback(evt => SetEventId(response, evt.newValue));
            row.Add(eventIdField);

            var removeButton = new Button(() => RemoveResponse(response)) { text = "X" };
            row.Add(removeButton);

            responsesContainer.Add(row);
            responseRows[response] = row;

            var port = CreatePort(Direction.Output, Port.Capacity.Single, response);
            port.portName = string.Empty;
            outputContainer.Add(port);
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
                outputContainer.Remove(port);
                responsePorts.Remove(response);
            }

            if (responseRows.TryGetValue(response, out var row))
            {
                responsesContainer.Remove(row);
                responseRows.Remove(response);
            }

            Model.Responses.Remove(response);

            RefreshExpandedState();
            RefreshPorts();
        }
    }
}
