using UnityEngine;

public class PhotoStorage : MonoBehaviour
{
    int _value;
    [SerializeField] GameObject _panelPhoto;
   
    public int Value { get => _value; }
    public GameObject PanelPhoto { get => _panelPhoto;}

    public void Setup(int d,int m, int y)
    {
        _value = d + 100 * m + 10000 * y;
    }
}
