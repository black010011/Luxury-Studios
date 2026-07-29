using UnityEngine;
using UnityEngine.UI;

public class Food : MonoBehaviour
{
    [Header("Food")]
    public float maxFood = 100f;
    public float currentFood = 100f;

    [Header("UI")]
    public Scrollbar foodBar;

    [Header("Consumption")]
    public float idleDrain = 0.02f;
    public float walkDrain = 0.05f;
    public float runDrain = 0.12f;
    public float driveDrain = 0.03f;

    [Header("Damage")]
    public Vida vida;
    public float damagePerSecond = 4f;

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
        DrainFood();
        UpdateBar();

        if (currentFood <= 0 && vida != null)
        {
            vida.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

    void DrainFood()
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

        currentFood -= drain * Time.deltaTime;
        currentFood = Mathf.Clamp(currentFood, 0, maxFood);
    }

    void UpdateBar()
    {
        if (foodBar != null)
            foodBar.size = currentFood / maxFood;
    }

    public void Eat(float amount)
    {
        currentFood += amount;
        currentFood = Mathf.Clamp(currentFood, 0, maxFood);
        UpdateBar();
    }

    public void Drink(float amount)
    {
        Eat(amount);
    }

    public bool IsHungry()
    {
        return currentFood < 25f;
    }

    public bool IsStarving()
    {
        return currentFood <= 0;
    }
}