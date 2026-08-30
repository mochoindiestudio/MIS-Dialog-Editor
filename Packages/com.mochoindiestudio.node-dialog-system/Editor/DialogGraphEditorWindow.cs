using MochoIndieStudio.DialogSystem;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MochoIndieStudio.DialogSystem.Editor
{
    /// <summary>
    /// Editor window hosting the <see cref="DialogGraphView"/> for a single <see cref="DialogTree"/> asset.
    /// </summary>
    public sealed class DialogGraphEditorWindow : EditorWindow
    {
        private DialogTree tree;
        private DialogGraphView graphView;

        [OnOpenAsset]
        private static bool OnOpenDialogTree(int instanceId, int line)
        {
            var asset = EditorUtility.EntityIdToObject(instanceId) as DialogTree;
            if (asset == null)
            {
                return false;
            }

            Open(asset);
            return true;
        }

        public static void Open(DialogTree tree)
        {
            var window = GetWindow<DialogGraphEditorWindow>();
            window.titleContent = new GUIContent(tree.name);
            window.Bind(tree);
        }

        private void Bind(DialogTree dialogTree)
        {
            tree = dialogTree;
            rootVisualElement.Clear();

            graphView = new DialogGraphView(tree)
            {
                name = "Dialog Graph View"
            };
            rootVisualElement.Add(BuildToolbar());

            graphView.style.flexGrow = 1;
            rootVisualElement.Add(graphView);
        }

        private Toolbar BuildToolbar()
        {
            var toolbar = new Toolbar();

            var snapToggle = new ToolbarToggle { text = "Snap to Grid", value = graphView.SnapToGrid };
            snapToggle.RegisterValueChangedCallback(evt => graphView.SnapToGrid = evt.newValue);
            toolbar.Add(snapToggle);

            return toolbar;
        }

        private void OnEnable()
        {
            if (tree != null)
            {
                Bind(tree);
            }
        }
    }
}
