using System.Collections;
using TMPro;
using UnityEngine;

public class AddTimeS : MonoBehaviour
{
    private GameObject _timerObjectCopy;

    #region Components
    [Header("Components")]
    [SerializeField] private GameObject _timerObject;
    #endregion

    #region Input State
    [Header("Input State")]
    [SerializeField] private double _time;
    #endregion

    public void AddTime()
    {
        _timerObject.GetComponent<TimerController>().time += _time;
        _timerObjectCopy = Instantiate(_timerObject);
        _timerObjectCopy.tag = "TimeAdder";
        GameObject _timerObjectChild = _timerObjectCopy.transform.GetChild(0).gameObject;
        GameObject _timerText = _timerObjectCopy.transform.GetChild(1).gameObject;
        Destroy(_timerObjectCopy.GetComponent<TimerController>());
        Destroy(_timerObjectChild);

        _timerText.transform.SetPositionAndRotation(new Vector3(_timerText.transform.position.x, 460, _timerText.transform.position.z), Quaternion.identity);
        _timerText.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 18);
        _timerText.GetComponent<TextMeshProUGUI>().text = $"+ {_time}.00";

        StartCoroutine(DestroyObject(_timerText.GetComponent<TextMeshProUGUI>()));
        FMODUnity.RuntimeManager.PlayOneShot("{dca805db-1c08-4dd3-89f1-da248e9f5cb2}");
    }

    IEnumerator DestroyObject(TextMeshProUGUI text)
    {
        yield return new WaitForSeconds(1);
        text.CrossFadeAlpha(0.0f, 0.6f, false);

        yield return new WaitForSeconds(1);
        Destroy(_timerObjectCopy);
    }
}