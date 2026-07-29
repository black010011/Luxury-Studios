using UnityEngine;
using UnityEngine.UI;

public class Thirst : MonoBehaviour
{
    [Header("Thirst")]
    public float maxThirst = 100f;
    public float currentThirst = 100f;

    [Header("UI")]
    public Scrollbar thirstBar;

    [Header("Consumption")]
    public float idleDrain = 0.03f;
    public float walkDrain = 0.07f;
    public float runDrain = 0.15f;
    public float driveDrain = 0.04f;

    [Header("Health")]
    public Vida vida;
    public float damagePerSecond = 5f;

    private CharacterController controller;
    private EnterCar enterCar;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        enterCar = GetComponent<EnterCar>();

        UpdateBar();
    }

    void Update()
    {
        DrainThirst();
        UpdateBar();

        if (currentThirst <= 0 && vida != null)
        {
            vida.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

    void DrainThirst()
    {
        float drain = idleDrain;

        if (enterCar != null && enterCar.driving)
        {
            drain = driveDrain;
        }
        else if (controller != null)
        {
            if (controller.velocity.magnitude > 0.1f)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    drain = runDrain;
                else
                    drain = walkDrain;
            }
        }

        currentThirst -= drain * Time.deltaTime;
        currentThirst = Mathf.Clamp(currentThirst, 0, maxThirst);
    }

    void UpdateBar()
    {
        if (thirstBar != null)
            thirstBar.size = currentThirst / maxThirst;
    }

    public void Drink(float amount)
    {
        currentThirst += amount;
        currentThirst = Mathf.Clamp(currentThirst, 0, maxThirst);
        UpdateBar();
    }

    public bool IsThirsty()
    {
        return currentThirst < 25f;
    }

    public bool IsDehydrated()
    {
        return currentThirst <= 0;
    }
}