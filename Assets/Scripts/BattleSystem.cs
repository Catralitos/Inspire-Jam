using System;
using System.Collections;
using TMPro;
using UnityEngine;

public enum BattleState
{
    START,
    TURN,
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

    public BattleState state;

    public Player playerUnit;
    public Enemy enemyUnit;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public TextMeshProUGUI dialogueText;

    public UnitMoves.Move playerMove;
    public UnitMoves.Move enemyMove;

    public UnitMoves.Move lastPlayerMove;
    public UnitMoves.Move lastEnemyMove;

    private void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    private void Update()
    {
        if (state == BattleState.TURN)
        {
            if (Input.GetKey(KeyCode.A))
            {
                Debug.Log("Detetou o A");
                Attack(playerUnit);
            }
        }

        /*if (state == BattleState.PLAYOUT)
        {
            PlayOutTurn();
        }*/
    }

    IEnumerator SetupBattle()
    {
        DisplayMessage(enemyUnit.unitName + " has appeared!");

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.TURN;
        DisplayMessage("Paint an action!");
    }

    private void DisplayMessage(string message)
    {
        dialogueText.text = message;
    }

    IEnumerator PlayOutTurn()
    {
        Debug.Log("Entrou no PlayoutTurn");
        if (state != BattleState.PLAYOUT) yield break;

        bool playerPerformed = false;
        bool enemyPerformed = false;

        if (playerMove == UnitMoves.Move.DEFEND)
        {
            PlayerAction(playerPerformed);
            playerPerformed = true;
            yield return new WaitForSeconds(3f);
        }

        if (enemyMove == UnitMoves.Move.DEFEND)
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
        DisplayMessage("Paint an action!");
    }

    private void PlayerAction(bool playerPerformed)
    {
        if (playerPerformed) return;
        Debug.Log("Entrou no PlayerAction");
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
        Debug.Log("Entrou no EnemyAction");
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

    //todo se calhar mudar isto para um switch case ou assim
    public void Attack(Unit attacker)
    {
        if (state != BattleState.TURN || attacker != playerUnit) return;

        playerMove = UnitMoves.Move.ATTACK;
        enemyMove = enemyUnit.ChooseMove(playerUnit, lastEnemyMove, lastPlayerMove);
        state = BattleState.PLAYOUT;
        StartCoroutine(nameof(PlayOutTurn));
    }
}