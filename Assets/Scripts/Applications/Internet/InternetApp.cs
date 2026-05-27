using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InternetApp : BaseApplication
{
    [SerializeField] TMP_Text _headerTxt;
    [SerializeField] GameObject _basePage;
    [SerializeField] GameObject _basePageContent;
    [SerializeField] GameObject _searchPrefab;
    [SerializeField] GameObject _DlPage;
    bool _isDlPage;
    StoryAppSetup _setup;
    public override void CloseCurrent()
    {
        if (_isDlPage)
        {
            _basePage.SetActive(true);
            _DlPage.SetActive(false);
            _isDlPage=false;
        }
    }

    public override void SetUp(StoryAppSetup setup)
    {
        _setup = setup;
        foreach(InternetSerach s in _setup.InternetSeraches)
        {
            GameObject search = Instantiate(_searchPrefab, _basePageContent.transform);
            search.GetComponent<Search>().SetUp(s.search, s.text);
        }
    }
    public void setDlPage(string url,ApplicationType type)
    {
        _isDlPage = true;
        _DlPage.GetComponent<DlPage>().Setup(_headerTxt, url, type, _setup);
    }

    internal void OnActivated()
    {
        if (_isDlPage)
        {
            PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.inApp);
            _basePage.SetActive(false);
            _DlPage.SetActive(true);
        }
        else
        {
            PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.app);
            _basePage.SetActive(true);
            _DlPage.SetActive(false);
        }
    }
}
