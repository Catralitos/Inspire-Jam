using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Unit
{
    //Used to code enemy AI
    public abstract UnitMoves.Move ChooseMove(Player playerUnit, UnitMoves.Move lastMove, UnitMoves.Move lastPlayerMove);
}