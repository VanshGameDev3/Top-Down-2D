using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    public Transform weaponTransform;         
    public GameObject projectilePrefab;       

    [Header("Tuning")]
    public float orbitRadius = 1.5f;         
    public float projectileSpeed = 10f;       
    public float maxProjectileDistance = 5f; 
    public float ProjectileLifeTime = 4f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip throwSound;

    private Camera _cam;

    void Awake()
    {
        _cam = Camera.main;
    }

    void Update()
    {
        AimWeapon();

        if (Input.GetButtonDown("Fire1"))
        {
            ThrowProjectile();
        }
    }

    void AimWeapon()
    {
        Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 dir = (mousePos - transform.position).normalized;
        weaponTransform.position = transform.position + dir * orbitRadius;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        weaponTransform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    void ThrowProjectile()
    {
        Vector3 spawnPos = weaponTransform.position;
        Quaternion rot = weaponTransform.rotation;

        GameObject proj = Instantiate(projectilePrefab, spawnPos, rot);

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = weaponTransform.up * projectileSpeed;

        WeaponLife controller = proj.GetComponent<WeaponLife>();
        if (controller != null)
            controller.Initialize(spawnPos, maxProjectileDistance, ProjectileLifeTime);

        audioSource.PlayOneShot(throwSound);
    }
}