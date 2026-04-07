using System.Globalization;
using UnityEngine;

namespace TCG.Core.Dialogues
{
    public class TextCommandAddName : TextCommand
    {
    
        private string _name = "John";

        public override void SetupData(string strCommandData)
        {
            Typer.CurrentText=Typer.CurrentText.Insert(EnterIndex, _name);
            return;
        }

    }
}