using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Unit))]
public class Player : MonoBehaviour
{
	[System.NonSerialized] public GridContructor grid;
	[System.NonSerialized] public CameraController cam;
	public GameObject bomb;

	public bool shooting = false;
	public float bulletSpeed = 0.1f;
	public float offset = 4f;
	public bool haveBomb = true;
	public int playerLife = 1;


	void Start ()
	{
		grid = FindObjectOfType<GridContructor> ();
		cam = FindObjectOfType<CameraController> ();
	}

	private void Update()
	{
		if(playerLife == 0)
		{
			//FindObjectOfType<SceneController>().LoadScene(3);
			PlayerDie();
		}
	}

	public void ThrowBomb (Tile tile)
	{
		GetComponent<Unit> ().unitBusy = true;
		GetComponent<Unit> ().exploding = true;
		haveBomb = false;
		GameObject.Instantiate (bomb, tile.transform);
		List<Node> tileNeighbours = grid.graph [tile.tileX, tile.tileY].neighbours;
		tile.Explode ();
		foreach (var neighbour in tileNeighbours) {
			if (neighbour.lXcY != GetComponent<Unit> ().unitPositionText) {
				GameObject.Find (neighbour.lXcY).GetComponent<Tile> ().Explode ();
			}
		}
	}

	public void PlayerDie()
	{
		GameObject.Find("Play Canvas").GetComponent<Canvas>().enabled = false;
		GameObject.Find("Plan Finished Canvas").GetComponent<Canvas>().enabled = true;
		GameObject.Find("Plan Finished Title").GetComponent<Text>().text = "Agente morreu, tente de novo!";
		Destroy(this.gameObject,0.5f);
	}


}
