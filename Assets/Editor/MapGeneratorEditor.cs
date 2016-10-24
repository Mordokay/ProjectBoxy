using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (MapTerrainGenerator))]
public class MapGeneratorEditor : Editor
{
    /*
	public override void OnInspectorGUI() {
        MapTerrainGenerator mapGen = (MapTerrainGenerator)target;

        
		if (DrawDefaultInspector ()) {
			if (mapGen.autoUpdate) {
				mapGen.instanciateCubes();
			}
		}
        
		if (GUILayout.Button ("Generate")) {
			mapGen.instanciateCubes();
		}
        if (GUILayout.Button("Clear Map"))
        {
            mapGen.removeMap();
        }
    }*/
}
