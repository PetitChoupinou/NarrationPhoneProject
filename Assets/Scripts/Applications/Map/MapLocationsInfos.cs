using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapLocationsInfos : MonoBehaviour
{

    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private NetworkState _networkS;


    public void SetupInfos(LocationData data)
    {
        _image.sprite = data.image;
        _title.text = data.locationName;
        _networkS = data.networkState;
    }

    public LocationData GetData()
    {
        LocationData data = new LocationData()
        {
            image = _image.sprite,
            locationName = _title.text,
            networkState = _networkS
        };
        return data;
    }
}


