using UnityEngine;

[RequireComponent(typeof(CarController))]
public class Gasolina : MonoBehaviour
{
    [Header("Fuel")]
    public float maxFuel = 100f;
    public float currentFuel = 100f;

    [Header("Consumption")]
    public float idleConsumption = 0.01f;
    public float accelerationConsumption = 0.08f;
    public float speedMultiplier = 0.0005f;

    private CarController car;

    public float FuelPercent
    {
        get { return currentFuel / maxFuel; }
    }

    public bool HasFuel
    {
        get { return currentFuel > 0f; }
    }

    void Start()
    {
        car = GetComponent<CarController>();
    }

    void Update()
    {
        if (currentFuel <= 0)
        {
            currentFuel = 0;
            return;
        }

        float consumption = idleConsumption;

        if (Mathf.Abs(car.mobileControls) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
            consumption += accelerationConsumption;

        consumption += car.speed * speedMultiplier;

        currentFuel -= consumption * Time.deltaTime;

        if (currentFuel < 0)
            currentFuel = 0;
    }

    public void AddFuel(float amount)
    {
        currentFuel += amount;

        if (currentFuel > maxFuel)
            currentFuel = maxFuel;
    }

    public void RemoveFuel(float amount)
    {
        currentFuel -= amount;

        if (currentFuel < 0)
            currentFuel = 0;
    }
}