using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InternetApp : Application
{
    [SerializeField] TMP_Text _headerTxt;
    [SerializeField] GameObject _basePage;
    [SerializeField] GameObject _basePageContent;
    [SerializeField] GameObject _searchPrefab;
    public override void CloseCurrent()
    {
        throw new System.NotImplementedException();
    }

    public override void SetUp(StoryAppSetup setup)
    {
        foreach(InternetSerach s in setup.InternetSeraches)
        {
            GameObject search = Instantiate(_searchPrefab, _basePageContent.transform);
            search.GetComponent<Search>().SetUp(s.search, s.text);
        }
    }
    public void newPage()
    {

    }
}
