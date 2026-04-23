using UnityEngine;

public class MapApp : Application
{
    [SerializeField] private GameObject _mapContent;
    [SerializeField] private MapUI _mapUI;

    public void OnActivated()
    {

        _mapUI.GetBounds(_canvas);
    }


    public override void CloseCurrent()
    {

    }

    public override void SetUp(StoryAppSetup setup)
    {
        
    }

    public float dragSpeed = 2;
    private Vector3 dragOrigin;

    


    
}
