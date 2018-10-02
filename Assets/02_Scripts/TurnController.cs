using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TurnController : MonoBehaviour
{

	public Unit[] units;
	public int currentUnit = 0;

	/*	void Start ()
	{
		units [currentUnit].NextTurn ();
	}*/

	void LateUpdate ()
	{
		bool unitDoingTurn = false;
		for (int i = 0; i < units.Length; i++) {
			if (units [i].waitingTurn == false) {
				unitDoingTurn = true;
				break;
			}
		}
		if (unitDoingTurn == false) {
			foreach (var unit in units) {
				unit.NextTurn ();
			}
		}
		/*if (units [currentUnit].waitingTurn == true) {
			currentUnit++;
			if (currentUnit >= units.Length) {
				currentUnit = 0;
			}
			units [currentUnit].NextTurn ();
		}*/
		
	}

	public void GetUnits ()
	{
		units = FindObjectsOfType<Unit> ();
		currentUnit = 0;
	}
}
