using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepsController : MonoBehaviour {

	public bool nextStep = false;

	void Update()
	{
		if (nextStep)
		{
			GetComponent<Button>().interactable = false;
		}
		else
		{
			GetComponent<Button>().interactable = true;
		}
	}

	public void DoNextStep () {
		nextStep = true;
	}
}
