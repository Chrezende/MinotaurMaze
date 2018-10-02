using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayersLibrary
{
	public Color32 color;
	public GameObject prefab;
	public string name;
	public Material material;

	public int actionPoints;
	public int stench;
	public float speed;
}

[System.Serializable]
public class ItemLibrary
{
	public Color32 color;
	public GameObject prefab;
	public string name;
	public Material material;

}

public class Spawner : MonoBehaviour
{
	public static int objQuantity = 0;
	public PlayersLibrary[] playerLibrary;
	public ItemLibrary[] itemLibrary;
	public GridContructor grid;
	public TurnController turnController;

	void Start ()
	{
		grid = GetComponent<GridContructor> ();
		turnController = GetComponent<TurnController> ();
	}

	public void SpawnUnit (Color32 color, Tile tile)
	{
		foreach (PlayersLibrary pl in playerLibrary) {
			if(color.Equals(pl.color))
			{
				Vector3 startPosition = grid.CalcWorldPos(tile.tileX, 1, tile.tileY);
				GameObject gameObj = Instantiate(pl.prefab, startPosition, Quaternion.identity) as GameObject;

				MeshRenderer objMesh = gameObj.GetComponent<MeshRenderer>();
				if (pl.material != null)
				{
					objMesh.material = pl.material;
				}

				gameObj.transform.parent = this.transform;
				gameObj.name = pl.name;
				gameObj.GetComponent<Unit>().unitPositionX = tile.tileX;
				gameObj.GetComponent<Unit>().unitPositionY = tile.tileY;
				gameObj.GetComponent<Unit>().unitPositionText = "l" + tile.tileX.ToString() + "c" + tile.tileY.ToString();
				gameObj.GetComponent<Unit>().moveSpeed = pl.actionPoints;
				gameObj.GetComponent<Unit>().stench = pl.stench;
				gameObj.GetComponent<Unit>().speed = pl.speed;

				if (gameObj.GetComponent<Minotaur>())
				{
					grid.problemObjects += "\n\t\t" + gameObj.name + " - minotaur\n\t\t";
				}
				if (gameObj.GetComponent<Player>())
				{
					grid.problemObjects += "\n\t\t" + gameObj.name + " - player\n\t\t";
					grid.initialState.Add("atTile", gameObj.name, gameObj.GetComponent<Unit>().unitPositionText);
					grid.initialState.Add("isAlive", gameObj.name);
				}
			}

			

		}

		turnController.GetUnits ();
	}

	public void SpawnItem (Color32 color, Tile tile)
	{
		foreach (var item in itemLibrary) {
			if (color.Equals (item.color)) {
				Vector3 ObjPosition = (grid.CalcWorldPos (tile.tileX, 1f, tile.tileY));
				GameObject gameObj = Instantiate (item.prefab, ObjPosition, Quaternion.identity) as GameObject; //Cria e posiciona o hexagono.
				gameObj.transform.parent = this.transform;

				gameObj.name = item.name + objQuantity.ToString ();

				grid.problemObjects += "\n\t\t" + gameObj.name + " - key\n\t\t";
				grid.initialState.Add ("objectOnMap", gameObj.name, tile.gameObject.name);
				objQuantity++;
			}
		}
	}
}
