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
            Meat,
            Fish,
            Vegetables,

            //estas sao combinaçoes secretas
            Counter,
            Vampire,
            HealingDefend,
            PhysicalMeat,
            PhysicalFish,
            PhysicalVegetables,
            AbsorbMeat,
            AbsorbFish,
            AbsorbVegetables,
            DefendMeat,
            DefendFish,
            DefendVegetables,

            //só para bosses
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
            bool willHit = performer.accuracy / target.evasion > 0.5f;
            float criticalRoll = Random.Range(0f, 1f);
            float criticalDamage = criticalRoll >= criticalHitChance ? 1 : criticalHitMultiplier;

            //Damage formulas
            int physicalDefense = targetMove == Move.Defend ? target.currentDefense * 2 : target.currentDefense;
            physicalDefense = targetMove == Move.HealingDefend || targetMove == Move.DefendMeat ||
                              targetMove == Move.DefendFish || targetMove == Move.DefendVegetables
                ? Mathf.RoundToInt(target.currentDefense * 1.5f)
                : target.currentDefense;

            int specialDefense = targetMove == Move.Defend
                ? target.currentSpecialDefense * 2
                : target.currentSpecialDefense;
            specialDefense = targetMove == Move.HealingDefend || targetMove == Move.DefendMeat ||
                             targetMove == Move.DefendFish || targetMove == Move.DefendVegetables
                ? Mathf.RoundToInt(target.currentSpecialDefense * 1.5f)
                : target.currentSpecialDefense;

            int physicalDamage = Mathf.RoundToInt(performer.currentAttack * performer.currentAttack * criticalDamage /
                                                  physicalDefense);
            int specialDamage =
                Mathf.RoundToInt(performer.currentSpecialAttack * performer.currentSpecialAttack * criticalDamage /
                                 specialDefense);
            int toHeal = performer.currentSpecialAttack * 2;

            if (performerMove == Move.HealingDefend)
            {
                toHeal = Mathf.RoundToInt(performer.currentSpecialAttack * 1.5f);
            }
            else if (performerMove == Move.DefendMeat)
            {
                if (performer.meatMultiplier > 0)
                {
                    performer.meatMultiplier = 0.5f;
                }
            }
            else if (performerMove == Move.DefendFish)
            {
                if (performer.fishMultiplier > 0)
                {
                    performer.fishMultiplier = 0.5f;
                }
            }
            else if (performerMove == Move.DefendVegetables)
            {
                if (performer.vegetablesMultiplier > 0)
                {
                    performer.vegetablesMultiplier = 0.5f;
                }
            }

            switch (performerMove)
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
                case Move.HealingDefend:
                    //Diminuir a defesa já tá feito em cima, só falta diminuir o heal
                    performer.HealDamage(toHeal / 2);
                    _battleSystem.DisplayMessage(performer.unitName + " defended and healed " +
                                                 Mathf.RoundToInt(toHeal / 2f) + " HP.");
                    break;
                case Move.Meat:
                    int meatDamage = Mathf.RoundToInt(specialDamage * target.meatMultiplier);
                    target.TakeDamage(meatDamage);
                    if (target.meatMultiplier < 0)
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
                    if (target.fishMultiplier < 0)
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
                    if (target.vegetablesMultiplier < 0)
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
                    if (target.meatMultiplier < 0)
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
                    if (target.fishMultiplier < 0)
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
                    if (target.vegetablesMultiplier < 0)
                    {
                        _battleSystem.DisplayMessage(performer.unitName +
                                                     " performs a physical vegetable attack! But " +
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
                    if (target.currentSpeed > performer.currentSpeed)
                    {
                        _battleSystem.DisplayMessage(performer.unitName + " tries to absorb a meat attack! But " +
                                                   target.unitName + " has already attacked...");
                    }
                    else
                    {
                        performer.meatMultiplier = -1.0f;
                        _battleSystem.DisplayMessage(performer.unitName + " will absorb a meat attack!");
                    }
                    break;
                case Move.AbsorbFish:
                    if (target.currentSpeed > performer.currentSpeed)
                    {
                        _battleSystem.DisplayMessage(performer.unitName + " tries to absorb a fish attack! But " +
                                                     target.unitName + " has already attacked...");
                    }
                    else
                    {
                        performer.fishMultiplier = -1.0f;
                        _battleSystem.DisplayMessage(performer.unitName + " will absorb a fish attack!");
                    }
                    break;
                case Move.AbsorbVegetables:
                    if (target.currentSpeed > performer.currentSpeed)
                    {
                        _battleSystem.DisplayMessage(performer.unitName + " tries to absorb a vegetable attack! But " +
                                                     target.unitName + " has already attacked...");
                    }
                    else
                    {
                        performer.vegetablesMultiplier = -1.0f;
                        _battleSystem.DisplayMessage(performer.unitName + " will absorb a vegetable attack!");
                    }
                    break;
                case Move.DefendMeat:
                    //defesa tratada em cima
                    _battleSystem.DisplayMessage(performer.unitName + " will defends with meat.");
                    break;
                case Move.DefendFish:
                    //defesa tratada em cima
                    _battleSystem.DisplayMessage(performer.unitName + " defends with fish.");
                    break;
                case Move.DefendVegetables:
                    //defesa tratada em cima
                    _battleSystem.DisplayMessage(performer.unitName + " defends with vegetables.");
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
                    performer.currentAttack += Mathf.RoundToInt(performer.currentAttack * debuffPercentage);
                    _battleSystem.DisplayMessage(performer.unitName + " just raised their attack!");
                    break;
                case Move.AttackDown:
                    target.currentAttack += Mathf.RoundToInt(target.currentAttack * debuffPercentage);
                    _battleSystem.DisplayMessage(performer.unitName + " just lowered " + target.unitName +
                                                 "'s attack!");
                    break;
                case Move.SpecialAttackUp:
                    performer.currentSpecialAttack +=
                        Mathf.RoundToInt(performer.currentSpecialAttack * debuffPercentage);
                    _battleSystem.DisplayMessage(performer.unitName + " just raised their special attack!");
                    break;
                case Move.SpecialAttackDown:
                    target.currentSpecialAttack += Mathf.RoundToInt(target.currentSpecialAttack * debuffPercentage);
                    _battleSystem.DisplayMessage(performer.unitName + " just lowered " + target.unitName +
                                                 "'s special attack!");
                    break;
                case Move.DefenseUp:
                    performer.currentDefense += Mathf.RoundToInt(performer.currentDefense * debuffPercentage);
                    _battleSystem.DisplayMessage(performer.unitName + " just raised their defense!");
                    break;
                case Move.DefenseDown:
                    target.currentDefense += Mathf.RoundToInt(target.currentDefense * debuffPercentage);
                    _battleSystem.DisplayMessage(performer.unitName + " just lowered " + target.unitName +
                                                 "'s defense!");
                    break;
                case Move.SpecialDefenseUp:
                    performer.currentSpecialDefense +=
                        Mathf.RoundToInt(performer.currentSpecialDefense * debuffPercentage);
                    _battleSystem.DisplayMessage(performer.unitName + " just raised their special defense!");
                    break;
                case Move.SpecialDefenseDown:
                    target.currentSpecialDefense += Mathf.RoundToInt(target.currentSpecialDefense * debuffPercentage);
                    _battleSystem.DisplayMessage(performer.unitName + " just lowered " + target.unitName +
                                                 "'s special defense!");
                    break;
                case Move.SpeedUp:
                    performer.currentSpeed +=
                        Mathf.RoundToInt(performer.currentSpeed * debuffPercentage);
                    _battleSystem.DisplayMessage(performer.unitName + " just raised their speed!");
                    break;
                case Move.SpeedDown:
                    target.currentSpeed += Mathf.RoundToInt(target.currentSpeed * debuffPercentage);
                    _battleSystem.DisplayMessage(performer.unitName + " just lowered " + target.unitName +
                                                 "'s speed!");
                    break;
                case Move.EvasionUp:
                    performer.currentEvasion +=
                        Mathf.RoundToInt(performer.currentEvasion * debuffPercentage);
                    _battleSystem.DisplayMessage(performer.unitName + " just raised their evasion!");
                    break;
                case Move.EvasionDown:
                    target.currentEvasion += Mathf.RoundToInt(target.currentEvasion * debuffPercentage);
                    _battleSystem.DisplayMessage(performer.unitName + " just lowered " + target.unitName +
                                                 "'s evasion!");
                    break;
                case Move.AccuracyUp:
                    performer.currentAccuracy +=
                        Mathf.RoundToInt(performer.currentAccuracy * debuffPercentage);
                    _battleSystem.DisplayMessage(performer.unitName + " just raised their accuracy!");
                    break;
                case Move.AccuracyDown:
                    target.currentAccuracy += Mathf.RoundToInt(target.currentAccuracy * debuffPercentage);
                    _battleSystem.DisplayMessage(performer.unitName + " just lowered " + target.unitName +
                                                 "'s accuracy!");
                    break;
                default:
                    _battleSystem.DisplayMessage(performer.unitName + " did nothing...");
                    break;
            }
        }
    }
}