using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HackApp : BaseApplication
{
    private PhoneManager _phoneManager;
    private HackSetup _hackSetup;
    private string _name="";
    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject _returnButton;
    [SerializeField] private Image _image;
    [SerializeField] private  TMP_Text _headerTxt;
    [SerializeField] private GameObject _folderButtonPrefab;
    [SerializeField] private GameObject _folderPrefab;
    [SerializeField] private GameObject _imagePanel;
    public override void CloseCurrent()
    {
         if(_phoneManager.CurrentDepth == PhoneManager.AppDepth.inApp)
        {
            _phoneManager.ChangeDepth(PhoneManager.AppDepth.app);
            _imagePanel.SetActive(false);
            _headerTxt.text = _name;
            _content.SetActive(true);
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
        HackFolderSetup folderSetup = _hackSetup.folders.Find(x => x.title == name);
        if (folderSetup == null) return;
        GameObject button = Instantiate(_folderButtonPrefab, _content.transform);
        //GameObject folder = Instantiate(_folderPrefab, transform);
        button.GetComponent<HackButton>().SetUp(folderSetup.title, _returnButton,_image,folderSetup.spPhoto,_imagePanel,_headerTxt);
        //folder.GetComponent<HackFolder>().Setup(folderSetup, _image, _headerTxt, _returnButton);
        //folder.SetActive(false);
    }
}
