using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Grid")) {
            GridManager gridManager = (GridManager)target;
            gridManager.GenerateGrid();
        }
    }
}
