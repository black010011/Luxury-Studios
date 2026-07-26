using UnityEngine;
using System.Collections;

public class WeaponAmmo : MonoBehaviour
{
    [Header("Magazine")]
    public int magazineSize = 30;
    public int currentAmmo = 30;

    [Header("Reserve")]
    public int reserveAmmo = 90;

    [Header("Reload")]
    public float reloadTime = 2f;

    public bool IsReloading { get; private set; }

    public bool CanShoot()
    {
        return !IsReloading && currentAmmo > 0;
    }

    public void Shoot()
    {
        if (CanShoot())
            currentAmmo--;
    }

    public IEnumerator Reload()
    {
        if (IsReloading)
            yield break;

        if (currentAmmo >= magazineSize)
            yield break;

        if (reserveAmmo <= 0)
            yield break;

        IsReloading = true;

        yield return new WaitForSeconds(reloadTime);

        int bulletsNeeded = magazineSize - currentAmmo;
        int bulletsToLoad = Mathf.Min(bulletsNeeded, reserveAmmo);

        currentAmmo += bulletsToLoad;
        reserveAmmo -= bulletsToLoad;

        IsReloading = false;
    }
}