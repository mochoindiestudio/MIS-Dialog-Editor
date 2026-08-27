using MochoIndieStudio.DialogSystem;
using UnityEditor;
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

        /// <summary>Inserts a small icon at the start of the node's title bar, loaded from a
        /// package-relative <paramref name="iconPath"/> (e.g. the 24px icons under Editor/Icons).</summary>
        protected void SetHeaderIcon(string iconPath, float size = 16f)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (texture == null)
            {
                return;
            }

            var icon = new Image
            {
                image = texture,
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    width = size,
                    height = size,
                    marginLeft = 4,
                    alignSelf = Align.Center
                }
            };
            titleContainer.Insert(0, icon);
        }
    }
}
