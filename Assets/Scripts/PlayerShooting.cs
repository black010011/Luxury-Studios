using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public Transform bulletSpawnPoint;
    public float fireRate = 0.2f;

    private bool isAiming;
    private float nextFireTime;

    public PlayerMovement player;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public AudioSource gunAudio;
    public Animation gun;

    [Header("UI")]
    public GameObject crosshair;

    void Update()
    {
        isAiming = player.isAiming;

        if (crosshair != null)
            crosshair.SetActive(isAiming);

        if (isAiming &&
            Input.GetMouseButton(0) &&
            Time.time >= nextFireTime &&
            !IsPointerOverUI())
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (gunAudio != null)
            gunAudio.PlayOneShot(gunAudio.clip);

        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (gun != null)
            gun.Play();

        Debug.DrawRay(bulletSpawnPoint.position, bulletSpawnPoint.forward * 100f, Color.red, 1f);
    }

    bool IsPointerOverUI()
    {
        if (EventSystem.current != null)
            return EventSystem.current.IsPointerOverGameObject();

        return false;
    }
}
