using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DarkModeManager : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> _darkModeTexts = new List<TextMeshProUGUI>();
    [SerializeField] List<Image> _darkModeBackgrounds = new List<Image>();
    [SerializeField] List<Image> _darkModeBackgroundsBlackAndWhite = new List<Image>();

    [Header("Light Mode")]
    [SerializeField] Color _lightModeTextColor;
    [SerializeField] Color _lightModeBackgroundColor;

    [Header("Dark Mode")]
    [SerializeField] Color _darkModeTextColor;
    [SerializeField] Color _darkModeBackgroundColor;

    bool _isDarkMode;

    public static DarkModeManager Instance;

    public bool IsDarkMode { get => _isDarkMode; set => _isDarkMode = value; }

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
        StartCoroutine(Startup());
    }

    IEnumerator Startup()
    {
        yield return new WaitForSeconds(0.05f);

        GetAllObjects();
        SwitchMode(true);
        yield return null;
    }
    

    void GetAllObjects()
    {
        _darkModeTexts.Clear();
        var tempTexts = GameObject.FindGameObjectsWithTag("DarkModeText");
        foreach (var text in tempTexts)
        {
            if (text.GetComponent<TextMeshProUGUI>() == null)
            {
                print(text.name + " in " + text.transform.parent.name);
            }
            _darkModeTexts.Add(text.GetComponent<TextMeshProUGUI>());
        }

        _darkModeBackgrounds.Clear();
        var tempImages = GameObject.FindGameObjectsWithTag("DarkModeBackground");
        foreach (var image in tempImages)
        {
            _darkModeBackgrounds.Add(image.GetComponent<Image>());
        }

        _darkModeBackgroundsBlackAndWhite.Clear();
        var tempImagesBW = GameObject.FindGameObjectsWithTag("DarkModeBackgroundBlackWhite");
        foreach (var image in tempImagesBW)
        {
            _darkModeBackgroundsBlackAndWhite.Add(image.GetComponent<Image>());
        }

    }

    public void SwitchMode(bool toDarkMode)
    {
        _isDarkMode = toDarkMode;
        for(int i = 0; i < _darkModeTexts.Count; i++)
        {
            
            if (toDarkMode) _darkModeTexts[i].color = _darkModeTextColor;
            else _darkModeTexts[i].color = _lightModeTextColor;
        }

        for (int i = 0; i < _darkModeBackgrounds.Count; i++)
        {
            if (toDarkMode) _darkModeBackgrounds[i].color = _darkModeBackgroundColor;
            else _darkModeBackgrounds[i].color = _lightModeBackgroundColor;
        }

        for (int i = 0; i < _darkModeBackgroundsBlackAndWhite.Count; i++)
        {
            if (toDarkMode) _darkModeBackgroundsBlackAndWhite[i].color = Color.black;
            else _darkModeBackgroundsBlackAndWhite[i].color = Color.white;
        }
    }

    public void SwitchObjectToDarkMode(TextMeshProUGUI text)
    {
        if (_darkModeTexts.Contains(text)) Debug.Log("Contains text");
        if (_isDarkMode) text.color = _darkModeTextColor;
        else text.color = _lightModeTextColor;
    }

    public void SwitchObjectToDarkMode(Image image)
    {
        if (_darkModeBackgrounds.Contains(image)) Debug.Log("Contains image");
        if (_isDarkMode) image.color = _darkModeBackgroundColor;
        else image.color = _lightModeBackgroundColor;
    }
}
