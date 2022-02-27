using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Combat.Units
{
    public class VegetableEnemy : Enemy
    {
        private readonly List<Type> _availableTypes = new List<Type>();
        private readonly List<Type> _unavailableTypes = new List<Type>();

        public int maxHeals = 2;
        private int _numHeals = 0;

        public int maxDebuffs = 1;
        private int _numDebuffs;
        private void Start()
        {
            _availableTypes.Add(Type.Meat);
            _availableTypes.Add(Type.Fish);
            _unavailableTypes.Add(Type.Vegetables);
        }

        public override UnitMoves.Move ChooseMove(Player playerUnit, UnitMoves.Move lastMove,
            UnitMoves.Move lastPlayerMove)
        {
            bool needsHeal = 1.0f * currentHp / maxHp <= healthPercentageToHeal;
            bool needsDebuff = 1.0f * currentHp / maxHp <= healthPercentageToHeal * 2;
            float x = Random.Range(0.0f, 1.0f);
            if (BattleSystem.Instance.turnsElapsed > 0 && BattleSystem.Instance.turnsElapsed % 3 == 0)
            {
                _numHeals = 0;
                //_numDebuffs = 0;
                if (_availableTypes.Count != 0) return ReturnRightMove();
                List<Type> aux = new List<Type>(_unavailableTypes);
                foreach (var t in aux.Where(t => t != currentType))
                {
                    _availableTypes.Add(t);
                    _unavailableTypes.Remove(t);
                }

                return ReturnRightMove();
            }

            if (playerUnit.currentHp > 0.3f * playerUnit.maxHp && _numHeals < maxHeals && needsHeal)
            {
                _numHeals++;
                return x <= 0.85f ? UnitMoves.Move.Heal : UnitMoves.Move.Vampire;
            }

            if (needsDebuff && _numDebuffs < maxDebuffs)
            {
                _numDebuffs++;
                return UnitMoves.Move.AttackDown;
            }
            
            if (BattleSystem.Instance.turnsElapsed > 0 &&
                (lastPlayerMove == UnitMoves.Move.Meat || lastPlayerMove == UnitMoves.Move.PhysicalMeat) &&
                currentType == Type.Vegetables)
            {
                return UnitMoves.Move.DefendMeat;
            }

            if (BattleSystem.Instance.turnsElapsed > 0 &&
                (lastPlayerMove == UnitMoves.Move.Fish || lastPlayerMove == UnitMoves.Move.PhysicalFish) &&
                currentType == Type.Meat)
            {
                return UnitMoves.Move.DefendFish;
            }

            if (BattleSystem.Instance.turnsElapsed > 0 && (lastPlayerMove == UnitMoves.Move.Vegetables ||
                                                           lastPlayerMove == UnitMoves.Move.PhysicalVegetables) &&
                currentType == Type.Fish)
            {
                return UnitMoves.Move.DefendVegetables;
            }

            return currentType switch
            {
                Type.Vegetables => UnitMoves.Move.Vegetables,
                Type.Meat => UnitMoves.Move.Meat,
                Type.Fish => UnitMoves.Move.Fish,
                Type.Neutral => UnitMoves.Move.Attack,
                _ => UnitMoves.Move.Attack
            };
        }

        private UnitMoves.Move ReturnRightMove()
        {
            switch (_availableTypes[0])
            {
                case Type.Meat:
                    _availableTypes.Remove(Type.Meat);
                    _unavailableTypes.Add(Type.Meat);
                    return UnitMoves.Move.SwitchMeat;
                case Type.Fish:
                    _availableTypes.Remove(Type.Fish);
                    _unavailableTypes.Add(Type.Fish);
                    return UnitMoves.Move.SwitchFish;
                case Type.Vegetables:
                    _availableTypes.Remove(Type.Vegetables);
                    _unavailableTypes.Add(Type.Vegetables);
                    return UnitMoves.Move.SwitchVegetables;
                case Type.Neutral:
                    return UnitMoves.Move.Attack;
                default:
                    return UnitMoves.Move.Attack;
            }
        }
    }
}