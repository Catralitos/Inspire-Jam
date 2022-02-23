using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Combat
{
    public class UnitMoves : MonoBehaviour
    {
        public enum Move
        {
            Attack,
            Defend,
            Heal,
            Counter,
            Vampire,
            HealingDefense,
            Meat,
            Fish,
            Vegetables,
            PhysicalMeat,
            PhysicalFish,
            PhysicalVegetables,
            AbsorbMeat,
            AbsorbFish,
            AbsorbVegetables,
            DefendMeat,
            DefendFish,
            DefendVegetables,
            SwitchMeat,
            SwitchFish,
            SwitchVegetables,
            AttackUp,
            AttackDown,
            SpecialAttackUp,
            SpecialAttackDown,
            DefenseUp,
            DefenseDown,
            SpecialDefenseUp,
            SpecialDefenseDown,
            SpeedUp,
            SpeedDown,
            EvasionUp,
            EvasionDown,
            AccuracyUp,
            AccuracyDown
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

        private BattleSystem _battleSystem;

        private void Start()
        {
            _battleSystem = BattleSystem.Instance;
        }

        [Header("Critical Hits")] public float criticalHitMultiplier = 1.5f;
        [Range(0f, 1f)] public float criticalHitChance = 0.35f;

        [Range(0f, 1f)] public float debuffPercentage = 0.2f;

        public void PerformMove(Move performerMove, Move targetMove, Unit performer, Unit target)
        {
            if (performerMove == null || targetMove == null || performer == null || target == null)
            {
                _battleSystem.DisplayMessage(performer.unitName + " did nothing...");
                return;
            }

            //Evasion and crits
            //TODO rever o willHit
            bool willHit = performer.accuracy / target.evasion > 0.5;
            float criticalRoll = Random.Range(0f, 1f);
            float criticalDamage = criticalRoll >= criticalHitChance ? 1 : criticalHitMultiplier;

            //Damage formulas
            int physicalDefense = targetMove == Move.Defend ? target.currentDefense * 2 : target.currentDefense;
            physicalDefense = targetMove == Move.HealingDefense
                ? Mathf.RoundToInt(target.currentDefense * 1.5f)
                : target.currentDefense;

            int specialDefense = targetMove == Move.Defend
                ? target.currentSpecialDefense * 2
                : target.currentSpecialDefense;
            specialDefense = targetMove == Move.HealingDefense
                ? Mathf.RoundToInt(target.currentSpecialDefense * 1.5f)
                : target.currentSpecialDefense;

            int physicalDamage = Mathf.RoundToInt(performer.currentAttack * performer.currentAttack * criticalDamage /
                                                  physicalDefense);
            int specialDamage =
                Mathf.RoundToInt(performer.currentAttack * performer.currentAttack * criticalDamage / specialDefense);
            int toHeal = performer.currentSpecialAttack * 2;


            switch (targetMove)
            {
                case Move.Attack:
                    if (!willHit)
                    {
                        _battleSystem.DisplayMessage(performer.unitName + " attack's missed...");
                        return;
                    }

                    target.TakeDamage(physicalDamage);
                    _battleSystem.DisplayMessage(performer.unitName + " attack dealt " + physicalDamage + " HP to " +
                                                 target.unitName + ".");
                    break;
                case Move.Defend:
                    _battleSystem.DisplayMessage(performer.unitName + " defends.");
                    break;
                case Move.Heal:
                    performer.HealDamage(toHeal);
                    break;
                case Move.Counter:
                    if (target.currentSpeed > performer.currentSpeed)
                    {
                        target.TakeDamage(physicalDamage * 2);
                        _battleSystem.DisplayMessage(performer.unitName + " counters with " + physicalDamage +
                                                     " damage dealt to " +
                                                     target.unitName + ".");
                    }
                    else
                    {
                        _battleSystem.DisplayMessage(performer.unitName + " tries to counter! But " + target.unitName +
                                                     " hasn't attacked yet.");
                    }

                    break;
                case Move.Vampire:
                    int aux = Mathf.RoundToInt(physicalDamage - (physicalDamage * debuffPercentage));
                    target.TakeDamage(aux);
                    performer.HealDamage(aux);
                    _battleSystem.DisplayMessage(performer.unitName + " stole " + aux + " HP from " + target.unitName +
                                                 "!");
                    break;
                case Move.HealingDefense:
                    //Diminuir a defesa já tá feito em cima, só falta diminuir o heal
                    performer.HealDamage(toHeal / 2);
                    _battleSystem.DisplayMessage(performer.unitName + " defended and healed " +
                                                 Mathf.RoundToInt(toHeal / 2f) + " HP.");
                    break;
                case Move.Meat:
                    int meatDamage = Mathf.RoundToInt(specialDamage * target.meatMultiplier);
                    target.TakeDamage(meatDamage);
                    if (target.currentType == Unit.Type.Meat)
                    {
                        _battleSystem.DisplayMessage(performer.unitName + " performs a meat attack! But " +
                                                     target.unitName + " absorbs it and heals " + meatDamage +
                                                     " HP...");
                    }
                    else
                    {
                        _battleSystem.DisplayMessage(performer.unitName + " performs a meat attack! " +
                                                     target.unitName + " loses " + meatDamage + " HP!");
                    }

                    break;
                case Move.Fish:
                    int fishDamage = Mathf.RoundToInt(specialDamage * target.fishMultiplier);
                    target.TakeDamage(fishDamage);
                    if (target.currentType == Unit.Type.Fish)
                    {
                        _battleSystem.DisplayMessage(performer.unitName + " performs a fish attack! But " +
                                                     target.unitName + " absorbs it and heals " + fishDamage +
                                                     " HP...");
                    }
                    else
                    {
                        _battleSystem.DisplayMessage(performer.unitName + " performs a fish attack! " +
                                                     target.unitName + " loses " + fishDamage + " HP!");
                    }

                    break;
                case Move.Vegetables:
                    int vegetableDamage = Mathf.RoundToInt(specialDamage * target.vegetablesMultiplier);
                    target.TakeDamage(vegetableDamage);
                    if (target.currentType == Unit.Type.Vegetables)
                    {
                        _battleSystem.DisplayMessage(performer.unitName + " performs a vegetable attack! But " +
                                                     target.unitName + " absorbs it and heals " + vegetableDamage +
                                                     " HP...");
                    }
                    else
                    {
                        _battleSystem.DisplayMessage(performer.unitName + " performs a vegetable attack! " +
                                                     target.unitName + " loses " + vegetableDamage + " HP!");
                    }

                    break;
                case Move.PhysicalMeat:
                    int physicalMeatDamage = Mathf.RoundToInt(physicalDamage * target.meatMultiplier);
                    target.TakeDamage(physicalMeatDamage);
                    if (target.currentType == Unit.Type.Meat)
                    {
                        _battleSystem.DisplayMessage(performer.unitName + " performs a physical meat attack! But " +
                                                     target.unitName + " absorbs it and heals " + physicalMeatDamage +
                                                     " HP...");
                    }
                    else
                    {
                        _battleSystem.DisplayMessage(performer.unitName + " performs a physical meat attack! " +
                                                     target.unitName + " loses " + physicalMeatDamage + " HP!");
                    }

                    break;
                case Move.PhysicalFish:
                    int physicalFishDamage = Mathf.RoundToInt(physicalDamage * target.fishMultiplier);
                    target.TakeDamage(physicalFishDamage);
                    if (target.currentType == Unit.Type.Fish)
                    {
                        _battleSystem.DisplayMessage(performer.unitName + " performs a physical fish attack! But " +
                                                     target.unitName + " absorbs it and heals " + physicalFishDamage +
                                                     " HP...");
                    }
                    else
                    {
                        _battleSystem.DisplayMessage(performer.unitName + " performs a physical fish attack! " +
                                                     target.unitName + " loses " + physicalFishDamage + " HP!");
                    }

                    break;
                case Move.PhysicalVegetables:
                    int physicalVegetableDamage = Mathf.RoundToInt(physicalDamage * target.vegetablesMultiplier);
                    target.TakeDamage(physicalVegetableDamage);
                    if (target.currentType == Unit.Type.Vegetables)
                    {
                        _battleSystem.DisplayMessage(performer.unitName + " performs a physical vegetable attack! But " +
                                                     target.unitName + " absorbs it and heals " +
                                                     physicalVegetableDamage + " HP...");
                    }
                    else
                    {
                        _battleSystem.DisplayMessage(performer.unitName + " performs a physical vegetable attack! " +
                                                     target.unitName + " loses " + physicalVegetableDamage + " HP!");
                    }

                    break;
                case Move.AbsorbMeat:
                    break;
                case Move.AbsorbFish:
                    break;
                case Move.AbsorbVegetables:
                    break;
                case Move.DefendMeat:
                    break;
                case Move.DefendFish:
                    break;
                case Move.DefendVegetables:
                    break;
                case Move.SwitchMeat:
                    performer.SetType(Unit.Type.Meat);
                    _battleSystem.DisplayMessage(performer.unitName + " became a meat afficionado!");
                    break;
                case Move.SwitchFish:
                    performer.SetType(Unit.Type.Fish);
                    _battleSystem.DisplayMessage(performer.unitName + " became a fish afficionado!");
                    break;
                case Move.SwitchVegetables:
                    performer.SetType(Unit.Type.Vegetables);
                    _battleSystem.DisplayMessage(performer.unitName + " became a vegetable afficionado!");
                    break;
                case Move.AttackUp:
                    break;
                case Move.AttackDown:
                    break;
                case Move.SpecialAttackUp:
                    break;
                case Move.SpecialAttackDown:
                    break;
                case Move.DefenseUp:
                    break;
                case Move.DefenseDown:
                    break;
                case Move.SpecialDefenseUp:
                    break;
                case Move.SpecialDefenseDown:
                    break;
                case Move.SpeedUp:
                    break;
                case Move.SpeedDown:
                    break;
                case Move.EvasionUp:
                    break;
                case Move.EvasionDown:
                    break;
                case Move.AccuracyUp:
                    break;
                case Move.AccuracyDown:
                    break;
                default:
                    _battleSystem.DisplayMessage(performer.unitName + " did nothing...");
                    break;
            }
        }
    }
}