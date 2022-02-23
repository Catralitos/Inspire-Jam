using UnityEngine;

namespace Combat.Units
{
    public class TestEnemy : Enemy
    {
        public override UnitMoves.Move ChooseMove(Player playerUnit, UnitMoves.Move lastMove, UnitMoves.Move lastPlayerMove)
        {
            int aux = Random.Range(0, 2); 
            return aux % 2 == 0 ? UnitMoves.Move.Attack : UnitMoves.Move.Defend;
        }
    }
}
