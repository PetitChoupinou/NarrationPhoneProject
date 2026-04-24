using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapApp : Application
{
    [SerializeField] private GameObject _mapContent;
    [SerializeField] private MapUI _mapUI;
    public List<MapLocation> locations = new List<MapLocation>();
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
        locations.Clear();
        
        locations.AddRange(_mapUI.GetExistingLocations());
        

        foreach (LocationData location in setup.Locations)
        {
            MapLocation newLocation = _mapUI.CreateLocation(location);
            locations.Add(newLocation);
        }
    }
}
