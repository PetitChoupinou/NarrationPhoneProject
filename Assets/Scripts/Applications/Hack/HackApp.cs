using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HackApp : BaseApplication
{
    private GameObject _currentFolder;
    private PhoneManager _phoneManager;
    private HackSetup _hackSetup;
    private string _name="";
    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject _returnButton;
    [SerializeField] private Image _image;
    [SerializeField] private  TMP_Text _headerTxt;
    [SerializeField] private GameObject _folderButtonPrefab;
    [SerializeField] private GameObject _folderPrefab;

    public GameObject CurrentFolder { get => _currentFolder; set => _currentFolder = value; }

    public override void CloseCurrent()
    {
        if (CurrentFolder == null) return;
        if (_phoneManager.CurrentDepth == PhoneManager.AppDepth.deep)
        {
            _currentFolder.GetComponent<HackFolder>().CloseCurrent();
        }
        else if(_phoneManager.CurrentDepth == PhoneManager.AppDepth.inApp)
        {
            _currentFolder.SetActive(false);
            _content.SetActive(true);
            _phoneManager.ChangeDepth(PhoneManager.AppDepth.app);
            _currentFolder = null;
            _headerTxt.text = _name;
        }
    }

    public override void SetUp(StoryAppSetup setup)
    {
        _phoneManager = PhoneManager.Instance;
        _hackSetup = setup.HackAppSetup;
        _name = _hackSetup.title;
        foreach (HackFolderSetup folderSetup in _hackSetup.folders) 
        {
            if (!folderSetup.isHackedFromStart) continue;
            AddFolder(folderSetup.title);
        }
    }

    public void AddFolder(string name)
    {
        Debug.Log("Miaou");
        HackFolderSetup folderSetup = _hackSetup.folders.Find(x => x.title == name);
        if (folderSetup == null) return;
        GameObject button = Instantiate(_folderButtonPrefab, _content.transform);
        GameObject folder = Instantiate(_folderPrefab, transform);
        button.GetComponent<InAppButton>().SetUp(folderSetup.title, folder, _returnButton);
        folder.GetComponent<HackFolder>().Setup(folderSetup, _image, _headerTxt, _returnButton);
        folder.SetActive(false);
    }
}
