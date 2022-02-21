using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueHolder : MonoBehaviour
    {

        public TextMeshProUGUI dialogBox;
        public Image leftPortrait;
        public Image rightPortrait;


        public List<DialogueLine> dialogueLines;
        
        private void Awake()
        {
            StartCoroutine(dialogueSequence());
        }
        private IEnumerator dialogueSequence()
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                Deactivate();
                transform.GetChild(i).gameObject.SetActive(true);
                //yield return new WaitUntil(()=> transform.GetChild(i).GetComponent<DialogueLine>().finished);
            }

            yield return null;
        }
        private void Deactivate()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}

