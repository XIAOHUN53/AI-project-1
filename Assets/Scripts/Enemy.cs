using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3f;
    public float minX = -8f;
    public float maxX = 8f;

    void Start()
    {
        // 确保有碰撞体（OnTriggerEnter 需要）
        if (GetComponent<Collider>() == null)
        {
            BoxCollider col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
        }
        else
        {
            // 确保已有的 Collider 是 trigger
            GetComponent<Collider>().isTrigger = true;
        }

        // 确保有 Rigidbody（子弹检测需要）
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // 设置 Enemy 标签
        gameObject.tag = "Enemy";
    }

    void Update()
    {
        // 向下移动
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // 掉到底部就销毁
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
}
