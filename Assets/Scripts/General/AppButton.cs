using UnityEngine;

public class AppButton : MonoBehaviour
{
    private ApplicationType _type;
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
}
