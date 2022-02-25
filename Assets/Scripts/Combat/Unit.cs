using System;
using UnityEngine;

namespace Combat
{
    public abstract class Unit : MonoBehaviour
    {
        public enum Type
        {
            Neutral,
            Meat,
            Vegetables,
            Fish
        }

        public string unitName;

        [Header("BaseStats")] public Type defaultType;
        public int maxHp;
        public int attack;
        public int specialAttack;
        public int defense;
        public int specialDefense;
        public int speed;
        [Range(0, 100)] public int evasion;
        [Range(0, 100)] public int accuracy;

        [HideInInspector] public int currentHp;
        [HideInInspector] public int currentAttack;
        [HideInInspector] public int currentSpecialAttack;
        [HideInInspector] public int currentDefense;
        [HideInInspector] public int currentSpecialDefense;
        [HideInInspector] public int currentSpeed;
        [HideInInspector] public int currentEvasion;
        [HideInInspector] public int currentAccuracy;
        [HideInInspector] public Type currentType;
        [HideInInspector] public float meatMultiplier;
        [HideInInspector] public float vegetablesMultiplier;
        [HideInInspector] public float fishMultiplier;

        public void Awake()
        {
            currentHp = maxHp;
            currentAttack = attack;
            currentSpecialAttack = specialAttack;
            currentDefense = defense;
            currentSpecialDefense = specialDefense;
            currentSpeed = speed;
            currentEvasion = evasion;
            currentAccuracy = accuracy;
            currentType = defaultType;
            SetType(defaultType);
        }

        public void TakeDamage(int damage)
        {
            currentHp = Mathf.Clamp(currentHp - damage, 0, maxHp);
        }
        
        public void HealDamage(int damage)
        {
            currentHp = Mathf.Clamp(currentHp + damage, 0, maxHp);
        }

        public void SetType(Type type)
        {
            switch (type)
            {
                case Type.Neutral:
                    currentType = Type.Neutral;
                    meatMultiplier = 1.0f;
                    fishMultiplier = 1.0f;
                    vegetablesMultiplier = 1.0f;
                    break;
                case Type.Meat:
                    currentType = Type.Meat;
                    meatMultiplier = -1.0f;
                    fishMultiplier = 2.0f;
                    vegetablesMultiplier = 1.0f;
                    break;
                case Type.Vegetables:
                    currentType = Type.Vegetables;
                    meatMultiplier = 2.0f;
                    fishMultiplier = 1.0f;
                    vegetablesMultiplier = -1.0f;
                    break;
                case Type.Fish:
                    currentType = Type.Fish;
                    meatMultiplier = 1.0f;
                    fishMultiplier = -1.0f;
                    vegetablesMultiplier = 2.0f;
                    break;
                default:
                    break;
            }
        }
    }
}