using UnityEngine;

public class MapLocation : MonoBehaviour
{
    [SerializeField] MapLocationsInfos _infos;
    bool _isInfosActive;
    [SerializeField] bool isMainLocation;
    [SerializeField] LocationData _data;

    public void SetupInstantiate(LocationData data)
    {
        _data = data;
        Setup();

    }

    public void Setup()
    {
        _infos.SetupInfos(_data);
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


