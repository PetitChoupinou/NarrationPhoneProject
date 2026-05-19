using TMPro;
using UnityEngine;

public class Search : MonoBehaviour
{
    [SerializeField] private TMP_Text _search;
    [SerializeField] private TMP_Text _searchResult;
    public void SetUp(string search,string text)
    {
        _search.text = search;
        _searchResult.text = text;
    }


}
