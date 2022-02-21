using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{

    public class BaseDialogueClass : MonoBehaviour
    {
        protected IEnumerator WriteText(string[] input, Text textHolder, Color textColor, Font textFont, float delay, int index)
        {
            for(int i = 0; i < input[index].Length; i++)
            {
                textHolder.text += input[index][i];
                yield return new WaitForSeconds(delay);
            }
        }
    }
}