using NUnit.Framework;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class QuestManager : MonoBehaviour
{


    [SerializeField] private QuestBase[] _possibleQuests;
    [SerializeField] private int _questsNbr=3;
    [SerializeField] private QuestBase[] _selectedQuests;
    [SerializeField] UnityEvent _onEnergyUsage;
    [SerializeField] private int _resetTime=23;
    [SerializeField] private TMP_Text _titre;
    private SaveManager _saveManager;
    private PlayerSaveData _save;
    public static QuestManager Instance { get; private set; }
    public QuestBase[] PossibleQuests { get => _possibleQuests;}
    public QuestBase[] SelectedQuests { get => _selectedQuests;}

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartQuests()
    {
        _saveManager = SaveManager.instance;

        if (_questsNbr>_possibleQuests.Length)_questsNbr = _possibleQuests.Length;
        _selectedQuests=new QuestBase[_questsNbr];
        _save = _saveManager.Save;
        if(_save != null)
        {
            _titre.text = "c'est passé par là";
            DateTime lastConnection = _save.lastAppQuit.CurrentTime;
            DateTime currentTime = InternetConnection.GetNistTime();
            if ((currentTime.Day != lastConnection.Day || _save.currentQuests[0].Title == ""))
            {
                SelectQuests();
                _titre.text = "là aussi" ;
            }
            else
            {
                _selectedQuests = _save.currentQuests;
            }
            _save.currentQuests = _selectedQuests;
        }
    }
    public void SelectQuests()
    {
        if(_possibleQuests.Length > 0)
        {
            for(int i=0; i < _questsNbr; i++)
            {
                int rand =UnityEngine.Random.Range(0, _possibleQuests.Length);
                while(_selectedQuests.Contains(PossibleQuests[rand]))
                {
                    rand++;
                    if (rand == PossibleQuests.Length) rand = 0;
                }
                _selectedQuests[i] = _possibleQuests[rand];
            }
        }
    }

    public void TempAddOne()
    {
        _selectedQuests[1].UpdateValue(1);
        _save.currentQuests = _selectedQuests;
    }
}
