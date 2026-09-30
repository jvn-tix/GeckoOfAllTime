using UnityEngine;
using UnityEngine.InputSystem;

public class TurretController : MonoBehaviour
{
    [Header("Turret Aiming")]
    public Camera mainCam;
    private Vector2 mousePos;

    [Header("Shooting Config")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;
    private float nextFireTime;

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;
    }

    void Update()
    {
        if (Mouse.current != null)
        {
            // 1. Ambil posisi layar 2D dari mouse
            Vector3 screenMousePos = Mouse.current.position.ReadValue();

            // 2. Beri nilai Z berdasarkan jarak kamera ke posisi Turret di world space
            screenMousePos.z = Mathf.Abs(mainCam.transform.position.z - transform.position.z);

            // 3. Konversi ke World Point yang presisi
            mousePos = mainCam.ScreenToWorldPoint(screenMousePos);
        }

        // Auto-Shooting Interval
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FixedUpdate()
    {
        // Hitung arah dari Turret ke Mouse
        Vector2 lookDir = mousePos - (Vector2)transform.position;

        // Hitung sudut rotasi
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 45f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}