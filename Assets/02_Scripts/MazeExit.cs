using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeExit : MonoBehaviour
{

	public Player player;
	public GridContructor gridContructor;
	public Tile thisTile;

	//private SceneController sceneController;

	// Use this for initialization
	void Start ()
	{
		//sceneController = FindObjectOfType<SceneController> ();
		gridContructor = FindObjectOfType<GridContructor> ();
		player = FindObjectOfType<Player> ();
		thisTile = GetComponent<Tile> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (player)
		{
			if (player.GetComponent<Unit>().unitPositionX == thisTile.tileX && player.GetComponent<Unit>().unitPositionY == thisTile.tileY)
			{
				WinGame();
			}
		}
		
	}

	/*    public void OpenExit() {
        gridContructor.graph[thisTile.tileX, thisTile.tileY].moveable = true;
        gridContructor.GeneratePathFindingGraph();
        this.transform.transform.Translate(0f,-1.0f,0f);
    }*/

	public void WinGame ()
	{
		GameObject.Find("Plan Finished Canvas").GetComponent<Canvas>().enabled = true;
		//sceneController.LoadScene (4);
	}
}
