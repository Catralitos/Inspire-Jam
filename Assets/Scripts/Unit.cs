using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    
    public string unitName;

    public int maxHp;
    public int currentHp;
    public int attack;
    public int specialAttack;
    public int defense;
    public int specialDefense;
    public int speed;
    public float evasion;
    public float accuracy;
    public float fireDamage;
    public float iceDamage;
    public float waterDamage;
    public float thunderDamage;

    public void TakeDamage(int damage)
    {
        currentHp = Mathf.Clamp(currentHp - damage, 0, maxHp);
    }
}
