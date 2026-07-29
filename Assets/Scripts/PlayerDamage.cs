using UnityEngine;

[RequireComponent(typeof(Vida))]
[RequireComponent(typeof(Chaleco))]
public class PlayerDamage : MonoBehaviour
{
    public Vida vida;
    public Chaleco chaleco;

    public bool armorAbsorbsAllDamage = true;

    void Awake()
    {
        if (vida == null)
            vida = GetComponent<Vida>();

        if (chaleco == null)
            chaleco = GetComponent<Chaleco>();
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f)
            return;

        if (chaleco != null && chaleco.currentArmor > 0f && armorAbsorbsAllDamage)
        {
            if (chaleco.currentArmor >= damage)
            {
                chaleco.TakeArmorDamage(damage);
            }
            else
            {
                float remainingDamage = damage - chaleco.currentArmor;
                chaleco.TakeArmorDamage(chaleco.currentArmor);
                vida.TakeDamage(remainingDamage);
            }

            return;
        }

        vida.TakeDamage(damage);
    }

    public void Heal(float amount)
    {
        vida.Heal(amount);
    }

    public void RepairArmor(float amount)
    {
        chaleco.RepairArmor(amount);
    }

    public void GiveFullArmor()
    {
        chaleco.currentArmor = chaleco.maxArmor;
        chaleco.UpdateArmorBar();
    }

    public void RemoveArmor()
    {
        chaleco.currentArmor = 0f;
        chaleco.UpdateArmorBar();
    }

    public float GetHealth()
    {
        return vida.currentHealth;
    }

    public float GetArmor()
    {
        return chaleco.currentArmor;
    }

    public bool IsDead()
    {
        return vida.currentHealth <= 0f;
    }
}