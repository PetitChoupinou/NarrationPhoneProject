using UnityEngine;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using TMPro;

public class StoryShareManager : MonoBehaviour
{

    [SerializeField] List<StoryAppSetup> datasTest = new List<StoryAppSetup>();
    [SerializeField] TextMeshProUGUI updateText;

    private void Awake()
    {
        ConnectionToServices();
    }

    async void ConnectionToServices()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateData()
    {
        //Peut etre add une date + infos de publication dans la data?
        var randData = datasTest[Random.Range(0, datasTest.Count)];
        string json = JsonUtility.ToJson(randData);
        var data = new Dictionary<string, object> { { randData.name, json } };
        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
    }


    public async void Refresh()
    {
        var datas = await CloudSaveService.Instance.Data.Player.LoadAllAsync();
        updateText.text = $"There are actually {datas.Count} stories online.";
    }
}
