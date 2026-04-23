using UnityEngine;

public class MapLocation : MonoBehaviour
{
    [SerializeField] GameObject _infos;
    bool _isInfosActive;

    public void ToggleInfos()
    {
        Debug.Log("ToggleInfos");
        _isInfosActive = !_isInfosActive;
        _infos.SetActive(_isInfosActive);
    }

    
}
