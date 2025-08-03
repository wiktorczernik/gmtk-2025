using TMPro;
using UnityEngine;

public class KartSpeedUI : MonoBehaviour
{
    public KartController controller;
    public TMP_Text text;

    public GameObject indicator;

    public GameObject dial;
    float loops;

    bool spinning = false;
    float spint = 0;
    void Update()
    {
        if (controller.groundedForwardSpeed % 360 > loops)
        {
            loops += 1;
            spinning = true;
            spint = 0;
        }

        if (spinning)
        {
            dial.transform.rotation = Quaternion.Euler(dial.transform.rotation.x, dial.transform.rotation.y, 360 * ((3 * (spint * spint)) - (2 * (spint * spint * spint))));
            spint += Time.deltaTime;
            if (spint > 1)
            {
                spinning = false;
                spint = 0;
            }
        }

        indicator.transform.rotation = Quaternion.Euler(0f, 0, 135-controller.groundedForwardSpeed);
        text.text = Mathf.RoundToInt(controller.groundedForwardSpeed).ToString();
    }
}
