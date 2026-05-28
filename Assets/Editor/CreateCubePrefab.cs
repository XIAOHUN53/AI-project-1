using UnityEngine;
using UnityEditor;

public class CreateCubePrefab
{
    [MenuItem("Tools/Create Cube Prefab")]
    public static void CreatePrefab()
    {
        // Ensure Prefabs folder exists
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        // Find the cube in the scene
        GameObject cube = GameObject.Find("MyCube");
        if (cube == null)
        {
            Debug.LogError("No cube named 'MyCube' found in the scene!");
            return;
        }
        
        // Create prefab
        string prefabPath = "Assets/Prefabs/MyCube.prefab";
        
        // Delete existing prefab if it exists
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existingPrefab != null)
        {
            AssetDatabase.DeleteAsset(prefabPath);
        }
        
        PrefabUtility.SaveAsPrefabAsset(cube, prefabPath);
        
        Debug.Log("Cube prefab created successfully at: " + prefabPath);
    }
}