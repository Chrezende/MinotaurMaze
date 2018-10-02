using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Unit))]
public class Minotaur : MonoBehaviour
{
	
	public int minotaurLife = 1;
	public bool engaged = false;
		
	[System.NonSerialized] public GridContructor grid;
	public TurnController turnController;
	public MazeExit mazeExit;

	private Player[] player;

	void Start ()
	{
		grid = FindObjectOfType<GridContructor> ();
		mazeExit = FindObjectOfType<MazeExit> ();
		turnController = FindObjectOfType<TurnController> ();
		player = FindObjectsOfType<Player> ();
		minotaurLife = player.Length;
	}

	void Update ()
	{
		Collider[] tilesInSpace = Physics.OverlapSphere (grid.CalcWorldPos (GetComponent<Unit> ().unitPositionX, transform.position.y - 1, GetComponent<Unit> ().unitPositionY), 0.1f);
		for (int i = 0; i < tilesInSpace.Length; i++) {
			if (tilesInSpace [i].GetComponent<Tile> ().playerNear < 99) {
				engaged = true;
			} else {
				engaged = false;
			}  
		}
		if (minotaurLife == 0) {
			Destroy (this.gameObject);
			GetComponent<Unit> ().HideStench ();
		}
	}

	public void OnTriggerEnter (Collider col)
	{
		if (col.GetComponent<Player> ()) {
			col.GetComponent<Player>().playerLife--;
		}
		if (col.GetComponent<Bullet> ()) {
			minotaurLife--;
			Destroy (col.gameObject);
		}
	}

	void OnDestroy ()
	{
		turnController.GetUnits ();
	}
}
