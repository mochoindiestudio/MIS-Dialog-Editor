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

        private readonly DialogGraphView graph;

        /// <summary>Lower bound for a resized node's width, in canvas pixels. Stops the node being
        /// dragged down to an unusable sliver.</summary>
        private const float MinNodeWidth = 180f;

        protected DialogGraphNodeView(DialogGraphNode model, DialogGraphView graph)
        {
            Model = model;
            this.graph = graph;
            SetPosition(new Rect(model.EditorPosition, Vector2.zero));

            style.minWidth = MinNodeWidth;
            if (Model.EditorWidth > 0f)
            {
                style.width = Model.EditorWidth;
            }

            // Drag handles on every edge/corner; ResizableElement writes style.width/height back and
            // fires GeometryChangedEvent, which is where OnGeometryChanged persists the new width.
            capabilities |= Capabilities.Resizable;
            hierarchy.Add(new ResizableElement());

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        /// <summary>Every drag, box-move and programmatic move routes through here, so this is where
        /// grid snapping is applied and where the position is written back to the model.</summary>
        public override void SetPosition(Rect newPos)
        {
            if (graph != null && graph.SnapToGrid)
            {
                newPos.x = Mathf.Round(newPos.x / DialogGraphView.GridSpacing) * DialogGraphView.GridSpacing;
                newPos.y = Mathf.Round(newPos.y / DialogGraphView.GridSpacing) * DialogGraphView.GridSpacing;
            }

            base.SetPosition(newPos);
            Model.EditorPosition = newPos.position;
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            Model.EditorPosition = GetPosition().position;

            // Persist width only (height is left to auto-fit the node's content). The stored width is
            // reapplied on reopen, so a node reappears at whatever width it was last shown/resized to.
            // ResizableElement changes the width outside of graphViewChanged, so mark the asset dirty
            // here when it actually changes -- otherwise a resize-only edit wouldn't be saved.
            var width = evt.newRect.width;
            if (width > 0f && !Mathf.Approximately(width, Model.EditorWidth))
            {
                Model.EditorWidth = width;
                graph?.MarkDirty();
            }
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
