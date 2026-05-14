using UnityEngine;
using UnityEngine.UI;

public class AppButton : MonoBehaviour
{
    private ApplicationType _type;
    [SerializeField]private Image _logo;
    private string ID;
    [SerializeField] private GameObject notif;

    public ApplicationType Type { get => _type; set => _type = value; }

    public void Pressed()
    {
        PhoneManager.Instance.GetInApp();
        AppManager.Instance.OpenApp(_type);
        if (notif.activeSelf)
        {
            notif.SetActive(false);
        }
    }
    public void SetNotifUp()
    {
        notif.SetActive(true);
    }
    private void Start()
    {
        _logo.sprite = AppManager.Instance.GetApplication(_type).Logo;
    }
}
