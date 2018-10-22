//MathEvaluator.cs - Responsible for evaluating the outcome of the game
//				  	 Get the best outcome to play the next move(AI)
//					 Random outcome to test the AI
namespace Game.TicTacToe
{
	using System;
	using System.Linq;
	using System.Collections.Generic;

	public class MathEvaluator
	{
		public const short 	MAX_NUM_MARKS = 9;

		private const short INVALID_SCORE = -1;
		private const short WINNING_SCORE = 100;
		private const short NO_SCORE = 0;

		public enum EBoardSelection
		{
			E_Selection_None = 0,
			E_Selection_X,
			E_Selection_O
		};

		struct MaxScoreData
		{
			public int score;
			public List<int> positions;

			public void Reset()
			{
				score = INVALID_SCORE;
				positions = null;
			}
		};

		public int GetNextBestMove(EBoardSelection [] boardData, EBoardSelection inputMark, bool bIsRandom = false)
		{
			UnityEngine.Debug.Assert (boardData.Length == MAX_NUM_MARKS, "Number of marks on board cant exceed max");

			DebugPrint("GetNextBestMove mark = " + inputMark.ToString());

			if (bIsRandom) 
			{
				return GenRandomMove (boardData);
			}

			MaxScoreData currPlayerMaxScore = GetMaxScorePositions (boardData, inputMark);

			//We have a winner, so return it
			if (currPlayerMaxScore.score == WINNING_SCORE) 
			{
				DebugPrint ("GetNextBestMove() winning move found ");
				return currPlayerMaxScore.positions [0];
			}

			DebugPrint ("GetNextBestMove() no winning move found for given player");

			//We dont a clear winner, so lets try scoring the opposite player moves.
			MaxScoreData oppPlayerMaxScore = GetMaxScorePositions (boardData, GetOppPlayerMark(inputMark));

			//Opposite Player has a winner, so block it
			if (oppPlayerMaxScore.score == WINNING_SCORE) 
			{
				DebugPrint ("GetNextBestMove() block opponent win");
				return oppPlayerMaxScore.positions [0];
			}

			DebugPrint ("GetNextBestMove() no blocking move found to opponent");

			if (oppPlayerMaxScore.score > currPlayerMaxScore.score) 
			{
				DebugPrint ("GetNextBestMove() Opposition got a better move");

				int randPosition = UnityEngine.Random.Range (0, oppPlayerMaxScore.positions.Count);
				return oppPlayerMaxScore.positions [randPosition];
			}

			//Lets find if there is any position which has high score for both players
			List<int> match = currPlayerMaxScore.positions.Intersect(oppPlayerMaxScore.positions).ToList<int>();

			if (match.Count > 0) 
			{
				//get a random positions from list of match
				int randPosition = UnityEngine.Random.Range (0, match.Count);
				return match [randPosition];
			} 
			else 
			{
				//get a random positions from list of max score positions
				int randPosition = UnityEngine.Random.Range (0, currPlayerMaxScore.positions.Count);
				return currPlayerMaxScore.positions [randPosition];
			}
		}

		public bool IsGameOver(EBoardSelection [] boardData)
		{
			UnityEngine.Debug.Assert (boardData.Length == MAX_NUM_MARKS, "Number of marks on board cant exceed max");

			bool bRetFlag = true;

			for (int i = 0; i < MAX_NUM_MARKS; i++) 
			{
				if (boardData [i] == EBoardSelection.E_Selection_None) 
				{
					bRetFlag = false;
				}
			}

			return bRetFlag;
		}

		public EBoardSelection EvaluateWin(EBoardSelection [] boardData, ref int [] winningPositions)
		{
			UnityEngine.Debug.Assert (boardData.Length == MAX_NUM_MARKS, "Number of marks on board cant exceed max");

			EBoardSelection winner = EBoardSelection.E_Selection_None;

			for(int i = 0; i < MathWinCombos.NUM_WINNING_COMBOS; i++)
			{
				if(boardData[MathWinCombos.WinningCombos[i, 0]] == EBoardSelection.E_Selection_O &&
					boardData[MathWinCombos.WinningCombos[i, 1]] == EBoardSelection.E_Selection_O &&
					boardData[MathWinCombos.WinningCombos[i, 2]] == EBoardSelection.E_Selection_O)
				{
					CopyWinningPositions (ref winningPositions, i);
					winner = EBoardSelection.E_Selection_O;
					break;
				}
				else if(boardData[MathWinCombos.WinningCombos[i, 0]] == EBoardSelection.E_Selection_X &&
					boardData[MathWinCombos.WinningCombos[i, 1]] == EBoardSelection.E_Selection_X &&
					boardData[MathWinCombos.WinningCombos[i, 2]] == EBoardSelection.E_Selection_X)
				{
					CopyWinningPositions (ref winningPositions, i);
					winner = EBoardSelection.E_Selection_X;
					break;
				}
				else
				{
					//do nothing
				}
			}

			return winner;
		}

		private void CopyWinningPositions(ref int [] winningPositions, int index)
		{
			//DebugPrint ("CopyWinningPositions index = " + index);

			winningPositions = new int[MathWinCombos.WINNING_POSITION_SIZE];

			for (int i = 0; i < MathWinCombos.WINNING_POSITION_SIZE; i++) 
			{
				winningPositions [i] = MathWinCombos.WinningCombos [index, i];
			}
		}

		private int GenRandomMove(EBoardSelection [] boardData)
		{
			List<int> avaiablePositions = new List<int> ();

			for (int i = 0; i < MAX_NUM_MARKS; i++) 
			{
				if (boardData [i] == EBoardSelection.E_Selection_None) 
				{
					avaiablePositions.Add (i);
				}
			}

			int randPosition = UnityEngine.Random.Range (0, avaiablePositions.Count); 

			return avaiablePositions[randPosition];
		}

		private MaxScoreData GetMaxScorePositions(EBoardSelection [] boardData, EBoardSelection inputMark)
		{
			MaxScoreData playerMaxScore = new MaxScoreData ();
			playerMaxScore.Reset ();

			DebugPrint ("GetMaxScorePositions inputMark = " + inputMark.ToString());

			List<int> scoreList = new List<int>();

			//First check if there is a potential winner with next move and calculate scores
			for (int i = 0; i < MAX_NUM_MARKS; i++) 
			{
				if (boardData [i] == EBoardSelection.E_Selection_None) 
				{
					int score = CalculateScore (boardData, inputMark, i);
					scoreList.Add(score);

					if (score == WINNING_SCORE) 
					{
						playerMaxScore.score = score;
						playerMaxScore.positions = new List<int> ();
						playerMaxScore.positions.Add (i);
						break; // We have a clear winner so break from loop
					} 
					else if (playerMaxScore.score != WINNING_SCORE 
						&& playerMaxScore.score < score) 
					{
						//If the score is less then remove the old scores
						if (playerMaxScore.score < score) 
						{
							playerMaxScore.positions = null;
							playerMaxScore.positions = new List<int> ();
						}
						playerMaxScore.score = score;
						playerMaxScore.positions.Add (i);
					} 
					else {
						//ignore the position
					}
				} 
				else 
				{
					scoreList.Add(INVALID_SCORE);
				}
			}

			string debugScores = "";
			for (int i = 0; i < scoreList.Count; i++) 
			{	
				debugScores += scoreList[i];
				debugScores += "\t";
			}

			DebugPrint ("GetMaxScorePositions Scores = " + debugScores);

			return playerMaxScore;
		}

		private int CalculateScore(EBoardSelection [] boardData, EBoardSelection inputMark, int position)
		{
			EBoardSelection[] clonedBoard = CloneBoardData (boardData);

			clonedBoard [position] = inputMark;

			int[] winningPositions = null;

			if (EvaluateWin (clonedBoard, ref winningPositions) == inputMark) 
			{
				return WINNING_SCORE;
			}

			if(IsGameOver(clonedBoard))
			{
				return NO_SCORE;
			}

			int score = 0;

			//How many winning combos are possible with this position
			for (int i = 0; i < MathWinCombos.NUM_WINNING_COMBOS; i++) 
			{
				//Check if the given position is part of winning combo
				if (MathWinCombos.WinningCombos [i, 0] == position
					|| MathWinCombos.WinningCombos [i, 1] == position
					|| MathWinCombos.WinningCombos [i, 2] == position) 
				{
					//We can ignore the winning combo, if the oponent already has a mark on it
					if (clonedBoard [MathWinCombos.WinningCombos [i, 0]] != GetOppPlayerMark (inputMark) &&
						clonedBoard [MathWinCombos.WinningCombos [i, 1]] != GetOppPlayerMark (inputMark) &&
						clonedBoard [MathWinCombos.WinningCombos [i, 2]] != GetOppPlayerMark (inputMark)) 
					{
						score++;
					}
				}
			}

			return score;
		}

		private EBoardSelection GetOppPlayerMark(EBoardSelection mark)
		{
			UnityEngine.Debug.Assert (mark != EBoardSelection.E_Selection_None, "player needs a valid mark");

			if (mark == EBoardSelection.E_Selection_O) 
			{
				return EBoardSelection.E_Selection_X;
			}

			return EBoardSelection.E_Selection_O;
		}

						
		private EBoardSelection [] CloneBoardData(EBoardSelection [] boardData)
		{
			EBoardSelection[] clonedBoard = new EBoardSelection[MAX_NUM_MARKS];

			for(int i = 0; i < boardData.Length; i++)
			{
				clonedBoard [i] = boardData [i];
			}

			return clonedBoard;
		}

		private void DebugPrint(string log)
		{
			#if UNITY_EDITOR
			UnityEngine.Debug.Log("MathEvaluator:" + log);
			#endif
		}
	}
}

