using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
public class Tds_GridWindow : EditorWindow {
	Tds_Grid grid;

	public void init(){
		grid = (Tds_Grid)FindObjectOfType(typeof(Tds_Grid));
	}

	void OnGUI()
	{
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField ("Grid Color X");
		grid.color = EditorGUILayout.ColorField (grid.color, GUILayout.Width (200));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField ("Grid Color Y");
		grid.GroundColor = EditorGUILayout.ColorField (grid.GroundColor, GUILayout.Width (200));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField ("Wall Color");
		grid.WallColor = EditorGUILayout.ColorField (grid.WallColor, GUILayout.Width (200));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField ("");
		grid.TeleportColor = EditorGUILayout.ColorField (grid.TeleportColor, GUILayout.Width (200));
		GUILayout.EndHorizontal();
	}
}
