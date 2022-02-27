using System.Collections;
using System.Collections.Generic;
using Audio;
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

        private AudioManager _audioManager;
        
        private void Awake()
        {
            index = 0;
            if(leftPortrait != null)
                leftPortrait.sprite = dialogueLines[index].leftSprite;
            if (rightPortrait != null)
                rightPortrait.sprite = dialogueLines[index].rightSprite;
            dialogBox.text = "";
            _audioManager = GetComponent<AudioManager>();
        }

        private void Start()
        {
            _audioManager.Play("DialogueMusic");
            string sceneName = SceneManager.GetActiveScene().name;
            if (rightPortrait.sprite != null)
            {
                GameManager.Instance.SaveScene();
            }
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
                if (leftPortrait != null)
                    leftPortrait.sprite = dialogueLines[index].leftSprite;
                if (rightPortrait != null)
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

