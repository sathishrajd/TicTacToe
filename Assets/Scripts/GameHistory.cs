//GameHistory.cs - Responsible for storing cyclic history of last n events and generating history text
namespace Game.TicTacToe
{
	using System;
	using System.Collections.Generic;

	public class GameHistory
	{
		private const short MAX_HISTORY_COUNT = 10;

		int bestWinTimeInSecs = Int32.MaxValue;
		Queue<bool> historyEvents = new Queue<bool>();
		
		public GameHistory ()
		{
		}

		public void ReportGameHistory(bool win, int gameTimeInSecs)
		{
			if (bestWinTimeInSecs > gameTimeInSecs 
				&& win == true) 
			{
				bestWinTimeInSecs = gameTimeInSecs;
			}

			if (historyEvents.Count == MAX_HISTORY_COUNT) 
			{
				historyEvents.Dequeue ();
			}

			historyEvents.Enqueue (win);
		}

		public string GetHistoryString()
		{
			string hisoryString = "";
			bool [] historyELementsArray = historyEvents.ToArray ();

			hisoryString += "HISTORY : ";

			for (int i = (historyEvents.Count -1); i >= 0; i--) 
			{
				if (historyELementsArray [i] == true) 
				{
					hisoryString += "W";
				} 
				else 
				{
					hisoryString += "D";
				}
					
				hisoryString += "\t";
			}

			if (bestWinTimeInSecs != Int32.MaxValue) 
			{				
				hisoryString += "\t BEST WIN (In Secs) : " + bestWinTimeInSecs;
			}

			return hisoryString;
		}
	}
}

