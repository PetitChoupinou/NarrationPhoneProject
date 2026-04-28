using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhoneApp : Application
{
    List<PhoneNumbers> _numbers=new List<PhoneNumbers>();
    [SerializeField] private TMP_Text _numDisplay;
    private string _currentNum="";

    public override void CloseCurrent()
    {
        throw new System.NotImplementedException();
    }

    public override void SetUp(StoryAppSetup setup)
    {
        foreach(CharacterSheet c in setup.Characters)
        {
            _numbers.Add(c.TelNum);
        }
        foreach(PhoneNumbers n in setup.PhoneNumbers)
        {
            _numbers.Add(n);
        }
    }
    public void AddToCurrentNbr(string x)
    {
        _currentNum += x;
        UpdateDisplay();
    }
    public void DelLastDigit()
    {
        _currentNum.Remove(_currentNum.Length - 1);
        UpdateDisplay();
    }
    private void UpdateDisplay()
    {
        _numDisplay.text = _currentNum;
    }
    public void Call()
    {
        if (_numbers.Exists(x=>x.numbers==_currentNum))
        {
            //do stuff
            print("calling " + _numbers.Find(x => x.numbers == _currentNum).title);
        }
        _currentNum = "";
        UpdateDisplay() ;
    }
}
