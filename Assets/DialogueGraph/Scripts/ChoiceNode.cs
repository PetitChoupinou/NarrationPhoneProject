using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class ChoiceNode : BaseNode {

    [Input] public int input;
    [Output] public int output;

    public override string GetData()
    {
        /*for (int i = 0; i < output.Count; i++)
        {
            
        }*/
        return "Choice/";
    }
}