using UnityEngine;

namespace Combat.Units
{
    public class MeatEnemy : Enemy
    {
        public override UnitMoves.Move ChooseMove(Player playerUnit, UnitMoves.Move lastMove,
            UnitMoves.Move lastPlayerMove)
        {
            if (currentSpeed <= speed || currentSpeed <= playerUnit.currentSpeed)
            {
                float x = Random.Range(0.0f, 1.0f);
                return x > 0.5 ? UnitMoves.Move.SpeedUp : UnitMoves.Move.SpeedDown;
            }

            if (lastPlayerMove == UnitMoves.Move.Attack || lastPlayerMove == UnitMoves.Move.PhysicalFish ||
                lastPlayerMove == UnitMoves.Move.PhysicalVegetables)
            {
                return UnitMoves.Move.Counter;
            }

            if (lastPlayerMove == UnitMoves.Move.DefendMeat || lastPlayerMove == UnitMoves.Move.AbsorbMeat)
            {
                return UnitMoves.Move.Attack;
            }
            
            if (1.0f * currentHp / maxHp <= healthPercentageToHeal)
            {
                return UnitMoves.Move.Heal;
            } 

            return UnitMoves.Move.PhysicalMeat;
        }
    }
}