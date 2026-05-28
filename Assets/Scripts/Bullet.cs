using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 3f;

    void Start()
    {
        // 确保子弹有碰撞体（自动添加，防止预制体缺少组件）
        if (GetComponent<Collider>() == null)
        {
            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 0.5f;
        }
        
        // 确保子弹有 Rigidbody（OnTriggerEnter 需要至少一方有 Rigidbody）
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
        }
        else
        {
            GetComponent<Rigidbody>().useGravity = false;
        }

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
            if (GameManager.Instance != null)
                GameManager.Instance.AddScore(10);
        }
    }
}
