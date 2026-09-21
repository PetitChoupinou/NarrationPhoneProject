using System.Globalization;
using UnityEngine;

namespace TCG.Core.Dialogues
{
    public class TextCommandAddName : TextCommand
    {
    
        private string _name = "John";

        public override void SetupData(string strCommandData)
        {

            if (SaveManager.Instance!=null&&SaveManager.Instance.Save != null)
            {
                _name = SaveManager.Instance.GetCurrentStoryPlayerName();
            }
            Typer.CurrentText=Typer.CurrentText.Insert(EnterIndex, _name);
            return;
        }

    }
}