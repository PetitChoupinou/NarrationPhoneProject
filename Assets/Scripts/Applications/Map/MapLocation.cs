using UnityEngine;

public class MapLocation : MonoBehaviour
{
    [SerializeField] MapLocationsInfos _infos;
    bool _isInfosActive;
    [SerializeField] LocationData _data;

    public void SetupInstantiate(LocationData data)
    {
        _infos.SetupInfos(data);
        _data = data;

    }

    public void Setup()
    {
        _data = GetData();
    }
    

    public LocationData GetData()
    {

        return _infos.GetData();
    }


    public void GoToLocation()
    {
        
        Debug.Log($"Go to {_data.locationName}");
        PhoneManager.Instance.CurrentLocation =_data;
    }



}


