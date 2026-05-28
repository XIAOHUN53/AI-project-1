using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 8f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.3f;

    private float nextFireTime = 0f;
    private float xMin = -8f;
    private float xMax = 8f;

    void Update()
    {
        // 左右移动
        float moveInput = Input.GetAxis("Horizontal");
        Vector3 pos = transform.position;
        pos.x += moveInput * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, xMin, xMax);
        transform.position = pos;

        // 射击
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        }
    }
}