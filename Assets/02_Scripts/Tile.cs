using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Tile : MonoBehaviour
{

	public GridContructor grid;

	public int tileX;
	public int tileY;
	public Unit[] units;
	public bool tileSafe = true;
	public int minotaurNear;
	public int playerNear;

	TextMesh tm;

	void Start ()
	{
		minotaurNear = 99;
		playerNear = 99;
		units = FindObjectsOfType<Unit> ();
	}


	//void OnMouseUp ()
	//{
	//	foreach (Unit unit in units) {
	//		grid.GeneratePathTo (units [0], tileX, tileY);
	//	}
	//}

	void MinotaurDistance (int value)
	{
		minotaurNear = value;
		if (value == 99) {
			tileSafe = true;
		} else {
			tileSafe = false;
		}
	}

	void PlayerDistance (int value)
	{
		playerNear = value;
	}

	public void Explode ()
	{
		ParticleSystem explosion = GetComponentInChildren<ParticleSystem> ();
		explosion.Play ();
		if (GetComponentInChildren<Bomb> ()) {
			Destroy (GetComponentInChildren<Bomb> ().gameObject, explosion.main.duration);
		}
		if (minotaurNear == 0) {
			FindObjectOfType<Minotaur> ().minotaurLife--;
		}
		if (playerNear == 0)
		{
			FindObjectOfType<Player>().playerLife--;
		}
	}
}
