using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using kTools.Decals;

namespace kTools.DecalsEditor
{
    public class DecalMenuItems
    {
        // Create a new Decal object from Hierarchy window
        [MenuItem("GameObject/kTools/Decal", false, 10)]
        static void CreateDecalObject(MenuCommand menuCommand)
        {
            GameObject go = new GameObject();
            go.name = "Decal";
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            go.AddComponent<Decal>();
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        // Refresh all Decal objects in Scene from kTools menu
        [MenuItem("kTools/Decals/Refresh All Scene Decals", false, 1)]
        static void RefreshAllSceneDecals(MenuCommand menuCommand)
        {
            Decal[] allDecals = GameObject.FindObjectsOfType<Decal>();
            foreach(Decal decal in allDecals)
                decal.Refresh();
            EditorSceneManager.MarkAllScenesDirty();
        }
    }
}
