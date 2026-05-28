using UnityEngine;
using UnityEditor;

public class SaveToPrefab
{
    [MenuItem("Tools/Save Selected As Prefab")]
    public static void SaveSelectedAsPrefab()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogError("请先在场景中选择一个物体！");
            return;
        }

        // 确保 Prefabs 文件夹存在
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        string prefabPath = $"Assets/Prefabs/{selected.name}.prefab";

        // 如果已存在则删除旧的
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existing != null)
        {
            AssetDatabase.DeleteAsset(prefabPath);
        }

        // 把场景中的物体保存为预制体
        PrefabUtility.SaveAsPrefabAsset(selected, prefabPath);
        
        Debug.Log($"已将 [{selected.name}] 保存为预制体: {prefabPath}");
    }
}