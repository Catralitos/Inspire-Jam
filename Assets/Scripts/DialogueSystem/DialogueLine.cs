using System;
using UnityEngine;

namespace DialogueSystem
{
    [Serializable] public class DialogueLine
    {

        public string line;
        public Sprite leftSprite;
        public Sprite rightSprite;

        /*private Text textHolder;
        private int index;
        public bool finished { get; private set; }

        [Header("Text options")]
        [SerializeField] public string[] lines;
        [SerializeField] private Color textColor;
        [SerializeField] private Font textFont;

        [Header("Time parameters")]
        [SerializeField] private float delay;

        [Header("Character Image")]
        [SerializeField] private Sprite characterSprite;
        [SerializeField] private Image imageHolder;

        private void Awake()
        {
            index = 0;
            finished = false;
            textHolder = GetComponent<Text>();
            textHolder.text = "";
            imageHolder.sprite = characterSprite;
            imageHolder.preserveAspect = true;
        }

        private void Start()
        {
            StartCoroutine(WriteText(lines, textHolder, textColor, textFont, delay, index));

        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if(textHolder.text == lines[index])
                {
                    NextLine();
                }
                else
                {
                    StopAllCoroutines();
                    textHolder.text = lines[index];
                }
            }
        }

        void NextLine()
        {
            if(index < lines.Length - 1)
            {
                index++;
                textHolder.text = "";
                StartCoroutine(WriteText(lines, textHolder, textColor, textFont, delay, index));

            }
            else
            {
                if (index == lines.Length - 1)
                    finished = true;
            }
        }*/
    }
}

