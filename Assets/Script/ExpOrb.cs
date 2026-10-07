using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    [Header("EXP Settings")]
    [SerializeField] private int expAmount = 10;
    [SerializeField] private float moveSpeed = 8f;

    [Header("Audio")]
    [SerializeField] private AudioClip collectSfx;

    private Transform playerTransform;
    private bool isAttracted = false;

    void Update()
    {
        // Jika sudah masuk radius sedot player, bergerak menuju player
        if (isAttracted && playerTransform != null)
        {
            moveSpeed += Time.deltaTime * 5f; 
            transform.position = Vector3.MoveTowards(
                transform.position,
                playerTransform.position,
                moveSpeed * Time.deltaTime
            );

            // Jika sudah sangat dekat dengan player, berikan EXP lalu hancurkan Orb
            if (Vector2.Distance(transform.position, playerTransform.position) < 0.3f)
            {
                CollectOrb();
            }
        }
    }

    // Dipanggil oleh PlayerExp ketika Orb masuk radius pickup
    public void StartAttracting(Transform target)
    {
        //Debug.Log("Orb mendeteksi Player! Mulai bergerak.");
        playerTransform = target;
        isAttracted = true;
    }

    private void CollectOrb()
    {
        //Debug.Log("Orb sampai di Player, mencoba memberikan EXP...");
        if (playerTransform != null)
        {
            // Cari PlayerExp di objek tersebut atau di parent-nya
            PlayerExp playerExp = playerTransform.GetComponent<PlayerExp>();
            if (playerExp == null)
            {
                playerExp = playerTransform.GetComponentInParent<PlayerExp>();
            }

            if (playerExp != null)
            {
                playerExp.AddExperience(expAmount);
            }
        }
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(collectSfx);
        Destroy(gameObject);
    }
}