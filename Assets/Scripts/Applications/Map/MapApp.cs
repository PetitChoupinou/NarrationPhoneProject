using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapApp : BaseApplication
{
    [SerializeField] private GameObject _mapContent;
    [SerializeField] private MapUI _mapUI;
    private List<MapLocation> _locations = new List<MapLocation>();

    public List<MapLocation> Locations { get => _locations;}

    public void OnActivated()
    {

        _mapUI.GetBounds(_canvas);
    }


    public override void CloseCurrent()
    {
        return;
    }

    public override void SetUp(StoryAppSetup setup)
    {
        _mapUI.Setup(setup.Map,setup.Locations);
    }
    public void AddLocation(MapLocation loc)
    {
        _locations.Add(loc);
    }
    public void SetPhotoHasBeenTaken(string locationName)
    {
        MapLocation foundLocation = _locations.Find(x => x.Data.locationName == locationName);
        if( foundLocation != null)
        {
            LocationData newLocation = new LocationData()
            {
                locationName = foundLocation.Data.locationName,
                image = foundLocation.Data.image,
                coordinates = foundLocation.Data.coordinates,
                photo = foundLocation.Data.photo,
                hasPhotoBeenTaken = true,
                networkState = foundLocation.Data.networkState,
            };
            foundLocation.Data = newLocation;
        }
    }
}
