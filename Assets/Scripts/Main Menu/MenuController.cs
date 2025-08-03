using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject _menuUI;
    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private GameObject _creditsUI;
    [SerializeField] private GameObject _fmodUI;
    [SerializeField] private GameObject _loadingUI;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _driftKeybind;

    [Header("Input State")]
    [SerializeField] private double _defaultChangeSpeed = 5;
    [SerializeField] private float _defaultChangeSpeedOfLetter = 0.25f;

    [Header("State")]
    [SerializeField] private double _changeSpeed;
    [SerializeField] private bool _changeCredit = false;
    [SerializeField] private bool _isSettingsActive = false;
    //[SerializeField] private bool _isKeybindActive = false;
    [SerializeField] private bool _isFirstLoad = true;
    //[SerializeField] private bool _isLoading = true;

    private int _numberOfObjects;
    private int _count = 1;
    private int _previousCount = 0;
    private List<GameObject> _objectsArray = new List<GameObject>();

    private void Start()
    {
        _changeSpeed = _defaultChangeSpeed;
        _numberOfObjects = _creditsUI.transform.childCount;
        _driftKeybind.text = KartController.driftKey.ToString();

        while (_count < _numberOfObjects)
        {
            _objectsArray.Add(_creditsUI.transform.GetChild(_count).gameObject);
            _count++;
        }

        _count = 0;
    }

    private void Update()
    {
        if (_menuUI.activeSelf)
        {
            if (!_changeCredit && !_isFirstLoad)
                _changeSpeed -= Time.deltaTime;

            if (_changeSpeed <= 0 && !_changeCredit || _isFirstLoad)
            {
                _changeCredit = true;
                StartCoroutine(ChangingTitle());
            }

            if (_isFirstLoad)
                _isFirstLoad = false;
        }

        //if (_isLoading)
        //{
        //    _isLoading = false;
        //    StartCoroutine(Loading());
        //}

        //if (_isSettingsActive && _isKeybindActive)
        //{
        //    if (Input.GetKeyDown(KeyCode.Escape))
        //    {
        //        _isKeybindActive = false;
        //        _driftKeybind.text = KartController.driftKey.ToString();
        //    }
        //    else
        //    {
        //        _isKeybindActive = false;
        //        KartController.driftKey = Input.;
        //        _driftKeybind.text = KartController.driftKey.ToString();
        //    }
        //}
    }

    //IEnumerator Loading()
    //{
    //    yield return new WaitForSeconds(5f);
    //    _loadingUI.transform.GetChild(0).gameObject.SetActive(false);
    //    _loadingUI.transform.GetChild(1).gameObject.SetActive(true);
    //    yield return new WaitForSeconds(5f);
    //    _loadingUI.transform.GetChild(1).gameObject.SetActive(false);
    //    _menuUI.SetActive(true);
    //    _creditsUI.SetActive(true);
    //    _fmodUI.SetActive(true);

    //    _isFirstLoad = true;
    //}

    IEnumerator ChangingTitle()
    {
        if (_previousCount != _count)
            _objectsArray[_previousCount].SetActive(false);

        string objectName = _objectsArray[_count].name;
        int nameLength = objectName.Length;
        int charCount = 1;

        _title.text = objectName[0].ToString();

        while(charCount < nameLength)
        {
            yield return new WaitForSeconds(_defaultChangeSpeedOfLetter);

            _title.text += objectName[charCount].ToString();
            charCount++;
        }

        StartCoroutine(ChangingCredit());
    }

    IEnumerator ChangingCredit()
    {
        yield return null;
        _objectsArray[_count].SetActive(true);

        _changeCredit = false;
        _changeSpeed = _defaultChangeSpeed;

        if (_count < _numberOfObjects - 2)
        {
            _count++;
            _previousCount = _count - 1;
        }
        else
        {
            _count = 0;
            _previousCount = _numberOfObjects - 2;
        }
    }

    public void Settings()
    {
        _isSettingsActive = !_isSettingsActive;

        if (_isSettingsActive)
        {
            _menuUI.SetActive(false);
            _creditsUI.SetActive(false);
            _fmodUI.SetActive(false);
            _settingsUI.SetActive(true);
        }
        else
        {
            _menuUI.SetActive(true);
            _creditsUI.SetActive(true);
            _fmodUI.SetActive(true);
            _settingsUI.SetActive(false);
        }
    }

    public void Play()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void ReturnToMenu()
    {
        if (GameManager.isPaused)
            Time.timeScale = 1f;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
