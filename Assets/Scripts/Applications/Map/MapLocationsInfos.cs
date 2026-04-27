using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapLocationsInfos : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _title;

    public void SetupInfos(LocationData data)
    {
        _image.sprite = data.image;
        _title.text = data.locationName;
    }

    public LocationData GetData()
    {
        LocationData data = new LocationData()
        {
            image = _image.sprite,
            locationName = _title.text
        };
        return data;
    }
}


