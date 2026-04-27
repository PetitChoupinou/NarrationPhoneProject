using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotoPreview : MonoBehaviour
{
    private Dictionary<int, GameObject> _chronologicalStorage = new Dictionary<int, GameObject>();
    private string _title;
    private bool _isLocked;
    [SerializeField] private GameObject _chronoStoragePrefab;
    [SerializeField] private GameObject _storagePanel;
    private GameObject _photoPanel;
    [SerializeField] private GameObject _photoPrefab;
    [SerializeField] private GameObject _lockedScreen;
    [SerializeField] private TMP_Text _passwordInput;
    private GameObject _returnButton;
    private Sprite _latestPhoto;
     private TMP_Text _headerTxt;
    private string _password;
    private PhotoApp app;
    public string Title { get => _title;}
    public GameObject StoragePanel { get => _storagePanel;}

    private void Awake()
    {
      app= AppManager.Instance.GetApplication(ApplicationType.Photos).GetComponent<PhotoApp>();

    }

    private void OnEnable()
    {
        if(_headerTxt)  _headerTxt.text = _title;
        app.CurrentStoragePanel = gameObject;
        PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.inApp);
        if (_isLocked)
        {
            _storagePanel.SetActive(false);
            _lockedScreen.SetActive(true);
        }
        else
        {
            _storagePanel.SetActive(true);
            _lockedScreen.SetActive(false);
        }
    }

    public  void SetUp(PhotoPreviews setup,Image photo, TMP_Text headerTxt, GameObject returnButton,GameObject photoPanel)
    {
        _title = setup.title;
        _isLocked = setup.locked;
        _photoPanel = photoPanel;
        _headerTxt = headerTxt;
        _returnButton = returnButton;
        _password = setup.password;
        List<PhotoData> photos = setup.photoDatas;
        foreach (PhotoData data in photos)
        {
            AddPhoto(data, photo);
        }
        SortStorage();
    }
    public void ChronoStorageCreation(int day, int month, int year)
    {
        DateTime storageDate = new DateTime(year, month, day);
        print(storageDate.ToString("MMMM"));
        print(CultureInfo.CurrentCulture.ToString());
        string monthStr = storageDate.ToString("MMMM");
        GameObject newStorage = Instantiate(_chronoStoragePrefab, _storagePanel.transform);
        PhotoStorage newStorageData = newStorage.GetComponent<PhotoStorage>();
        newStorageData.Setup(day, month, year);
        newStorage.name = year + "/" + month + "/" + day;
        newStorage.transform.GetChild(0).GetComponent<TMP_Text>().text = day + " " + monthStr + " " + year;
        _chronologicalStorage.Add(newStorageData.Value, newStorage);
    }
    private void SortStorage()
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in _storagePanel.transform)
            children.Add(child);
        children = children.OrderBy(o => o.GetComponent<PhotoStorage>().Value).Reverse().ToList();

        foreach (Transform child in children)
        {
            child.SetParent(null);
        }

        foreach (Transform child in children)
        {
            child.SetParent(_storagePanel.transform);
            child.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }
    public void Unlock()
    {
        string value = _passwordInput.text.TrimEnd(new char[] { '\r', ' ' , (char)8203 });
        print(value.Length+" : " + _password.Length);
        if (value  == _password)
        {
            _isLocked = false;
            _storagePanel.SetActive(true);
            _lockedScreen.SetActive(false);
        }
    }
    public void AddPhoto(PhotoData data,Image photo)
    {
        int date = data.day + data.month * 100 + data.year * 10000;
        if (!_chronologicalStorage.ContainsKey(date))
        {
            ChronoStorageCreation(data.day, data.month, data.year);
        }
        Transform parent = _chronologicalStorage[date].GetComponent<PhotoStorage>().PanelPhoto.transform;
        GameObject newImage = Instantiate(_photoPrefab, parent);
        newImage.GetComponent<Photo>().Setup(data, _headerTxt, _photoPanel, _storagePanel, photo, _returnButton);
        _latestPhoto = data.image;
    }
}
