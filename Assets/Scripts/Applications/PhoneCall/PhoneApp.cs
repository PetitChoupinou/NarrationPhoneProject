using System.Collections;
using System.Collections.Generic;
using TCG.Core.Dialogues;
using TMPro;
using UnityEngine;

public class PhoneApp : BaseApplication
{
    List<PhoneNumbers> _numbers=new List<PhoneNumbers>();
    [SerializeField] private TMP_Text _numDisplay;
    [SerializeField] private UITextTyper _textDisplay;
    private string _currentNum="";
    private SoundManager _soundManager;
    private PhoneManager _phoneManager;

    public override void CloseCurrent()
    {
        
    }

    public override void SetUp(StoryAppSetup setup)
    {
        _soundManager = SoundManager.instance;
        foreach(CharacterSheet c in setup.Characters)
        {
            print(c);
            _numbers.Add(c.TelNum);
        }
        foreach(PhoneNumbers n in setup.PhoneNumbers)
        {
            print(n);
            _numbers.Add(n);
        }
        _phoneManager = PhoneManager.Instance;
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
        if (AppManager.Instance.GetApplication(ApplicationType.Map)&& _phoneManager.CurrentLocation.networkState != NetworkState.Good)
        {
            _phoneManager.CreateThought("Hmm pas de réseaux.");
            _currentNum = "";
            UpdateDisplay();
            return;
        }
        if (!_numbers.Exists(x => x.numbers == _currentNum))
        {
            _phoneManager.CreateThought("Mauvais numéro ...");
            _currentNum = "";
            UpdateDisplay();
            return;
        }
        PhoneNumbers calledNumber = _numbers.Find(x => x.numbers == _currentNum);
        print("calling " + calledNumber.title);
        StartCoroutine(Call(calledNumber));
    }

     IEnumerator Call(PhoneNumbers x) 
    {
        yield return null;
        print(x.callText.Count);
        for (int i=0;i<x.callText.Count;i++)
        {
            _soundManager.PlaySound(x.callAudio.name);
            _textDisplay.ReadText(x.callText[i]);
            yield return new WaitForSeconds(x.callText[i].Length/ (float)_textDisplay.CharactersPerSecond + 1.0f);
            _soundManager.StopSound(x.callAudio.name);
            yield return null;
        }
        _currentNum = "";
        UpdateDisplay();
        yield return null;
        if (x.title == "Police")
        {
            Application.Quit();
            yield return null;
        }
    }
}
