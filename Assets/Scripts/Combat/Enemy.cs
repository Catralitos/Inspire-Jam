using Combat.Units;

namespace Combat
{
    public abstract class Enemy : Unit
    {
        //Used to code enemy AI
        public abstract UnitMoves.Move ChooseMove(Player playerUnit, UnitMoves.Move lastMove, UnitMoves.Move lastPlayerMove);
    }
}