using TCG.Core.Dialogues;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageChoice : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] UITextTyperMsg  textTyper;
    public string value;

    private void Awake()
    {
        CheckForEnergy(EnergyManager.Instance.CurrentEnergy);
        EnergyManager.Instance.OnEnergyChange += CheckForEnergy;
    }
    public void OnPressed()
    {
        print(text.text);
        var messageApp = FindFirstObjectByType<MessageApp>();
        messageApp.SetCurrentConv(messageApp.GetCurrentDiscussion().gameObject);
        if (messageApp.CurrentConv==null)
        {
            return; // il y a un problème si ça passe par là!
        }
        messageApp.CurrentConv.GetComponent<Discussion>().Choose(value);

    }

    public void OnDestroy()
    {
        EnergyManager.Instance.OnEnergyChange -= CheckForEnergy;
    }


    public void CheckForEnergy(int value)
    {
        Button button = GetComponent<Button>();
        if (value < 5)
        {
            button.enabled = false;
        }
        else
        {
            button.enabled = true;
        }
    }
}
