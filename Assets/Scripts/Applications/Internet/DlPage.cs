using TMPro;
using UnityEngine;

public class DlPage : MonoBehaviour
{
    private ApplicationType _type;
    private PhoneManager _phone;
    private void Start()
    {
        _phone = PhoneManager.Instance;
    }
    public void Setup(TMP_Text header, string url, ApplicationType type, StoryAppSetup setup)
    {
        header.text = url;
        _type = type;
    }

    public void OnDL()
    {
        _phone.AddApplication(_phone.lockedApps[_type]);
        _phone.lockedApps.Remove(_type);
    }
}
