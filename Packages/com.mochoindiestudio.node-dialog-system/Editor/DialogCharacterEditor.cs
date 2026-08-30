using MochoIndieStudio.DialogSystem;
using UnityEditor;
using UnityEngine;

namespace MochoIndieStudio.DialogSystem.Editor
{
    /// <summary>Draws a portrait preview below the default <see cref="DialogCharacter"/> inspector.</summary>
    [CustomEditor(typeof(DialogCharacter))]
    public sealed class DialogCharacterEditor : UnityEditor.Editor
    {
        private const float PreviewSize = 128f;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var character = (DialogCharacter)target;
            if (character.Portrait == null)
            {
                return;
            }

            if (AssetPreview.IsLoadingAssetPreview(character.Portrait.GetEntityId()))
            {
                Repaint();
            }

            var texture = AssetPreview.GetAssetPreview(character.Portrait);
            if (texture == null)
            {
                texture = character.Portrait.texture;
            }

            EditorGUILayout.Space();
            var rect = GUILayoutUtility.GetRect(PreviewSize, PreviewSize, GUILayout.ExpandWidth(false));
            GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
        }
    }
}
