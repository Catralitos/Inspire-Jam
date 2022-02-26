using UnityEngine;

namespace Combat.Units
{
    public class MeatEnemy : Enemy
    {
        private bool _raisedSpeed;

        public override UnitMoves.Move ChooseMove(Player playerUnit, UnitMoves.Move lastMove,
            UnitMoves.Move lastPlayerMove)
        {
            bool needsHeal = 1.0f * currentHp / maxHp <= healthPercentageToHeal;
            float x = Random.Range(0.0f, 1.0f);

            if (!_raisedSpeed)
            {
                _raisedSpeed = true;
                return x > 0.5 ? UnitMoves.Move.SpeedUp : UnitMoves.Move.SpeedDown;
            }

            /*if (!needsHeal && (lastPlayerMove == UnitMoves.Move.Attack || lastPlayerMove == UnitMoves.Move.PhysicalFish ||
                lastPlayerMove == UnitMoves.Move.PhysicalVegetables))
            {
                return UnitMoves.Move.Counter;
            }*/

            if (BattleSystem.Instance.turnsElapsed > 0 && (lastPlayerMove == UnitMoves.Move.DefendMeat ||
                                                             lastPlayerMove == UnitMoves.Move.AbsorbMeat))
            {
                return UnitMoves.Move.Attack;
            }

            if (needsHeal)
            {
                return x > 0.5f ? UnitMoves.Move.Vampire : UnitMoves.Move.Heal;
            }

            return UnitMoves.Move.PhysicalMeat;
        }
    }
}