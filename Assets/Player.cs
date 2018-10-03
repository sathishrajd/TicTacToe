//Player.cs - Manages player data
namespace Game.TicTacToe
{
	using System;
	using System.Collections.Generic;

	public class Player
	{
		public Player()
		{
		}

		public enum EPlayerTypes
		{
			E_PLAYER_INPUT,
			E_PLAYER_RANDOM,
			E_PLAYER_AI
		};

		public string PlayerName 
		{
			get;
			private set;
		}

		public MathEvaluator.EBoardSelection PlayerMark
		{
			get ;
			private set;
		}

		public EPlayerTypes PlayerType
		{
			get;
			private set;
		}

		public void InitializePlayer(string name, EPlayerTypes playerType, MathEvaluator.EBoardSelection _playerMark)
		{
			PlayerType = playerType;
			PlayerMark = _playerMark;
			PlayerName = name;

			switch (playerType) 
			{
			case EPlayerTypes.E_PLAYER_AI:
				{
					PlayerName += " (AI)";
					break;
				} 
			case EPlayerTypes.E_PLAYER_RANDOM:
				{
					PlayerName += " (RANDOM)";
					break;
				} 
			default:
				break;
			}
		}
	}
}

