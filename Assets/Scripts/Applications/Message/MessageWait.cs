using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageWait : MonoBehaviour
{
    [SerializeField] List<Image> circles;
    [SerializeField,Range(75f/365f,135f/365f)] float baseTint;
    [SerializeField,Range(23/365f,70/365f)] float darkTint;
    int currentDark = 0;
    float timer = 0;
    [SerializeField,Range(.5f,2)] float timeScale = 1;
    // Update is called once per frame
    private void OnEnable()
    {
        transform.SetAsLastSibling();
        SetCircleColor(0);
    }
    void Update()
    {
        if (timer > 1) timer = 0;
        timer += Time.deltaTime*timeScale;
        int darkOne = 0;
        if (timer < .33f) darkOne = 1;
        else if(timer <.67f) darkOne = 2;
        if (darkOne == currentDark) return;
        SetCircleColor(darkOne);
    }
    public void SetCircleColor(int darkOne)
    {
        for (int i = 0; i < circles.Count; i++)
        {
            if (i == darkOne)
            {
                circles[i].color = new Color(darkTint, darkTint, darkTint, 1);
                continue;
            }
            circles[i].color = new Color(baseTint, baseTint, baseTint, 1);
        }
        currentDark= darkOne; ;
    }
}
