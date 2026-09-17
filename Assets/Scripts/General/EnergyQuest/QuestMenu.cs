using UnityEngine;

public class QuestMenu : MonoBehaviour
{
    private QuestManager _questManager;
    [SerializeField] GameObject questDisplay;
    [SerializeField] GameObject returnButton;
    void Start()
    {
        _questManager = QuestManager.Instance;
        foreach(QuestBase quest in _questManager.SelectedQuests)
        {
            if (quest == null) continue;
            GameObject questDisp = Instantiate(questDisplay, transform);
            questDisp.GetComponent<QuestDisplay>().QuestDisplaySetup(quest);
        }
        returnButton.transform.SetAsLastSibling();
    }

}
