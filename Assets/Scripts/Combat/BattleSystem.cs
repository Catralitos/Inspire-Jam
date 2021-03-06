using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Brush;
using Combat.Units;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace Combat
{
    public enum BattleState
    {
        Start,
        Turn,
        Error,
        Confirmation,
        Playout,
        Won,
        Lost
    }

    public class BattleSystem : MonoBehaviour
    {
        public static BattleSystem Instance { get; private set; }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("Multiple battle systems present in scene! Destroying...");
                Destroy(gameObject);
            }
        }

        [HideInInspector] public BattleState state;

        public string winScene;
        public string loseScene;

        [Header("Player")] public Player playerUnit;
        public BattleHUD playerHUD;

        [Header("Enemy")] public Enemy enemyUnit;
        public BattleHUD enemyHUD;

        [Header("UI")] public TextMeshProUGUI dialogueText;

        [HideInInspector] public UnitMoves.Move playerMove;
        [HideInInspector] public UnitMoves.Move enemyMove;

        [HideInInspector] public UnitMoves.Move lastPlayerMove;
        [HideInInspector] public UnitMoves.Move lastEnemyMove;

        [HideInInspector] public int turnsElapsed;

        [Header("Unique Dialogue")] public string winText;
        public string loseText;

        public Image spriteHolder;
        public List<PairSpriteMove> sprites;
        
        private string _playerMoveString;

        private AudioManager _audioManager;
        private void Start()
        {
            _audioManager = GetComponent<AudioManager>();
            _audioManager.Play("BattleMusic");
            state = BattleState.Start;
            playerHUD.SetHUD(playerUnit);
            enemyHUD.SetHUD(enemyUnit);
            DisplayMessage(enemyUnit.name + " has challenged you!");
            spriteHolder.gameObject.SetActive(false);

            StartCoroutine(SetupBattle());
        }

        private IEnumerator SetupBattle()
        {
            yield return new WaitForSeconds(2f);

            SetStateToTurn();
        }

        private void SetStateToTurn()
        {
            state = BattleState.Turn;
            spriteHolder.gameObject.SetActive(false);

            DisplayMessage("Paint an action! Press space to finish, and R to clear the canvas.");
        }

        private void SetStateToConfirm()
        {
            state = BattleState.Confirmation;
            ChooseRightSprite();
            spriteHolder.gameObject.SetActive(true);
            DisplayMessage("Do you want to perform a " + _playerMoveString + " move? Space to confirm, R to redraw.");
        }

        private void ChooseRightSprite()
        {
            foreach (var pair in sprites.Where(pair => pair.move.ToString() == _playerMoveString))
            {
                spriteHolder.sprite = pair.sprite;
                return;
            }
        }
        
        private void Update()
        {
            if (state == BattleState.Turn)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    LineManager.Instance.ClearLine();
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _playerMoveString = LineManager.Instance.TryRecognize();
                    if (_playerMoveString != "")
                    {
                        SetStateToConfirm();
                    }
                    else StartCoroutine(ActionNotRecognized());
                }
            }
            else if (state == BattleState.Confirmation)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ChooseMove(_playerMoveString);
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    SetStateToTurn();
                }
            }
        }

        private IEnumerator ActionNotRecognized()
        {
            state = BattleState.Error;
            spriteHolder.gameObject.SetActive(false);
            DisplayMessage("Your move was not recognized. Please try again.");
            yield return new WaitForSeconds(3f);
            SetStateToTurn();
        }

        private void ChooseMove(string move)
        {
            if (state != BattleState.Confirmation || move == "") return;

            playerMove = (UnitMoves.Move) Enum.Parse(typeof(UnitMoves.Move), move);
            enemyMove = enemyUnit.ChooseMove(playerUnit, lastEnemyMove, lastPlayerMove);
            StartCoroutine(nameof(PlayOutTurn));
        }

        private IEnumerator PlayOutTurn()
        {
            state = BattleState.Playout;
            spriteHolder.gameObject.SetActive(false);

            bool playerPerformed = false;
            bool enemyPerformed = false;

            if (playerMove == UnitMoves.Move.Defend || playerMove == UnitMoves.Move.DefendFish ||
                playerMove == UnitMoves.Move.DefendMeat || playerMove == UnitMoves.Move.DefendVegetables ||
                playerMove == UnitMoves.Move.HealingDefend)
            {
                PlayerAction(playerPerformed);
                playerPerformed = true;
                yield return new WaitForSeconds(3f);
            }

            if (enemyMove == UnitMoves.Move.Defend || enemyMove == UnitMoves.Move.DefendFish ||
                enemyMove == UnitMoves.Move.DefendMeat || enemyMove == UnitMoves.Move.DefendVegetables ||
                enemyMove == UnitMoves.Move.HealingDefend)
            {
                EnemyAction(enemyPerformed);
                enemyPerformed = true;
                yield return new WaitForSeconds(3f);
            }

            if (enemyUnit.currentSpeed > playerUnit.currentSpeed)
            {
                EnemyAction(enemyPerformed);
                yield return new WaitForSeconds(3f);
                PlayerAction(playerPerformed);
                yield return new WaitForSeconds(3f);
            }
            else
            {
                PlayerAction(playerPerformed);
                yield return new WaitForSeconds(3f);
                EnemyAction(enemyPerformed);
                yield return new WaitForSeconds(3f);
            }

            lastPlayerMove = playerMove;
            lastEnemyMove = enemyMove;
            //tenho que fazer isto para dar reset dos buffs no caso do defend e absorb
            playerUnit.SetType(playerUnit.currentType);
            enemyUnit.SetType(enemyUnit.currentType);
            playerHUD.SetHUD(playerUnit);
            enemyHUD.SetHUD(enemyUnit);

            turnsElapsed++;
            SetStateToTurn();
        }

        private void PlayerAction(bool playerPerformed)
        {
            if (playerPerformed) return;
            UnitMoves.Instance.PerformMove(playerMove, enemyMove, playerUnit, enemyUnit);
            enemyHUD.SetHUD(enemyUnit);

            if (enemyUnit.currentHp > 0) return;
            state = BattleState.Won;
            DisplayMessage(winText);
            Invoke(nameof(WonBattle), 3f);
        }

        private void EnemyAction(bool enemyPerformed)
        {
            if (enemyPerformed) return;
            UnitMoves.Instance.PerformMove(enemyMove, playerMove, enemyUnit, playerUnit);
            playerHUD.SetHUD(playerUnit);

            if (playerUnit.currentHp > 0) return;
            state = BattleState.Lost;
            DisplayMessage(loseText);
            Invoke(nameof(LostBattle), 3f);
        }

        private void WonBattle()
        {
            SceneManager.LoadScene(sceneName: winScene);
        }

        private void LostBattle()
        {
            SceneManager.LoadScene(sceneName: loseScene);
        }

        public void DisplayMessage(string message)
        {
            dialogueText.text = message;
        }
    }
}