using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

	[System.NonSerialized] public int unitPositionX;
	[System.NonSerialized] public int unitPositionY;
	public string unitPositionText;
	[System.NonSerialized] public int moveSpeed = 2;
	[System.NonSerialized] public int stench = 5;
	public int actionPoints;
	[System.NonSerialized] public float speed = 10f;

	public bool waitingTurn = false;
	public bool unitBusy = true;
	public bool haveKey = false;
	public bool exploding = false;

	public List<Node> currentPath = null;
	public GridContructor grid;

	// Use this for initialization
	void Start ()
	{
		grid = FindObjectOfType<GridContructor> ();
		actionPoints = 0; 
		ShowStench ();
		grid.graph [unitPositionX, unitPositionY].occupied = true;
	}
	
	// Update is called once per frame
	void Update ()
	{

		if (Vector3.Distance (transform.position, grid.CalcWorldPos (unitPositionX, transform.position.y, unitPositionY)) < 0.05f) {
			MoveNextTile ();
			ActionPolicy ();
			if (!exploding) {
				unitBusy = false;
			}
		}

		unitPositionText = "l" + unitPositionX.ToString () + "c" + unitPositionY.ToString ();

		transform.position = Vector3.Lerp (transform.position, grid.CalcWorldPos (unitPositionX, transform.position.y, unitPositionY), speed * Time.deltaTime);

	}

	void MoveNextTile ()
	{
		HideStench ();
		if (currentPath == null) {
			ShowStench ();
			return;
		}

		if (actionPoints <= 0) {
			ShowStench ();
			return;
		}
			
		actionPoints -= currentPath [0].movementCost;
		//Remove the old current/first node from the path
		grid.graph [currentPath [0].x, currentPath [0].y].occupied = false;
		unitPositionX = currentPath [1].x;
		unitPositionY = currentPath [1].y;
		grid.graph [currentPath [1].x, currentPath [1].y].occupied = true;

		currentPath.RemoveAt (0);

		if (currentPath.Count == 1) {
			//We only have one tile left in our path, and that tile MUST be our ultimate
			//destination -- and we are stanting on it!
			//So let's just clear our pathfinding info.
			currentPath = null;
		}
		ShowStench ();
	}

	public void NextTurn ()
	{
		waitingTurn = false;
		currentPath = null;
		actionPoints = moveSpeed;
	}

	public void MoveUnitTo (int tileX, int tileY)
	{
		grid.GeneratePathTo (this, tileX, tileY);
		if (Vector3.Distance (transform.position, grid.CalcWorldPos (unitPositionX, transform.position.y, unitPositionY)) < 0.05f) {
			MoveNextTile ();
		}
		unitBusy = true;

	}

	void ActionPolicy ()
	{
		if (actionPoints > 0) {
			// ----------- Movimentação do Minotauro ----------
			if (GetComponent<Minotaur> ()) {
				MoveUnitTo (FindObjectOfType<Player> ().GetComponent<Unit> ().unitPositionX, FindObjectOfType<Player> ().GetComponent<Unit> ().unitPositionY);
			}
		} else {
			waitingTurn = true;
		}
	}

	void ShowStench ()
	{
		for (int i = 0; i <= stench; i++) {
			List<Node> tilesReachable = grid.TilesReachables (this, stench - i);
			foreach (Node tile in tilesReachable) {
				Collider[] tilesInSpace = Physics.OverlapSphere (grid.CalcWorldPos (tile.x, transform.position.y - 1, tile.y), 0.1f);
				for (int j = 0; j < tilesInSpace.Length; j++) {
					if (GetComponent<Minotaur> ()) {
						tilesInSpace [j].SendMessage ("MinotaurDistance", stench - i);
					} else {
						tilesInSpace [j].SendMessage ("PlayerDistance", stench - i);
					}
				}
			}
		}
	}

	public void HideStench ()
	{
		List<Node> tilesReachable = grid.TilesReachables (this, stench * 2);
		foreach (Node tile in tilesReachable) {
			Collider[] tilesInSpace = Physics.OverlapSphere (grid.CalcWorldPos (tile.x, transform.position.y - 1, tile.y), 0.1f);
			for (int i = 0; i < tilesInSpace.Length; i++) {
				if (GetComponent<Minotaur> ()) {
					tilesInSpace [i].SendMessage ("MinotaurDistance", 99);
				} else {
					tilesInSpace [i].SendMessage ("PlayerDistance", 99);
				}
			}
		}
	}

	
	void OnDestroy()
	{
		FindObjectOfType<TurnController>().GetUnits();
	}
}
