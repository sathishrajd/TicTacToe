//MathWinCombos.cs - Holds the Math constants and win combos
namespace Game.TicTacToe
{
	using System;

	public static class MathWinCombos
	{
		public const short NUM_WINNING_COMBOS = 8;
		public const short WINNING_POSITION_SIZE = 3;

		//Can be read from a config file in future
		public static int[,] WinningCombos = new int[NUM_WINNING_COMBOS, WINNING_POSITION_SIZE] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, 
			{ 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 },
			{ 0, 4, 8 }, { 2, 4, 6 } };
	}
}

