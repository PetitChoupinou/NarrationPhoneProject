using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;

public class PhotoApp : Application
{
    private Dictionary<int, GameObject> _chronologicalStorage = new Dictionary<int, GameObject>();
    [SerializeField] private GameObject _chronoStoragePrefab;
    [SerializeField] private GameObject _storagePanel;
    [SerializeField] private GameObject _photoPanel;
    [SerializeField] private GameObject _photoPrefab;
    [SerializeField] private TMP_Text _headerTxt;
    [SerializeField] private List<PhotoData> _photosData=new List<PhotoData>();

    public override void CloseCurrent()
    {
        throw new System.NotImplementedException();
    }

    private void Start()
    {
        foreach (PhotoData data in _photosData)
        {
            int date = data.day + data.month * 100 + data.year * 10000;
            if (!_chronologicalStorage.ContainsKey(date))
            {
                ChronoStorageCreation(data.day, data.month, data.year);
            }
            Transform parent = _chronologicalStorage[date].transform;
            GameObject newImage = Instantiate(_photoPrefab,parent);
            newImage.GetComponent<Photo>().Setup(data, _headerTxt, _photoPanel, _storagePanel);
        }
        SortStorage();
    }
    public override void SetUp(List<CharacterSheet> characters)
    {
        foreach(PhotoData data in _photosData)
        {
            int key = data.day + data.month * 100 + data.year * 10000;
            if (!_chronologicalStorage.ContainsKey(key))
            {
                ChronoStorageCreation(data.day, data.month, data.year);
            }
        }
    }
    public void ChronoStorageCreation(int day, int month ,int year)
    {
        DateTime storageDate = new DateTime(year, month, day);
        string monthString = storageDate.ToString("MMMM", CultureInfo.CurrentCulture);
        GameObject newStorage = Instantiate(_chronoStoragePrefab, _storagePanel.transform);
        PhotoStorage newStorageData= newStorage.GetComponent<PhotoStorage>();
        newStorageData.Setup(day, month, year);
        newStorage.name = year + "/" + month + "/" + day;
        newStorage.transform.GetChild(0).GetComponent<TMP_Text>().text= day + " " + monthString + " " + year;
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
        }
    }
}
[Serializable]
public struct PhotoData// temporary test use
{
    public Sprite image;
    public int year;
    [Range(1, 12)] public int month;
    [Range(1, 31)] public int day;
    [Range(0, 23)] public int hour;
    [Range(0, 59)] public int mintute;
    }
