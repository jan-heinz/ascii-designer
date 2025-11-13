#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

// basically adding buttons to the GridSystem inspector to show/hide/delete the grid
[CustomEditor(typeof(GridSystem))]
public class GridSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        GridSystem gridSystem = (GridSystem)target;
        
        // add padding
        EditorGUILayout.Space(10);
        
        // add buttons
        if (GUILayout.Button("Show Grid")) gridSystem.ShowGridEditor();
        
        if (GUILayout.Button("Hide Grid")) gridSystem.HideGrid();
        
        if (GUILayout.Button("Delete Grid")) gridSystem.DeleteGrid();
    }
}
#endif