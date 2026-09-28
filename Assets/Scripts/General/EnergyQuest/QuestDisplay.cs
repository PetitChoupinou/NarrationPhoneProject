using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text _titre;
    [SerializeField] TMP_Text _description;
    [SerializeField] TMP_Text _completion;
    [SerializeField] TMP_Text _reward;
    [SerializeField] Slider _completionBar;


    public void QuestDisplaySetup(QuestBase q)
    {
        _titre.text = q.Title;
        _description.text = q.Desc;
        _completion.text = q.Current+"/"+q.Targ;
        _completionBar.value = (float)q.Current/q.Targ;
        string rewardT = "";
        if (q.EReward ==1) rewardT = q.EReward + " énergie";
        else if (q.EReward >1) rewardT = q.EReward + " énergies";
        if (q.GReward > 0)
        {
            if (rewardT != "") rewardT += " et";
            if(q.GReward==1)rewardT += " " + q.GReward + " gemme";
            else rewardT += " " + q.GReward + " gemmes";
           
        }
        _reward.text = rewardT;
        if (q.IsComp == true) GetComponent<Image>().color = Color.green;
    }
    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
