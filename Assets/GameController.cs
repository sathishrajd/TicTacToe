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

		private const float AUTO_PLAY_WAIT_TIME = 1.5f;

		int currentPlayerIndex = INVALID_PLAYER_INDEX;
		private const short FIRST_PLAYER_INDEX = 0;

		//MathEvaluator can be static but not required
		MathEvaluator mathEval = null;

		Player player1 = new Player();
		Player player2 = new Player();

		[SerializeField]
		Button newGameBtn;

		[SerializeField]
		Text AutoPlayBtnText;

		[SerializeField]
		Text gameStatusText = null;

		[SerializeField]
		Text elapsedTimeText = null;

		[SerializeField]
		Text historyText = null;

		[SerializeField]
		GameObject gameBoardObject = null;

		[SerializeField]
		string player1_name= "";

		[SerializeField]
		string player2_name = "";

		[SerializeField]
		[Tooltip("Enter 0 - Input, Random -1 , AI - 2")] 
		int player1_type = 0;

		[SerializeField]
		[Tooltip("Enter 0 - Input, Random -1 , AI - 2")] 
		int player2_type = 1;

		GameBoard board;

		GameHistory gameHistory;

		float gameStartTime;

		Coroutine gameDelayCoroutine = null;

		bool bAutoPlay = false;
		private const float DELAY_BETWEEN_AUTOPLAY_GAMES = 4.0f;

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
			player1.InitializePlayer (player1_name, (Player.EPlayerTypes)player1_type, MathEvaluator.EBoardSelection.E_Selection_X);
			player2.InitializePlayer (player2_name, (Player.EPlayerTypes)player2_type, MathEvaluator.EBoardSelection.E_Selection_O);

			currentPlayerIndex = INVALID_PLAYER_INDEX;

			DisplayStatusMessage ("Select New Game to play ! \n (Toggle player_type in scene for AI, RANDOM or 2 PLAYERS)");
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
				yield return new WaitForSeconds (AUTO_PLAY_WAIT_TIME);

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
				Invoke("StartNewGame", DELAY_BETWEEN_AUTOPLAY_GAMES);
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

