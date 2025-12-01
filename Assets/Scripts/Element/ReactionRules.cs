using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

[CreateAssetMenu(fileName = "ReactionRules", menuName = "Scriptable Objects/ReactionRules")]
public class ReactionRules : ScriptableObject
{
    [System.Serializable]
    public struct Reaction
    {
        public string requiredElementOne;
        public string requiredElementTwo;

    }

    public List<Reaction> rules = new List<Reaction>();

    public bool TryGetMatch(string inputA, string inputB)
    {
        

        string queryElementOne = inputA;
        string queryElementTwo = inputB;

       
        if (string.Compare(queryElementOne, queryElementTwo, System.StringComparison.Ordinal) > 0)
        {
            
            var temp = queryElementOne;
            queryElementOne = queryElementTwo;
            queryElementTwo = temp;
        }

       
        foreach (var definedReaction in rules)
        {
            string ruleElementOne = definedReaction.requiredElementOne;
            string ruleElementTwo = definedReaction.requiredElementTwo;

           

            if (ruleElementOne == queryElementOne && ruleElementTwo == queryElementTwo)
            {
                
                return true;
            }
        }

       
        return false;
    }
}
