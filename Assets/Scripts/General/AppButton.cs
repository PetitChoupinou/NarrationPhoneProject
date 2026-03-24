using UnityEngine;

public class AppButton : MonoBehaviour
{
    private ApplicationType _type;

    public ApplicationType Type { get => _type; set => _type = value; }

    public void Pressed()
    {
        PhoneManager.Instance.GetInApp();
        AppManager.Instance.OpenApp(_type);
    }
}
