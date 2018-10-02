using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RepeatScene : MonoBehaviour {

	public GameObject repeatGoalsToggle;
	//public Texture2D oldPixelMap;
	//public string oldPlanGoals;

	public bool repeatMaze = false;
	public bool repeatGoals = false;
	
	public void RepeatMaze()
	{
		repeatMaze = !repeatMaze;
		if (repeatMaze)
		{
			repeatGoalsToggle.SetActive(true);
			GetComponent<Button>().interactable = true;
		}
		else
		{
			repeatGoalsToggle.SetActive(false);
			GetComponent<Button>().interactable = false;
		}
		
	}

	public void RepeatGoals()
	{
		repeatGoals = !repeatGoals;
	}

	public void Repeat()
	{
		GetComponentInParent<Canvas>().enabled = false;
		if (repeatMaze)
		{
			FindObjectOfType<BuildMazeLayout>().RunMazeGame();
			if (repeatGoals)
			{
				FindObjectOfType<BuildPlanGoal>().StartPlanner();
			}

		}
		
	}
	
}
