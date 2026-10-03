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

    [Header("Upgrade Stats")]
    [SerializeField] private float bulletDamage = 10f;
    [SerializeField] private float fireRate = 0.5f; // Cooldown/interval antar tembakan (detik)
    [SerializeField] private int projectileCount = 1;
    [SerializeField] private float spreadAngle = 15f; // Sudut sebaran peluru jika projectile > 1

    private float nextFireTime;

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;
    }

    void Update()
    {
        if (Mouse.current != null)
        {
            Vector3 screenMousePos = Mouse.current.position.ReadValue();
            screenMousePos.z = Mathf.Abs(mainCam.transform.position.z - transform.position.z);
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
        Vector2 lookDir = mousePos - (Vector2)transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0,angle - 45f);
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        Vector2 lookDir = mousePos - (Vector2)firePoint.position;

        // Karena peluru menghadap ke ATAS (+Y), kita butuh -90f agar arah atas peluru menuju ke Mouse
        float targetAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;

        if (projectileCount == 1)
        {
            InstantiateBullet(targetAngle);
        }
        // 2. Jika peluru > 1, sebarkan secara simetris di tengah
        else
        {
            float totalSpread = spreadAngle * (projectileCount - 1);
            float startAngle = targetAngle - (totalSpread / 2f);

            for (int i = 0; i < projectileCount; i++)
            {
                float currentAngle = startAngle + (i * spreadAngle);
                InstantiateBullet(currentAngle);
            }
        }
    }

    private void InstantiateBullet(float zRotation)
    {
        Quaternion bulletRotation = Quaternion.Euler(0, 0, zRotation);
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, bulletRotation);

        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDamage(bulletDamage);
        }
    }

    // --- FUNCTION CALLS DARI LEVEL UP MANAGER ---

    public void AddDamage(float amount)
    {
        bulletDamage += amount;
        Debug.Log($"Damage bertambah! Damage saat ini: {bulletDamage}");
    }

    public void AddFireRate(float amount)
    {
        // Pengurangan cooldown interval (semakin kecil interval, semakin cepat menembak)
        fireRate = Mathf.Max(0.05f, fireRate - amount);
        Debug.Log($"Cooldown tembak berkurang! Cooldown saat ini: {fireRate}s");
    }

    public void AddProjectileCount(int amount)
    {
        projectileCount += amount;
        Debug.Log($"Jumlah peluru bertambah! Peluru saat ini: {projectileCount}");
    }
}