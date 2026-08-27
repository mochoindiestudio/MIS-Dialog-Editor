using MochoIndieStudio.DialogSystem;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MochoIndieStudio.DialogSystem.Editor
{
    /// <summary>
    /// Base view for a node rendered inside <see cref="DialogGraphView"/>. Keeps the view's on-canvas
    /// position written back into the underlying <see cref="DialogGraphNode.EditorPosition"/>, so the
    /// graph is always a direct reflection of the serialized data.
    /// </summary>
    public abstract class DialogGraphNodeView : Node
    {
        /// <summary>The data this view represents.</summary>
        public DialogGraphNode Model { get; }

        /// <summary>The node's single input port (null for the root character node, which nothing points to).</summary>
        public Port InputPort { get; protected set; }

        protected DialogGraphNodeView(DialogGraphNode model)
        {
            Model = model;
            SetPosition(new Rect(model.EditorPosition, Vector2.zero));
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            Model.EditorPosition = GetPosition().position;
        }

        protected static Port CreatePort(Direction direction, Port.Capacity capacity, object userData)
        {
            var port = Port.Create<Edge>(Orientation.Horizontal, direction, capacity, typeof(bool));
            port.portName = string.Empty;
            port.userData = userData;
            return port;
        }
    }
}
