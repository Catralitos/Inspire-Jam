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
        START,
        TURN,
        ERROR,
        PLAYOUT,
        WON,
        LOST
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

        [Header("Brush")] public Transform brush;

        private void Start()
        {
            state = BattleState.START;
            StartCoroutine(SetupBattle());
        }

        IEnumerator SetupBattle()
        {
            DisplayMessage(enemyUnit.unitName + " has appeared!");

            playerHUD.SetHUD(playerUnit);
            enemyHUD.SetHUD(enemyUnit);

            yield return new WaitForSeconds(2f);

            SetStateToTurn();
        }

        private void SetStateToTurn()
        {
            state = BattleState.TURN;
            DisplayMessage("Paint an action! Press space to finish, and R to clear the canvas.");
        }

        private void Update()
        {
            if (state == BattleState.TURN)
            {
                if (Input.GetKey(KeyCode.R))
                {
                    LineManager.Instance.ClearLine();
                }

                if (Input.GetKey(KeyCode.Space))

                {
                    //Vector3 temp = Input.mousePosition;
                    //temp.z = .4f;

                    /*if (Input.GetKey(KeyCode.A))
                {
                    Debug.Log("Detetou o A");
                    Attack(playerUnit);
                }*/
                    //brush.position = Vector3.Lerp(brush.position, Camera.main.ScreenToWorldPoint(temp), .5f);
                    //ClampPosition(brush);

                    string move = LineManager.Instance.TryRecognize();
                    if (move != "") ChooseMove(move);
                    else StartCoroutine(ActionNotRecognized());
                }
            }
        }

        private IEnumerator ActionNotRecognized()
        {
            state = BattleState.ERROR;
            DisplayMessage("Your move was not recognized. Please try again.");
            yield return new WaitForSeconds(3f);
            SetStateToTurn();
        }

        private void ChooseMove(string move)
        {
            if (state != BattleState.TURN || move == "") return;

            if (move == "Attack")
            {
                playerMove = UnitMoves.Move.Attack;
                enemyMove = enemyUnit.ChooseMove(playerUnit, lastEnemyMove, lastPlayerMove);
                StartCoroutine(nameof(PlayOutTurn));
            }
            else if (move == "Defense")
            {
                playerMove = UnitMoves.Move.Defend;
                enemyMove = enemyUnit.ChooseMove(playerUnit, lastEnemyMove, lastPlayerMove);
                StartCoroutine(nameof(PlayOutTurn));
            }
        }

        private IEnumerator PlayOutTurn()
        {
            state = BattleState.PLAYOUT;

            bool playerPerformed = false;
            bool enemyPerformed = false;

            if (playerMove == UnitMoves.Move.Defend)
            {
                PlayerAction(playerPerformed);
                playerPerformed = true;
                yield return new WaitForSeconds(3f);
            }

            if (enemyMove == UnitMoves.Move.Defend)
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

            state = BattleState.TURN;
            DisplayMessage("Paint an action! Press space to finish, and R to clear the canvas.");
        }

        private void PlayerAction(bool playerPerformed)
        {
            if (playerPerformed) return;
            UnitMoves.Instance.PerformMove(playerMove, enemyMove, playerUnit, enemyUnit);
            enemyHUD.SetHP(enemyUnit.currentHp);

            if (enemyUnit.currentHp <= 0)
            {
                state = BattleState.WON;
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
                state = BattleState.LOST;
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

        private void ClampPosition(Transform obj)
        {
            Vector3 pos = Camera.main.WorldToViewportPoint(obj.position);
            pos.x = Mathf.Clamp01(pos.x);
            pos.y = Mathf.Clamp01(pos.y);
            obj.position = Camera.main.ViewportToWorldPoint(pos);
        }
    }
}