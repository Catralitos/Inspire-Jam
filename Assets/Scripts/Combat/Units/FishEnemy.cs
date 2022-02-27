using UnityEngine;

namespace Combat.Units
{
    public class FishEnemy : Enemy
    {
        public override UnitMoves.Move ChooseMove(Player playerUnit, UnitMoves.Move lastMove,
            UnitMoves.Move lastPlayerMove)
        {
            bool needsHeal = 1.0f * currentHp / maxHp <= healthPercentageToHeal;
            bool needsDebuff = 1.0f * currentHp / maxHp <= healthPercentageToHeal * 2;
            float x = Random.Range(0.0f, 1.0f);

            if (lastPlayerMove == UnitMoves.Move.DefendFish || lastPlayerMove == UnitMoves.Move.AbsorbFish)
            {
                return UnitMoves.Move.Attack;
            }

            if (needsHeal)
            {
                return x < 0.85f ? UnitMoves.Move.Vampire : UnitMoves.Move.Heal;
            }

            if (BattleSystem.Instance.turnsElapsed > 0 && (lastPlayerMove == UnitMoves.Move.Attack ||
                                                           lastPlayerMove == UnitMoves.Move.Vampire))
            {
                return x < 0.5f ? UnitMoves.Move.Counter : UnitMoves.Move.Defend;
            }

            if (BattleSystem.Instance.turnsElapsed > 0 && (lastPlayerMove == UnitMoves.Move.Vegetables ||
                                                           lastPlayerMove == UnitMoves.Move.PhysicalVegetables))
            {
                return UnitMoves.Move.DefendVegetables;
            }

            if (needsDebuff)
            {
                return UnitMoves.Move.SpecialAttackDown;
            }

            return UnitMoves.Move.PhysicalFish;
        }
    }
}