using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Fire")]
    public float fireRate = 0.15f;

    [Header("Ammo")]
    public WeaponAmmo weaponAmmo;

    [Header("Player")]
    public PlayerMovement player;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public AudioSource gunAudio;

    private float nextFireTime;

    void Update()
    {
        if (player != null && Input.GetMouseButton(0))
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(weaponAmmo.Reload());
        }
    }

    void Shoot()
    {
        if (!weaponAmmo.CanShoot())
            return;

        weaponAmmo.Shoot();

        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (gunAudio != null)
            gunAudio.Play();

        Debug.Log("Disparo");
    }
}
