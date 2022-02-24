using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace DialogueSystem
{
    public class DialogueHolder : MonoBehaviour
    {
        [SerializeField] private string nextScene;
        public TextMeshProUGUI dialogBox;
        public Image leftPortrait;
        public Image rightPortrait;
        public bool finished { get; private set; }

        private int index;

        [Header("Text options")]
        [SerializeField] private Color textColor;
        [SerializeField] private Font textFont;

        [Header("Time parameters")]
        [SerializeField] private float delay;

        public List<DialogueLine> dialogueLines;
        
        private void Awake()
        {
            index = 0;
            leftPortrait.sprite = dialogueLines[index].leftSprite;
            rightPortrait.sprite = dialogueLines[index].rightSprite;
            dialogBox.text = "";
        }

        private void Start()
        {
            StartCoroutine(WriteText());
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (dialogBox.text == dialogueLines[index].line)
                {
                    NextLine();
                }
                else
                {
                    StopAllCoroutines();
                    dialogBox.text = dialogueLines[index].line;
                }
            }
        }

        void NextLine()
        {
            if (index < dialogueLines.Count - 1)
            {
                index++;
                leftPortrait.sprite = dialogueLines[index].leftSprite;
                rightPortrait.sprite = dialogueLines[index].rightSprite;
                dialogBox.text = "";
                StartCoroutine(WriteText());

            }
            else
            {
                if (index == dialogueLines.Count - 1)
                {
                    finished = true;
                    /*SceneManager.LoadScene(sceneName: nextScene);*/
                    GameManager.Instance.LoadNextScene(nextScene);
                }

            }
        }
        protected IEnumerator WriteText()
        {
            for (int i = 0; i < dialogueLines[index].line.Length; i++)
            {
                dialogBox.text += dialogueLines[index].line[i];
                yield return new WaitForSeconds(delay);
            }
        }
    }
}

