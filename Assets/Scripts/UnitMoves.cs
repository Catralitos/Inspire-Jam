using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitMoves : MonoBehaviour
{
    public enum Move
    {
        ATTACK,
        DEFEND
    }

    [HideInInspector] public static UnitMoves Instance { get; private set; }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple unit moves present in scene! Destroying...");
            Destroy(gameObject);
        }
    }
    
    public TextMeshProUGUI dialogueText;

    public int level = 100;
    public int power = 80;
    public int badge = 8;
    public int baseAccuracy = 80;

    //Fórmula de dano Pokémon: https://bulbapedia.bulbagarden.net/wiki/Damage
    //Fórmula de evasão de Pokémon: https://monster-master.fandom.com/wiki/Evasion

    private void DisplayMessage(string message)
    {
        dialogueText.text = message;
    }
    
    public void PerformMove(Move performerMove, Move targetMove, Unit performer, Unit target)
    {
        if (performerMove == Move.ATTACK)
        {
            bool willHit = baseAccuracy * performer.accuracy / target.evasion > 1;
            if (!willHit)
            {
                DisplayMessage(performer.unitName + " attack's missed...");
                return;
            }
            int defense = targetMove == Move.DEFEND ? target.defense * 2 : target.defense;
            int isCritical = Random.Range(2, 4);
            int damage = Mathf.RoundToInt(
                ((((((2 * level) / 5) + 2) * power * (performer.attack / defense)) / 50) + 2.0f) *
                (isCritical * 0.5f) * badge);
            target.TakeDamage(damage);
            DisplayMessage(performer.unitName + " attack dealt " + damage + " to " + target.unitName + ".");
            Debug.Log(performer.unitName + " attack dealt " + damage + " to " + target.unitName + ".");

        }
        if (performerMove == Move.DEFEND)
        {
            //do nothing, cause if defense is already handled in attack
            DisplayMessage(performer.unitName + " defends.");
            Debug.Log(performer.unitName + " defends.");

        }
    }

}