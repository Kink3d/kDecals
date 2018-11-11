using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using kTools.Decals;

namespace kTools.DecalsEditor
{
    public class DecalMenuItems
    {
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
    }
}
