using TMPro;
using UnityEngine;


public class Battery : MonoBehaviour
{
    [SerializeField]TMP_Text _text;
    EnergyManager _energyManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _energyManager = EnergyManager.Instance;
        _energyManager.EnergyChange.AddListener(UpdateEnergyUI);
    }

    // Update is called once per frame
    void UpdateEnergyUI()
    {
        _text.text =_energyManager.CurrentEnergy+"%"; 
    }
}
