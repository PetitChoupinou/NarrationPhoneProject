using UnityEngine;

public class PhotoStorage : MonoBehaviour
{
    int _value;
   
    public int Value { get => _value; }
    public void Setup(int d,int m, int y)
    {
        _value = d + 100 * m + 10000 * y;
    }
}
