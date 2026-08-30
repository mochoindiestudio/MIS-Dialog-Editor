using MochoIndieStudio.DialogSystem;
using UnityEditor;
using UnityEngine;

namespace MochoIndieStudio.DialogSystem.Editor
{
    /// <summary>
    /// Assigns the package's custom icons to <see cref="DialogCharacter"/>/<see cref="DialogTree"/>
    /// assets in the Project window, since <c>[Icon]</c> alone isn't picked up reliably. Idempotent --
    /// runs on every domain reload but only touches the importer when the icon actually differs.
    /// </summary>
    [InitializeOnLoad]
    internal static class DialogAssetIcons
    {
        private const string IconsFolder = "Packages/com.mochoindiestudio.node-dialog-system/Editor/Icons/";

        static DialogAssetIcons()
        {
            ApplyIcon("Packages/com.mochoindiestudio.node-dialog-system/Runtime/DialogCharacter.cs", IconsFolder + "icon_character_128.png");
            ApplyIcon("Packages/com.mochoindiestudio.node-dialog-system/Runtime/DialogTree.cs", IconsFolder + "icon_dialog_128.png");
        }

        private static void ApplyIcon(string scriptPath, string iconPath)
        {
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (script == null || icon == null)
            {
                return;
            }

            var importer = (MonoImporter)AssetImporter.GetAtPath(scriptPath);
            if (importer.GetIcon() == icon)
            {
                return;
            }

            importer.SetIcon(icon);
            importer.SaveAndReimport();
        }
    }
}
