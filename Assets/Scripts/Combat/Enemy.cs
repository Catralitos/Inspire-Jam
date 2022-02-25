using Combat.Units;
using UnityEngine;

namespace Combat
{
    public abstract class Enemy : Unit
    {
        [Range(0.0f,1.0f)] public float healthPercentageToHeal = 0.2f;

        //Used to code enemy AI
        public abstract UnitMoves.Move ChooseMove(Player playerUnit, UnitMoves.Move lastMove, UnitMoves.Move lastPlayerMove);
    }
}