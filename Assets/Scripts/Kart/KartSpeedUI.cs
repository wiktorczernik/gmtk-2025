using TMPro;
using UnityEngine;

public class KartSpeedUI : MonoBehaviour
{
    public KartController controller;
    public TMP_Text text;

    void Update()
    {
        text.text = Mathf.RoundToInt(controller.groundedForwardSpeed).ToString();
    }
}
