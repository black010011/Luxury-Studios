using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    public EnterCar enterCar;
    public TMP_Text speedText;

    void Update()
    {
        if (enterCar.currentcar != null && enterCar.driving)
        {
            speedText.text = Mathf.RoundToInt(enterCar.currentcar.speed) + " KM/H";
            speedText.gameObject.SetActive(true);
        }
        else
        {
            speedText.gameObject.SetActive(false);
        }
    }
}