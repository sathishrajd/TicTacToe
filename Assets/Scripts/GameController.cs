//GameController.cs - Responsible for the main game play.
//					  Supports 3 modes - AI, Random, Player
namespace  Game.TicTacToe
{
	using System;
	using System.Collections;
	using UnityEngine;
	using UnityEngine.UI;

	public class GameController : MonoBehaviour
	{
		private const short INVALID_PLAYER_INDEX = -1;
		private const short MAX_PLAYERS = 2;

		private int currentPlayerIndex = INVALID_PLAYER_INDEX;
		private const short FIRST_PLAYER_INDEX = 0;

		//MathEvaluator can be static but not required
		MathEvaluator mathEval = null;

		Player player1 = new Player();
		Player player2 = new Player();

		[SerializeField]
		Button newGameBtn = null;

		[SerializeField]
		Text AutoPlayBtnText = null;

		[SerializeField]
		Text gameStatusText = null;

		[SerializeField]
		Text elapsedTimeText = null;

		[SerializeField]
		Text historyText = null;

		[SerializeField]
		GameObject gameBoardObject = null;

		[SerializeField]
		string Player1_Name= "";

		[SerializeField]
		[Tooltip("Enter 0 - Player Input, Random Play - 1 , AI - 2")] 
		int Player1_Type = 0;

		[SerializeField]
		string Player2_Name = "";

		[SerializeField]
		[Tooltip("Enter 0 - Player Input, Random Play - 1 , AI - 2")] 
		int Player2_Type = 2;

		[SerializeField]
		private float autoPlayWaitTimeInSecs = 1.5f;

		[SerializeField]
		private float delayInAutoPlayGamesInSecs = 4.0f;

		GameBoard board;

		GameHistory gameHistory;

		float gameStartTime;

		Coroutine gameDelayCoroutine = null;

		bool bAutoPlay = false;


		public GameController ()
		{
		}

		// Use this for initialization
		void Start () 
		{
			elapsedTimeText.enabled = false;
			gameStatusText.enabled = false;

			board = gameBoardObject.GetComponent<GameBoard> ();
			board.Initialize (OnPlayerPick);

			mathEval = new MathEvaluator();
			gameHistory = new GameHistory ();

			StartNewSession ();
		}

		// Update is called once per frame
		void Update () 
		{
			if (elapsedTimeText.enabled) 
			{
				//TODO should use StringBuilder
				elapsedTimeText.text = "Time Elapsed (secs) : "+ GetElapsedTime ().ToString();
			}
			else 
			{
				elapsedTimeText.text = "";
			}
		}

		public void StartNewSession()
		{
			player1.InitializePlayer (Player1_Name, (Player.EPlayerTypes)Player1_Type, MathEvaluator.EBoardSelection.E_Selection_X);
			player2.InitializePlayer (Player2_Name, (Player.EPlayerTypes)Player2_Type, MathEvaluator.EBoardSelection.E_Selection_O);

			currentPlayerIndex = INVALID_PLAYER_INDEX;

			DisplayStatusMessage ("Toggle Player[n]_Type in scene " +
				"for changing play mode to AI, Random or 2 Players" 
				+ "\n\n Select [New Game] to play !");
		}

		public void OnNewGameButtonPressed()
		{
			StartNewGame ();
		}

		void StartNewGame()
		{
			if (gameDelayCoroutine != null) 
			{				
				StopCoroutine (gameDelayCoroutine);
				gameDelayCoroutine = null;
			}

			board.Reset ();

			//Player always starts the game
			currentPlayerIndex = FIRST_PLAYER_INDEX;

			RestartTimer ();

			ShowElapsedTime (true);

			PlayTheTurn();
		}

		void PlayTheTurn()
		{
			DisplayStatusMessage ("Waiting for " + GetCurrentPlayer().PlayerName);

			gameDelayCoroutine = StartCoroutine ("OnWaitAndPlay");
		}

		public void OnPlayerPick(int pickedPosition)
		{
			board.EnableTouches (false);

			//should use TODO StringBuilder
			DebugPrint ("OnPlayerPick position = " + pickedPosition);

			board.UpdateBoard(pickedPosition, GetCurrentPlayer().PlayerMark);

			int[] winningPositions = null;

			if (mathEval.EvaluateWin (board.GameBoardData, ref winningPositions) == GetCurrentPlayer().PlayerMark) 
			{
				board.HighlightWin(winningPositions);

				DisplayStatusMessage (GetCurrentPlayer().PlayerName + " is the winner !");

				OnGameOver (true);
			} 
			else 
			{
				if (mathEval.IsGameOver (board.GameBoardData)) 
				{
					OnGameOver (false);
				} 
				else 
				{
					SwitchPlayer ();
				}
			}
		}

		public void OnAutoPlayBtnPressed()
		{
			if (!bAutoPlay) 
			{
				bAutoPlay = true;
				AutoPlayBtnText.text = "AutoPlay\nStop";
				newGameBtn.gameObject.SetActive(false);

				StartNewGame ();
			}
			else
			{
				bAutoPlay = false;
				AutoPlayBtnText.text = "AutoPlay";
				newGameBtn.gameObject.SetActive(true);
			}
		}

		private IEnumerator OnWaitAndPlay()
		{
			if (GetCurrentPlayer().PlayerType == Player.EPlayerTypes.E_PLAYER_AI || 
				GetCurrentPlayer().PlayerType == Player.EPlayerTypes.E_PLAYER_RANDOM
				|| bAutoPlay == true) 
			{
				yield return new WaitForSeconds (autoPlayWaitTimeInSecs);

				int newPosition = mathEval.GetNextBestMove (board.GameBoardData, GetCurrentPlayer().PlayerMark, 
					GetCurrentPlayer().PlayerType == Player.EPlayerTypes.E_PLAYER_RANDOM);

				OnPlayerPick (newPosition);
			}
			else
			{
				//Enable touches only for the player
				board.EnableTouches(true);	
			}

			yield break;
		}

		private void OnGameOver(bool isThereAWinner)
		{
			if(!isThereAWinner)
			{				
				DisplayStatusMessage ("Game Over!");
			}

			ShowElapsedTime (false);

			//Update History on end of game
			gameHistory.ReportGameHistory (isThereAWinner, GetElapsedTime ());
			historyText.text = gameHistory.GetHistoryString ();

			if (bAutoPlay) 
			{
				Invoke("StartNewGame", delayInAutoPlayGamesInSecs);
			}
  	    }

		private void SwitchPlayer()
		{
			//switch the player the game is not over yet
			currentPlayerIndex = currentPlayerIndex == 0 ? 1 : 0;  
			PlayTheTurn();
		}

		private void DisplayStatusMessage (string statusMsg)
		{
			gameStatusText.enabled = true;
			gameStatusText.text = statusMsg;
		}

		private void ShowElapsedTime (bool bShow)
		{
			elapsedTimeText.enabled = bShow;
		}

		private void RestartTimer()
		{
			gameStartTime = UnityEngine.Time.realtimeSinceStartup;
		}

		private int GetElapsedTime()
		{
			return (int)(UnityEngine.Time.realtimeSinceStartup - gameStartTime);
		}

		private Player GetCurrentPlayer()
		{
			if (currentPlayerIndex == 0)
				return player1;
			else
				return player2;
		}

		private void DebugPrint(string log)
		{
			#if UNITY_EDITOR
			Debug.Log("GameController:" + log);
			#endif
		}
	}
}

