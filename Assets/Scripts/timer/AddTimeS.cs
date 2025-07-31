using System.Collections;
using TMPro;
using UnityEngine;

public class AddTimeS : MonoBehaviour
{
    private GameObject _timerObjectCopy;

    #region GameObjects
    [Header("GameObjects")]
    [SerializeField] private GameObject _timerObject;
    #endregion

    #region Fields
    [Header("Fields")]
    [SerializeField] private double _time;
    #endregion

    public void AddTime()
    {
        TimerController.time += _time;
        _timerObjectCopy = Instantiate(_timerObject);
        GameObject _timerObjectChild = _timerObjectCopy.transform.GetChild(0).gameObject;
        GameObject _timerText = _timerObjectCopy.transform.GetChild(1).gameObject;
        Destroy(_timerObjectCopy.GetComponent<TimerController>());
        Destroy(_timerObjectChild);

        _timerText.transform.SetPositionAndRotation(new Vector3(_timerText.transform.position.x, 460, _timerText.transform.position.z), Quaternion.identity);
        _timerText.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 18);
        _timerText.GetComponent<TextMeshProUGUI>().text = $"+ {_time}.00";

        StartCoroutine(DestroyObject(_timerText.GetComponent<TextMeshProUGUI>()));
    }

    IEnumerator DestroyObject(TextMeshProUGUI text)
    {
        yield return new WaitForSeconds(1);
        text.CrossFadeAlpha(0.0f, 0.6f, false);

        yield return new WaitForSeconds(1);
        Destroy(_timerObjectCopy);
    }
}