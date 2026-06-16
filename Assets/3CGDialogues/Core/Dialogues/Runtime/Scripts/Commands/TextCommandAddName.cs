using System.Globalization;
using UnityEngine;

namespace TCG.Core.Dialogues
{
    public class TextCommandAddName : TextCommand
    {
    
        private string _name = "John";

        public override void SetupData(string strCommandData)
        {

            if (SaveManager.instance!=null&&SaveManager.instance.Save != null)
            {
                _name = SaveManager.instance.Save.name;
            }
            Typer.CurrentText=Typer.CurrentText.Insert(EnterIndex, _name);
            return;
        }

    }
}