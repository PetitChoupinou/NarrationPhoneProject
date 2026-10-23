using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DarkModeManager : MonoBehaviour
{

  

    List<TextMeshProUGUI> _darkModeTexts = new List<TextMeshProUGUI>();
    List<Image> _darkModeElements = new List<Image>();


    [Header("Light Mode")]
    [SerializeField] Color _lightModeTextColor;
    [SerializeField] Color _lightModeBackgroundColor;
    [SerializeField] Color _lightModeBackground2Color;
    [SerializeField] Color _lightModePanelColor;
    [SerializeField] Color _lightModeButtonColor;

    [Header("Dark Mode")]
    [SerializeField] Color _darkModeTextColor;
    [SerializeField] Color _darkModeBackgroundColor;
    [SerializeField] Color _darkModeBackground2Color;
    [SerializeField] Color _darkModePanelColor;
    [SerializeField] Color _darkModeButtonColor;
    [SerializeField] Color _inversedDarkModeBackgroundColor;
    [SerializeField] Color _inversedLightModeBackgroundColor;


    bool _isDarkMode;

    public static DarkModeManager Instance;
    

    public bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            _isDarkMode = value;
            SaveManager.Instance.Save.optionsData.isDarkMode = value;
            SaveManager.Instance.SavePlayerData();
        }
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        var savedData = SaveManager.Instance.Save;
        _isDarkMode = savedData.optionsData.isDarkMode;
        ApplyDarkMode();
    }

    IEnumerator Startup()
    {
        //yield return new WaitForSeconds(0.05f);
        yield return new WaitForEndOfFrame();
        GetAllObjects();
        SwitchMode(_isDarkMode);
        yield return null;

    }
    

    void GetAllObjects()
    {
        
        _darkModeTexts.Clear();
        var tempTexts = GameObject.FindGameObjectsWithTag("DarkModeText");
        
        foreach (var text in tempTexts)
        {
            
            _darkModeTexts.Add(text.GetComponent<TextMeshProUGUI>());
        }

       
        _darkModeElements.Clear();
        var tempElements = new List<GameObject>();
        tempElements.AddRange(GameObject.FindGameObjectsWithTag("DarkModeBackground"));
        tempElements.AddRange(GameObject.FindGameObjectsWithTag("DarkModeBackground_2"));
        tempElements.AddRange(GameObject.FindGameObjectsWithTag("DarkModePanel"));
        tempElements.AddRange(GameObject.FindGameObjectsWithTag("DarkModeButton"));
        
        foreach(var element in tempElements)
        {
            if (element != null || element.activeInHierarchy)
                _darkModeElements.Add(element.GetComponent<Image>());
        }

    }


    public void ApplyDarkMode(bool shouldWait = false)
    {
        if(shouldWait) StartCoroutine(Startup());
        else
        {
            GetAllObjects();
            SwitchMode(_isDarkMode);
        }
    }

    public void SwitchMode(bool toDarkMode)
    {
        IsDarkMode = toDarkMode;

        for(int i = 0; i < _darkModeTexts.Count; i++)
        {
            if (toDarkMode) _darkModeTexts[i].color = _darkModeTextColor;
            else _darkModeTexts[i].color = _lightModeTextColor;
        }
        Color objectColor = Color.white;
        for (int i = 0; i < _darkModeElements.Count; i++)
        {
            if (_darkModeElements[i] == null) continue;
            if (_darkModeElements[i].gameObject != null || _darkModeElements[i].gameObject.activeInHierarchy)
            {
                switch (_darkModeElements[i].gameObject.tag)
                {
                    case "DarkModeBackground":
                        objectColor = _isDarkMode ? _darkModeBackgroundColor : _lightModeBackgroundColor;
                        break;
                    case "DarkModeBackground_2":
                        objectColor = _isDarkMode ? _darkModeBackground2Color : _lightModeBackground2Color;
                        break;
                    case "DarkModePanel":
                        objectColor = _isDarkMode ? _darkModePanelColor : _lightModePanelColor;
                        break;
                    case "DarkModeButton":
                        objectColor = _isDarkMode ? _darkModeButtonColor : _lightModeButtonColor;
                        break;
                }
                _darkModeElements[i].color = objectColor;
            }
           
            
        }

        
    }

    public void SwitchTextToDarkMode(TextMeshProUGUI text)
    {
        if (_isDarkMode) text.color = _darkModeTextColor;
        else text.color = _lightModeTextColor;
    }

    public void SwitchObjectToDarkMode(Image image)
    {
        if (_isDarkMode) image.color = _darkModeBackgroundColor;
        else image.color = _lightModeBackgroundColor;
    }

    public void SwitchImageToInversedDarkMode(Image image)
    {
        if (_isDarkMode) image.color = _inversedDarkModeBackgroundColor;
        else image.color = _inversedLightModeBackgroundColor;
    }

    public void SwitchButtonToDarkMode(GameObject button)
    {
        Image image = button.GetComponent<Image>();
        
        if (_isDarkMode) image.color = _darkModeButtonColor;
        else image.color = _lightModeButtonColor;

        var texts = button.GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < texts.Length; i++)
        {
            SwitchTextToDarkMode(texts[i]);
        }        
    }

    public void SwitchPanelToDarkMode(Image image)
    {
        if (_isDarkMode) image.color = _darkModePanelColor;
        else image.color = _lightModePanelColor;
    }
}
