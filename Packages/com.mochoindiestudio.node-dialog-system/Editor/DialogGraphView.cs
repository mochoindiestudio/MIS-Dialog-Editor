using System.Collections.Generic;
using System.Linq;
using MochoIndieStudio.DialogSystem;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MochoIndieStudio.DialogSystem.Editor
{
    /// <summary>
    /// Renders and edits a <see cref="DialogTree"/>'s node graph. Node views are a direct reflection
    /// of the underlying data: moving/connecting/disconnecting nodes writes straight back into the
    /// tree's serialized fields.
    /// </summary>
    public sealed class DialogGraphView : GraphView
    {
        private readonly DialogTree tree;
        private readonly Dictionary<string, DialogGraphNodeView> nodeViewsById = new Dictionary<string, DialogGraphNodeView>();

        public DialogGraphView(DialogTree dialogTree)
        {
            tree = dialogTree;

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            grid.StretchToParentSize();
            Insert(0, grid);

            nodeCreationRequest = context => CreateDialogNode(contentViewContainer.WorldToLocal(this.WorldToLocal(context.screenMousePosition)));

            graphViewChanged += OnGraphViewChanged;

            PopulateFromTree();
        }

        private void PopulateFromTree()
        {
            var rootView = new CharacterNodeView(tree.RootNode);
            AddElement(rootView);
            nodeViewsById[tree.RootNode.Id] = rootView;

            foreach (var node in tree.Nodes)
            {
                if (node is DialogNode dialogNode)
                {
                    var nodeView = new DialogNodeView(dialogNode, this);
                    AddElement(nodeView);
                    nodeViewsById[dialogNode.Id] = nodeView;
                }
            }

            ConnectExistingLink(rootView.OutputPort, tree.RootNode.NextNodeId);

            foreach (var node in tree.Nodes)
            {
                if (node is DialogNode dialogNode && nodeViewsById[dialogNode.Id] is DialogNodeView dialogNodeView)
                {
                    foreach (var response in dialogNode.Responses)
                    {
                        if (TryGetResponsePort(dialogNodeView, response, out var outputPort))
                        {
                            ConnectExistingLink(outputPort, response.TargetNodeId);
                        }
                    }
                }
            }
        }

        private static bool TryGetResponsePort(DialogNodeView view, DialogResponse response, out Port port)
        {
            foreach (var candidate in view.outputContainer.Query<Port>().ToList())
            {
                if (ReferenceEquals(candidate.userData, response))
                {
                    port = candidate;
                    return true;
                }
            }

            port = null;
            return false;
        }

        private void ConnectExistingLink(Port outputPort, string targetNodeId)
        {
            if (outputPort == null || string.IsNullOrEmpty(targetNodeId))
            {
                return;
            }

            if (!nodeViewsById.TryGetValue(targetNodeId, out var targetView) || targetView.InputPort == null)
            {
                return;
            }

            var edge = outputPort.ConnectTo(targetView.InputPort);
            AddElement(edge);
        }

        private void CreateDialogNode(Vector2 position)
        {
            var node = new DialogNode { EditorPosition = position };
            tree.Nodes.Add(node);

            var nodeView = new DialogNodeView(node, this);
            AddElement(nodeView);
            nodeViewsById[node.Id] = nodeView;

            MarkDirty();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var result = new List<Port>();
            foreach (var port in ports.ToList())
            {
                if (port.direction == startPort.direction || port.node == startPort.node)
                {
                    continue;
                }

                result.Add(port);
            }

            return result;
        }

        /// <summary>Removes every edge currently connected to <paramref name="port"/>, from both the
        /// view and the underlying data (via <see cref="OnGraphViewChanged"/>).</summary>
        public void RemoveEdgesConnectedTo(Port port)
        {
            var connected = edges.ToList().Where(edge => edge.output == port || edge.input == port).ToList();
            if (connected.Count == 0)
            {
                return;
            }

            DeleteElements(connected);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            if (change.edgesToCreate != null)
            {
                foreach (var edge in change.edgesToCreate)
                {
                    SetLink(edge.output, GetNodeId(edge.input));
                }
            }

            if (change.elementsToRemove != null)
            {
                foreach (var element in change.elementsToRemove)
                {
                    if (element is Edge edge)
                    {
                        SetLink(edge.output, null);
                    }
                    else if (element is DialogGraphNodeView nodeView && nodeView.Model != tree.RootNode)
                    {
                        tree.Nodes.Remove(nodeView.Model);
                        nodeViewsById.Remove(nodeView.Model.Id);
                    }
                }
            }

            MarkDirty();
            return change;
        }

        private static string GetNodeId(Port inputPort)
        {
            return inputPort != null && inputPort.userData is DialogGraphNode node ? node.Id : null;
        }

        private static void SetLink(Port outputPort, string targetNodeId)
        {
            if (outputPort == null)
            {
                return;
            }

            switch (outputPort.userData)
            {
                case CharacterDialogNode characterNode:
                    characterNode.NextNodeId = targetNodeId;
                    break;
                case DialogResponse response:
                    response.TargetNodeId = targetNodeId;
                    break;
            }
        }

        private void MarkDirty()
        {
            EditorUtility.SetDirty(tree);
        }
    }
}
