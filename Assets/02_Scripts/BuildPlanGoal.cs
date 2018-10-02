using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildPlanGoal : MonoBehaviour {

	public GameObject newGoalInputField;

	public string goalBuilt;


	// Update is called once per frame
	void Update () {
		goalBuilt = "(and ";
		Toggle[] toggles = GameObject.Find("Goals Panel").GetComponentsInChildren<Toggle>();
		foreach (var toggle in toggles)
		{
			if (toggle.isOn)
			{
				goalBuilt += toggle.GetComponentInChildren<Text>().text;
			}
			
		}
		goalBuilt += " )";
	}

	public void AddNewGoal()
	{
		GameObject goalPanel = GameObject.Find("Goals Panel");
		Instantiate(newGoalInputField, goalPanel.transform);
		if(goalPanel.transform.childCount > 5)
		{
			GetComponent<Button>().interactable = false;
		}
	}


	public void StartPlanner()
	{
		GetComponentInParent<Canvas>().enabled = false;
		FindObjectOfType<PlayerPlanner>().problemGoal = goalBuilt;
		GameObject.Find("Play Canvas").GetComponent<Canvas>().enabled = true;
		Time.timeScale = 1;
		FindObjectOfType<PlayerPlanner>().StartPlanner();
		
	}
}
