using UnityEngine;
using UnityEngine.UIElements;

public class MapLocation : MonoBehaviour
{
    [SerializeField] MapLocationsInfos _infos;
    bool _isInfosActive;
    [SerializeField] bool isMainLocation;
    float _scale;
    [SerializeField] LocationData _data;

    public LocationData Data { get => _data; set => _data = value; }

    public void SetupInstantiate(LocationData data ,float scale)
    {
        _data = data;
        _scale=scale;
        Setup();
    }

    public void Setup()
    {
        transform.localPosition = (_data.coordinates+new Vector2(Data.image.rect.width/2,-Data.image.rect.height/2))*_scale;
        isMainLocation = Data.isStartLocation;
        _infos.SetupInfos(_data,_scale);
        if (isMainLocation)
        {
            PhoneManager.Instance.CurrentLocation = _data;
        }
    }
    

    public LocationData GetData()
    {
        return _infos.GetData();
    }


    public void GoToLocation()
    {
        PhoneManager.Instance.ChangeLocation(_data);
    }
}


