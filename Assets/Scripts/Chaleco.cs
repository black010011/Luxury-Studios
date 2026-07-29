using UnityEngine;
using UnityEngine.UI;

public class Chaleco : MonoBehaviour
{
    public float maxArmor = 100f;
    public float currentArmor;

    public Scrollbar armorBar;

    void Start()
    {
        currentArmor = maxArmor;
        UpdateArmorBar();
    }

    public void TakeArmorDamage(float damage)
    {
        currentArmor -= damage;

        if (currentArmor < 0f)
            currentArmor = 0f;

        UpdateArmorBar();
    }

    public void RepairArmor(float amount)
    {
        currentArmor += amount;

        if (currentArmor > maxArmor)
            currentArmor = maxArmor;

        UpdateArmorBar();
    }

    public void SetArmor(float amount)
    {
        currentArmor = Mathf.Clamp(amount, 0f, maxArmor);
        UpdateArmorBar();
    }

    public void FillArmor()
    {
        currentArmor = maxArmor;
        UpdateArmorBar();
    }

    public void UpdateArmorBar()
    {
        if (armorBar != null)
            armorBar.size = currentArmor / maxArmor;
    }
}