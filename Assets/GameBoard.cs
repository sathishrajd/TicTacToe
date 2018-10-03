//GameBoard.cs - Manages TicTacToe board touch area and win highlight
namespace Game.TicTacToe
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	public class GameBoard : MonoBehaviour
	{
		MathEvaluator.EBoardSelection [] board = new MathEvaluator.EBoardSelection[MathEvaluator.MAX_NUM_MARKS];
		Action<int> playerPickCallback;

		[SerializeField]
		List<Image> touchPositions = null;

		[SerializeField]
		List<Text> boardPlayerMarks = null;

		public MathEvaluator.EBoardSelection [] GameBoardData
		{
			get 
			{
				return board;
			}

		}

		void Start()
		{
			for (int i = 0; i < touchPositions.Count; i++) 
			{
				touchPositions [i].canvasRenderer.SetAlpha(0);
				touchPositions [i].enabled = false;
			}
		}

		public void Initialize(Action<int> pickCB)
		{
			playerPickCallback = pickCB;
		}

		public void Reset()
		{			
			for (int i = 0; i < board.Length; i++) 
			{
				board [i] = MathEvaluator.EBoardSelection.E_Selection_None;
				boardPlayerMarks [i].text = "";
				boardPlayerMarks [i].color = Color.white;
				boardPlayerMarks [i].fontStyle = FontStyle.Bold;
			}
		}

		public void UpdateBoard(int position, MathEvaluator.EBoardSelection playerMark)
		{
			DebugPrint ("UpdateBoard position = "+ position + "\t playerMark = " + playerMark.ToString());

			board [position] = playerMark;

			if (MathEvaluator.EBoardSelection.E_Selection_O == playerMark) 
			{				
				boardPlayerMarks [position].text = "0";
				boardPlayerMarks [position].color = Color.blue;
			} 
			else 
			{
				boardPlayerMarks [position].text = "X";
				boardPlayerMarks [position].color = Color.red;
			}
    	}

		public void HighlightWin(int[] winningPositions)
		{
			for (int i = 0; i < winningPositions.Length; i++) 
			{
				boardPlayerMarks [winningPositions[i]].color = Color.green;
				boardPlayerMarks [winningPositions [i]].fontStyle = FontStyle.BoldAndItalic;
				//DebugPrint ("HighlightWin " + winningPositions[i]);
			}
		}

		public void EnableTouches(bool bShouldAllowTouches)
		{
			for (int i = 0; i < touchPositions.Count; i++) 
			{
				touchPositions [i].canvasRenderer.SetAlpha(0);
				touchPositions [i].enabled = bShouldAllowTouches;
			}
		}

		public void OnTouchReceived(int position)
		{
			if (board [position-1] == MathEvaluator.EBoardSelection.E_Selection_None) 
			{
				playerPickCallback (position-1);
			}
		}

		private void DebugPrint(string log)
		{
			#if UNITY_EDITOR
			Debug.Log("GameBoard:" + log);
			#endif
		}
	}
}

