using TMPro;
using UnityEngine;


public class Battery : MonoBehaviour
{
    [SerializeField]TMP_Text _text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _text.text =SystemInfo.batteryLevel*100+"%"; 
    }
}
