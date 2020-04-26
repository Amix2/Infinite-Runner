using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChunkSpawnerAvaliableChunks))]
public class ChunkSpawnerAvaliableChunksEditor : Editor
{
    public ChunkSpawner chunkSpawner;
    public void FindChunks()
    {
        string[] guids = AssetDatabase.FindAssets("t:prefab", null);

        chunkSpawner.avaliableChunks = new List<GameObject>();
        foreach (string guid in guids)
        {
            string guidPath = AssetDatabase.GUIDToAssetPath(guid);
            if (guidPath.Contains("Chunk"))
            {
                chunkSpawner.avaliableChunks.Add((GameObject)AssetDatabase.LoadAssetAtPath(guidPath, typeof(GameObject)));
            }
        }
    }

    public override void OnInspectorGUI()
    {
        //chunkSpawner = EditorGUILayout.ObjectField(chunkSpawner, typeof(GameObject), true);
        chunkSpawner = GameObject.Find("ChunkSpawner").GetComponent<ChunkSpawner>();
        if (GUILayout.Button("Assign Avaliable Chunks"))
            FindChunks();
    }
}
