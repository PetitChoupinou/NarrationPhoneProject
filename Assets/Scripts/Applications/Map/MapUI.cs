using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform _rectTransform;
    private Vector2 _originPointerPosition;
    private Vector2 _originMapPosition;
    private Vector2[] _boundsMap;
    private Vector2 _basePosition;

    [SerializeField] Image _mapTexture;
    [SerializeField] GameObject _locationPrefab;
    [SerializeField] RectTransform _centerScreen;
    [SerializeField] GameObject _debugPoint;

    GameObject pointMin;
    GameObject pointMax;
    GameObject pointPos;

    private void OnEnable()
    {

        /*pointMin = Instantiate(_debugPoint, transform.parent.transform);
        pointMin.name = "Min Point";
        pointMax = Instantiate(_debugPoint, transform.parent.transform);
        pointMax.name = "Max Point";
        pointPos = Instantiate(_debugPoint, transform.parent.transform);
        pointPos.name = "Pos Point";*/
        _rectTransform = GetComponent<RectTransform>();
        _basePosition = _rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originPointerPosition = eventData.position;
        _originMapPosition = _rectTransform.anchoredPosition;

    }

    public void OnDrag(PointerEventData eventData)
    {
        var drag = eventData.position - _originPointerPosition;
        Vector2 newPosition = _originMapPosition + drag;
        Vector2 clampedPosition = new Vector2(
            Mathf.Clamp(newPosition.x, _boundsMap[0].x, _boundsMap[1].x),
            Mathf.Clamp(newPosition.y, _boundsMap[0].y, _boundsMap[1].y));
        _rectTransform.anchoredPosition = clampedPosition;
        /*pointPos.transform.position = clampedPosition;
        Debug.Log($"{_boundsMap[0].x} < {clampedPosition.x} < {_boundsMap[1].x} \n {_boundsMap[0].y} < {clampedPosition.y} < {_boundsMap[1].y}");*/
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public MapLocation CreateLocation(LocationData data)
    {
        GameObject newLocation = Instantiate(_locationPrefab, transform);
        newLocation.GetComponent<RectTransform>().anchoredPosition = new Vector2(data.coordinates.x, data.coordinates.y);
        var mapLocation = newLocation.GetComponent<MapLocation>();
        mapLocation.SetupInstantiate(data);
        return mapLocation;
    }

    public MapLocation[] GetExistingLocations()
    {
        var locations = GetComponentsInChildren<MapLocation>(true);
        foreach (MapLocation location in locations)
        {
            location.Setup();
        }
        return locations;
    }

    public void GetBounds(Canvas parentCanvas)
    {
        _rectTransform.anchoredPosition = _basePosition;
        Vector3 factorScale = parentCanvas.GetComponent<RectTransform>().localScale;
        if (factorScale.x == 0 || factorScale.y == 0)
        {
            factorScale = Vector3.one;
        }
        var screenWidth = Screen.width / factorScale.x;
        var screenHeight = Screen.height / factorScale.y;
        Vector2[] _screenBounds = new Vector2[2]
        {
            new Vector2(_centerScreen.anchoredPosition.x - screenWidth / 2, _centerScreen.anchoredPosition.y - screenHeight / 2),
            new Vector2(_centerScreen.anchoredPosition.x + screenWidth / 2, _centerScreen.anchoredPosition.y + screenHeight / 2)
        };
        /*        pointMin.GetComponent<RectTransform>().anchoredPosition = _screenBounds[0];
                pointMax.GetComponent<RectTransform>().anchoredPosition = _screenBounds[1];*/
        RectTransform rect = GetComponent<RectTransform>();
        Vector2 centerTexture = new Vector2(rect.position.x, rect.position.y);
        var width = _mapTexture.rectTransform.rect.width / factorScale.x;
        var height = _mapTexture.rectTransform.rect.height / factorScale.y;

        _boundsMap = new Vector2[2]
        {
            new Vector2(_screenBounds[1].x - width / 2, _screenBounds[1].y - height / 2),
            new Vector2(_screenBounds[0].x + width / 2, _screenBounds[0].y + height / 2),
        };
        /*        pointMin.GetComponent<RectTransform>().anchoredPosition = _boundsMap[0];
                pointMax.GetComponent<RectTransform>().anchoredPosition = _boundsMap[1];*/


    }

    /*public LocationData[] GetLocationsData()
    {
        var locations = GetExistingLocations();
        LocationData[] datas = new LocationData[locations.Length];

        for (int i = 0; i < locations.Length; i++)
        {
            datas[i] = locations[i].GetData();
        }
        return datas;
    }*/
}
