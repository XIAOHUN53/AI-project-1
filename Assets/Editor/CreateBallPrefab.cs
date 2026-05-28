using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class CreateBallPrefab
{
    [MenuItem("Tools/Create Ball Prefab")]
    public static void CreatePrefab()
    {
        // Create a sphere GameObject
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "Ball";
        
        // Add Rigidbody
        Rigidbody rb = ball.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.drag = 0f;
        rb.angularDrag = 0.05f;
        rb.useGravity = true;
        
        // Check if material exists
        Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/BallMaterial.mat");
        if (mat != null)
        {
            ball.GetComponent<Renderer>().material = mat;
        }
        
        // Create prefab
        string prefabPath = "Assets/Ball.prefab";
        
        // Delete existing prefab if it exists
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existingPrefab != null)
        {
            AssetDatabase.DeleteAsset(prefabPath);
        }
        
        PrefabUtility.SaveAsPrefabAsset(ball, prefabPath);
        
        // Clean up scene object
        Object.DestroyImmediate(ball);
        
        Debug.Log("Ball prefab created successfully at: " + prefabPath);
    }

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

    [MenuItem("Tools/Setup Shooting Game")]
    public static void SetupShootingGame()
    {
        // ===== 清理场景 =====
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name != "Main Camera" && obj.name != "Directional Light" && obj.transform.root == obj.transform)
            {
                Object.DestroyImmediate(obj);
            }
        }

        // 确保 Prefabs 文件夹存在
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");

        // 添加 Enemy 标签
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        bool enemyTagExists = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            if (tagsProp.GetArrayElementAtIndex(i).stringValue == "Enemy")
            {
                enemyTagExists = true;
                break;
            }
        }
        if (!enemyTagExists)
        {
            tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
            tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = "Enemy";
        }
        tagManager.ApplyModifiedProperties();

        // ===== 获取脚本类型（反射方式跨 Assembly）=====
        System.Type bulletType = System.Type.GetType("Bullet, Assembly-CSharp");
        System.Type enemyType = System.Type.GetType("Enemy, Assembly-CSharp");
        System.Type playerCtrlType = System.Type.GetType("PlayerController, Assembly-CSharp");
        System.Type spawnerType = System.Type.GetType("Spawner, Assembly-CSharp");
        System.Type gameManagerType = System.Type.GetType("GameManager, Assembly-CSharp");

        // ===== 1. 创建 Bullet 预制体（带完整组件：Collider + Rigidbody + 脚本）=====
        GameObject bulletObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bulletObj.name = "Bullet";
        bulletObj.transform.localScale = Vector3.one * 0.15f;
        // 移除不需要的 MeshRenderer（球形用 Collider 足矣）
        Object.DestroyImmediate(bulletObj.GetComponent<MeshRenderer>());
        // 设置 Collider 为 Trigger
        SphereCollider sc = bulletObj.GetComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = 0.5f;
        // 添加 Rigidbody（OnTriggerEnter 必需）
        Rigidbody bulletRb = bulletObj.AddComponent<Rigidbody>();
        bulletRb.useGravity = false;
        bulletRb.isKinematic = true;
        // 添加脚本
        if (bulletType != null) bulletObj.AddComponent(bulletType);
        PrefabUtility.SaveAsPrefabAsset(bulletObj, "Assets/Prefabs/Bullet.prefab");
        Object.DestroyImmediate(bulletObj);

        // ===== 2. 创建 Enemy 预制体（带完整组件：Collider + Rigidbody + 脚本）=====
        GameObject enemyObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        enemyObj.name = "Enemy";
        enemyObj.tag = "Enemy";
        enemyObj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        // 设置 Collider 为 Trigger
        BoxCollider bc = enemyObj.GetComponent<BoxCollider>();
        bc.isTrigger = true;
        // 添加 Rigidbody（OnTriggerEnter 必需）
        Rigidbody enemyRb = enemyObj.AddComponent<Rigidbody>();
        enemyRb.useGravity = false;
        enemyRb.isKinematic = true;
        // 添加脚本
        if (enemyType != null) enemyObj.AddComponent(enemyType);
        PrefabUtility.SaveAsPrefabAsset(enemyObj, "Assets/Prefabs/Enemy.prefab");
        Object.DestroyImmediate(enemyObj);

        // ===== 3. 创建 Player =====
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Cube);
        player.name = "Player";
        player.transform.position = new Vector3(0, -4f, 0);
        player.transform.localScale = new Vector3(1.5f, 0.5f, 0.5f);
        player.AddComponent<Rigidbody>().useGravity = false;
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        
        if (playerCtrlType != null)
        {
            Component pc = player.AddComponent(playerCtrlType);
            var firePoint = new GameObject("FirePoint");
            firePoint.transform.SetParent(player.transform);
            firePoint.transform.localPosition = new Vector3(0, 0.5f, 0);
            var firePointField = playerCtrlType.GetField("firePoint");
            if (firePointField != null) firePointField.SetValue(pc, firePoint.transform);
            var bulletField = playerCtrlType.GetField("bulletPrefab");
            if (bulletField != null) bulletField.SetValue(pc, AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Bullet.prefab"));
        }

        // ===== 4. 创建 Spawner =====
        GameObject spawner = new GameObject("Spawner");
        if (spawnerType != null)
        {
            Component sp = spawner.AddComponent(spawnerType);
            var enemyField = spawnerType.GetField("enemyPrefab");
            if (enemyField != null) enemyField.SetValue(sp, AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemy.prefab"));
        }

        // ===== 5. 创建 Canvas UI =====
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject textGO = new GameObject("ScoreText");
        textGO.transform.SetParent(canvasGO.transform);
        
        Text scoreText = textGO.AddComponent<Text>();
        scoreText.text = "Score: 0";
        scoreText.fontSize = 36;
        scoreText.color = Color.white;
        scoreText.alignment = TextAnchor.UpperLeft;
        
        RectTransform rt = textGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector3(20, -20, 0);
        rt.sizeDelta = new Vector2(200, 50);

        // ===== 6. 创建 GameManager =====
        GameObject gm = GameObject.Find("GameManager");
        if (gm == null) gm = new GameObject("GameManager");
        
        if (gameManagerType != null)
        {
            Component existingComp = gm.GetComponent(gameManagerType);
            if (existingComp == null) existingComp = gm.AddComponent(gameManagerType);
            var scoreField = gameManagerType.GetField("scoreText");
            if (scoreField != null) scoreField.SetValue(existingComp, scoreText);
        }

        // ===== 7. 连接 Player Spawner 引用 =====
        if (playerCtrlType != null)
        {
            var pc = player.GetComponent(playerCtrlType);
            if (pc != null)
            {
                var bulletField = playerCtrlType.GetField("bulletPrefab");
                if (bulletField != null) bulletField.SetValue(pc, AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Bullet.prefab"));
            }
        }
        if (spawnerType != null)
        {
            var sp = spawner.GetComponent(spawnerType);
            if (sp != null)
            {
                var enemyField = spawnerType.GetField("enemyPrefab");
                if (enemyField != null) enemyField.SetValue(sp, AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemy.prefab"));
            }
        }

        // ===== 8. 设置摄像机 =====
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(0, 0, -10);
            cam.clearFlags = CameraClearFlags.Color;
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

        Debug.Log("✅ 射击游戏场景搭建完成！使用方向键←→移动，空格/鼠标左键射击！");
    }
}
