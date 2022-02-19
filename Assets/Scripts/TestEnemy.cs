using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Enemy
{
    public override UnitMoves.Move ChooseMove(Player playerUnit, UnitMoves.Move lastMove, UnitMoves.Move lastPlayerMove)
    {
        return UnitMoves.Move.DEFEND;
    }
}
