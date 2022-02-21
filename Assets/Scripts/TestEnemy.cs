using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Enemy
{
    public override UnitMoves.Move ChooseMove(Player playerUnit, UnitMoves.Move lastMove, UnitMoves.Move lastPlayerMove)
    {
        int aux = Random.Range(0, 2); 
        return aux % 2 == 0 ? UnitMoves.Move.ATTACK : UnitMoves.Move.DEFEND;
    }
}
