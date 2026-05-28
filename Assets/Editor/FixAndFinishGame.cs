using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FixAndFinishGame
{
    [MenuItem("Tools/Fix And Finish Game")]
    public static void FixAndFinish()
    {
        // 1. 确保 Prefabs 文件夹存在
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");

        // 2. 创建 Bullet 预制体（如果不存在）
        GameObject bulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Bullet.prefab");
        if (bulletPrefab == null)
        {
            GameObject bulletObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bulletObj.name = "Bullet";
            bulletObj.transform.localScale = Vector3.one * 0.15f;
            Object.DestroyImmediate(bulletObj.GetComponent<MeshRenderer>());
            SphereCollider sc = bulletObj.GetComponent<SphereCollider>();
            sc.isTrigger = true;
            bulletObj.AddComponent<Bullet>();
            PrefabUtility.SaveAsPrefabAsset(bulletObj, "Assets/Prefabs/Bullet.prefab");
            Object.DestroyImmediate(bulletObj);
        }

        // 3. 创建 Enemy 预制体（如果不存在）
        GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemy.prefab");
        if (enemyPrefab == null)
        {
            GameObject enemyObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            enemyObj.name = "Enemy";
            enemyObj.tag = "Enemy";
            enemyObj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            BoxCollider bc = enemyObj.GetComponent<BoxCollider>();
            bc.isTrigger = true;
            enemyObj.AddComponent<Enemy>();
            PrefabUtility.SaveAsPrefabAsset(enemyObj, "Assets/Prefabs/Enemy.prefab");
            Object.DestroyImmediate(enemyObj);
        }

        // 4. 连接 PlayerController 的子弹引用
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.bulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Bullet.prefab");
                if (pc.firePoint == null)
                {
                    GameObject fp = new GameObject("FirePoint");
                    fp.transform.SetParent(player.transform);
                    fp.transform.localPosition = new Vector3(0, 0.5f, 0);
                    pc.firePoint = fp.transform;
                }
            }
        }

        // 5. 连接 Spawner 的敌人引用
        GameObject spawner = GameObject.Find("Spawner");
        if (spawner != null)
        {
            Spawner sp = spawner.GetComponent<Spawner>();
            if (sp != null)
                sp.enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemy.prefab");
        }

        // 6. 创建 GameManager
        GameObject gm = GameObject.Find("GameManager");
        if (gm == null)
        {
            gm = new GameObject("GameManager");
        }
        GameManager gameManager = gm.GetComponent<GameManager>();
        if (gameManager == null)
            gameManager = gm.AddComponent<GameManager>();

        // 7. 连接 Score Text
        GameObject scoreGO = GameObject.Find("ScoreText");
        if (scoreGO != null)
        {
            Text scoreText = scoreGO.GetComponent<Text>();
            if (scoreText != null)
            {
                // 修复字体
                Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (font != null) scoreText.font = font;
                gameManager.scoreText = scoreText;
            }
        }

        // 8. 设置摄像机
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(0, 0, -10);
            cam.clearFlags = CameraClearFlags.Color;
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.2f);
            cam.orthographic = true;
            cam.orthographicSize = 7f;
        }

        Debug.Log("✅ 修复完成！游戏现在应该可以正常运行！");
        Debug.Log("🎮 操作：← → 方向键移动 | 空格键射击");
    }
}