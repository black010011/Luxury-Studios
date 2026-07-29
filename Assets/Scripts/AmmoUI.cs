using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    public WeaponAmmo weaponAmmo;
    public TMP_Text ammoText;

    void Update()
    {
        if (weaponAmmo == null || ammoText == null)
            return;

        ammoText.text = weaponAmmo.currentAmmo + " / " + weaponAmmo.reserveAmmo;
    }
}
