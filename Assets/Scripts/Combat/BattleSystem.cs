using System;
using System.Collections;
using Brush;
using Combat.Units;
using TMPro;
using UnityEngine;

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
        [HideInInspector] public static BattleSystem Instance { get; private set; }

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

        [Header("Player")] public Player playerUnit;
        public BattleHUD playerHUD;

        [Header("Enemy")] public Enemy enemyUnit;
        public BattleHUD enemyHUD;

        [Header("UI")] public TextMeshProUGUI dialogueText;

        [HideInInspector] public UnitMoves.Move playerMove;
        [HideInInspector] public UnitMoves.Move enemyMove;

        [HideInInspector] public UnitMoves.Move lastPlayerMove;
        [HideInInspector] public UnitMoves.Move lastEnemyMove;

        public int turnsElapsed;
        
        private string _playerMoveString;

        private void Start()
        {
            state = BattleState.Start;
            playerHUD.SetHUD(playerUnit);
            enemyHUD.SetHUD(enemyUnit);
            DisplayMessage(enemyUnit.unitName + " has appeared!");

            StartCoroutine(SetupBattle());
        }

        IEnumerator SetupBattle()
        {
            yield return new WaitForSeconds(2f);

            SetStateToTurn();
        }

        private void SetStateToTurn()
        {
            state = BattleState.Turn;
            DisplayMessage("Paint an action! Press space to finish, and R to clear the canvas.");
        }

        private void SetStateToConfirm()
        {
            state = BattleState.Confirmation;
            DisplayMessage("Do you want to perform a " + _playerMoveString + " move? Space to confirm, R to redraw.");
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
                    Debug.Log("Entrou no space");
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

            bool playerPerformed = false;
            bool enemyPerformed = false;

            if (playerMove == UnitMoves.Move.Defend || playerMove == UnitMoves.Move.DefendFish ||
                playerMove == UnitMoves.Move.DefendMeat || playerMove == UnitMoves.Move.DefendMeat ||
                playerMove == UnitMoves.Move.HealingDefend)
            {
                PlayerAction(playerPerformed);
                playerPerformed = true;
                yield return new WaitForSeconds(3f);
            }

            if (enemyMove == UnitMoves.Move.Defend || enemyMove == UnitMoves.Move.DefendFish ||
                enemyMove == UnitMoves.Move.DefendMeat || enemyMove == UnitMoves.Move.DefendMeat ||
                enemyMove == UnitMoves.Move.HealingDefend)
            {
                EnemyAction(enemyPerformed);
                enemyPerformed = true;
                yield return new WaitForSeconds(3f);
            }

            if (enemyUnit.speed > playerUnit.speed)
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

            turnsElapsed++;
            SetStateToTurn();
        }

        private void PlayerAction(bool playerPerformed)
        {
            if (playerPerformed) return;
            UnitMoves.Instance.PerformMove(playerMove, enemyMove, playerUnit, enemyUnit);
            enemyHUD.SetHP(enemyUnit.currentHp);

            if (enemyUnit.currentHp <= 0)
            {
                state = BattleState.Won;
                WonBattle();
            }
        }

        private void EnemyAction(bool enemyPerformed)
        {
            if (enemyPerformed) return;
            UnitMoves.Instance.PerformMove(enemyMove, playerMove, enemyUnit, playerUnit);
            playerHUD.SetHP(playerUnit.currentHp);

            if (playerUnit.currentHp <= 0)
            {
                state = BattleState.Lost;
                LostBattle();
            }
        }

        private void WonBattle()
        {
            DisplayMessage("You won!");
            //invoke load scene certa (meter parametro)
            UnityEditor.EditorApplication.isPlaying = false;
        }

        private void LostBattle()
        {
            DisplayMessage("You lost!");
            //invoke load scene certa (meter parametro)
            UnityEditor.EditorApplication.isPlaying = false;
        }

        public void DisplayMessage(string message)
        {
            dialogueText.text = message;
        }
        
    }
}