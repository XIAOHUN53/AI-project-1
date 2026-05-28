using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SetupShootingGame
{
    [MenuItem("Tools/Setup Shooting Game")]
    public static void Setup()
    {
        // ===== 清理场景 =====
        // 只保留 Main Camera 和 Directional Light
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name != "Main Camera" && obj.name != "Directional Light" && obj.transform.root == obj.transform)
            {
                Object.DestroyImmediate(obj);
            }
        }

        // ===== 1. 创建 Bullet 预制体 =====
        GameObject bulletObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bulletObj.name = "Bullet";
        bulletObj.transform.localScale = Vector3.one * 0.3f;
        bulletObj.AddComponent<Bullet>();
        SphereCollider sc = bulletObj.GetComponent<SphereCollider>();
        sc.isTrigger = true;
        
        // 确保 Prefabs 文件夹存在
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        
        // 保存 Bullet 预制体
        PrefabUtility.SaveAsPrefabAsset(bulletObj, "Assets/Prefabs/Bullet.prefab");
        Object.DestroyImmediate(bulletObj);

        // ===== 2. 创建 Enemy 预制体 =====
        GameObject enemyObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        enemyObj.name = "Enemy";
        enemyObj.tag = "Enemy";
        enemyObj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        enemyObj.AddComponent<Enemy>();
        BoxCollider bc = enemyObj.GetComponent<BoxCollider>();
        bc.isTrigger = true;
        
        PrefabUtility.SaveAsPrefabAsset(enemyObj, "Assets/Prefabs/Enemy.prefab");
        Object.DestroyImmediate(enemyObj);

        // ===== 3. 创建 Player =====
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Cube);
        player.name = "Player";
        player.transform.position = new Vector3(0, -4f, 0);
        player.transform.localScale = new Vector3(1.5f, 0.5f, 0.5f);
        player.AddComponent<Rigidbody>().useGravity = false;
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        
        PlayerController pc = player.AddComponent<PlayerController>();
        
        // 创建 FirePoint 子物体
        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(player.transform);
        firePoint.transform.localPosition = new Vector3(0, 0.5f, 0);
        pc.firePoint = firePoint.transform;
        
        // 加载 Bullet 预制体
        pc.bulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Bullet.prefab");

        // ===== 4. 创建 Spawner =====
        GameObject spawner = new GameObject("Spawner");
        Spawner sp = spawner.AddComponent<Spawner>();
        sp.enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemy.prefab");

        // ===== 5. 创建 Canvas UI =====
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // 创建 Score Text
        GameObject textGO = new GameObject("ScoreText");
        textGO.transform.SetParent(canvasGO.transform);
        
        Text scoreText = textGO.AddComponent<Text>();
        scoreText.text = "Score: 0";
        scoreText.fontSize = 36;
        scoreText.color = Color.white;
        scoreText.alignment = TextAnchor.UpperLeft;
        
        // 设置字体（使用默认字体）
        Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (font != null) scoreText.font = font;
        
        RectTransform rt = textGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector3(20, -20, 0);
        rt.sizeDelta = new Vector2(200, 50);

        // ===== 6. 创建 GameManager =====
        GameObject gm = new GameObject("GameManager");
        GameManager gameManager = gm.AddComponent<GameManager>();
        gameManager.scoreText = scoreText;

        // ===== 7. 设置摄像机 =====
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(0, 0, -10);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.2f);
            cam.orthographic = true;
            cam.orthographicSize = 7f;
        }

        // ===== 8. 设置灯光 =====
        Light light = Object.FindObjectOfType<Light>();
        if (light != null)
        {
            light.transform.position = new Vector3(0, 5, -5);
            light.transform.rotation = Quaternion.Euler(30, 0, 0);
        }

        Debug.Log("射击游戏场景搭建完成！使用方向键移动，空格/鼠标左键射击！");
    }
}