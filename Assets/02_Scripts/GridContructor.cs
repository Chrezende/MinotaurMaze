using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class PrefabLibrary
{
	public Color32 color;
	public GameObject prefab;
	public string name;
	public float height;
	public int movementCost = 1;
	public bool moveable = true;
	public bool spawnPoint = false;
}

public class GridContructor : MonoBehaviour
{
	#region Declaracao de variaveis

	private Vector3 startPos;
	//Declarando Vector3 para a posição inicial
	private float width = 1f;
	//Largura do HexTile
	private float height = 1f;
	//Altura do HexTile
	[System.NonSerialized] public int mapWidth;
	[System.NonSerialized] public int mapHeight;

	public MinotaurMaze.State initialState;
	[System.NonSerialized] public string problemObjects = "";

	[System.NonSerialized] public int objNumber = 0;

	public Node[,] graph;

	public float gap = 0.0f;
	//Valor dos espaços, inicialmente 0.0f
	public Texture2D pixelMap;
	//PixelMap que será usado de base para a construção;
	public PrefabLibrary[] prefabLibrary;

	#endregion

	void Start ()
	{
		initialState = new MinotaurMaze.State ("Initial State");
		
	}

	//Leitura dos pixels do pixelmap;
	public void LoadMap ()
	{
		EmptyMap ();

		Color32[] allPixels = pixelMap.GetPixels32 ();	//Carrega o pixelmap em um vetor;
		mapWidth = pixelMap.width;
		mapHeight = pixelMap.height;

		Camera.main.orthographicSize = mapHeight/2 + 1;

		CalcStartPos (mapHeight, mapWidth);

		graph = new Node[mapWidth, mapHeight];

		for (int x = 0; x < mapWidth; x++) {
			for (int y = 0; y < mapHeight; y++) {
				SpawnTileAt (allPixels [(y * mapWidth) + x], x, y);
			}
		}
		GeneratePathFindingGraph();
	}

	//Gerador do peças do grid;
	void SpawnTileAt (Color32 c, int x, int y)
	{
		//Verifica se é um pixel transparente, resultando em um espaço vazio.
		if (c.a <= 0) {
			return;
		}

		//Note: Não otimizado, usar dicionário para melhorar a velociade;
		foreach (PrefabLibrary pl in prefabLibrary) {
			if (c.Equals (pl.color)) {
				Vector3 ObjPosition = (CalcWorldPos (x, pl.height, y));
				GameObject gameObj = Instantiate (pl.prefab, ObjPosition, Quaternion.identity) as GameObject;
				gameObj.transform.parent = this.transform;

				if (gameObj.GetComponent<MazeExit> ()) {
					gameObj.name = "exit";
				} else if (gameObj.GetComponent<Passage> ()) {
					gameObj.name = "passage";
				} else {
					gameObj.name = "l" + x + "c" + y;
				}
				gameObj.GetComponent<Tile> ().tileX = x;
				gameObj.GetComponent<Tile> ().tileY = y;
				gameObj.GetComponent<Tile> ().grid = this;


				graph [x, y] = new Node ();

				graph [x, y].x = x;
				graph [x, y].y = y;
				if (gameObj.name == "exit") {
					graph [x, y].lXcY = "exit";
				} else if (gameObj.name == "passage") {
					graph [x, y].lXcY = "passage";
				} else {
					graph [x, y].lXcY = "l" + x + "c" + y;
				}
				graph [x, y].movementCost = pl.movementCost;
				graph [x, y].moveable = pl.moveable;
				if (graph [x, y].moveable) {
					if (objNumber % 5 == 0) {
						problemObjects += graph [x, y].lXcY + " - tile\n\t\t";
					} else {
						problemObjects += graph [x, y].lXcY + " - tile\t\t";
					}
					initialState.Add ("tileSafe", graph [x, y].lXcY);
					objNumber++;
				}

				if (pl.spawnPoint) {
					GetComponent<Spawner>().SpawnUnit(pl.color, gameObj.GetComponent<Tile>());
					GetComponent<Spawner> ().SpawnItem (pl.color, gameObj.GetComponent<Tile> ());
				}
				return;
			}
		}
		//Se essa linha for executada, significa que a função não achou um obj para a cor.
		Debug.LogError ("No color to prefab found for: " + c.ToString ());

	}

	//Limpa todos os filhos desse objeto;
	void EmptyMap ()
	{
		while (transform.childCount > 0) {
			Transform c = transform.GetChild (0);
			c.SetParent (null);
			DestroyImmediate(c.gameObject);
			//Destroy (c.gameObject);
		}
	}

	//Calcula a posição inicial;
	void CalcStartPos (int gH, int gW)
	{
		float x = -width * (gW / 2);
		float z = height * (gH / 2);

		startPos = new Vector3 (x, 0, z);
	}

	//Método para converter posição em (X,Y,Z) para World Position
	public Vector3 CalcWorldPos (int x, float y, int z)
	{
		float offset = 0;
		float xPos = startPos.x + x * width + offset;
		float zPos = startPos.z - z * height;
		float yPos = y; //*0.2								//gridPos.y = Nível que o HexTile tem que estar; 0.2f = offset de altura do HexTile

		return new Vector3 (xPos, yPos, -zPos);
	}

	//Classe que calcula o menor caminho entre dois Nodes
	public void GeneratePathTo (Unit unit, int x, int y)
	{
		/*Player unit = FindObjectOfType<Player>();*/
		unit.currentPath = null;

		Dictionary<Node, float> dist = new Dictionary<Node, float> ();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node> ();

		#region --- Algoritimo de Dijkstra ---
		//Setup the "Q" -- the list of nodes we haven't checked yet
		List<Node> unvisited = new List<Node> ();

		Node source = graph [unit.unitPositionX, unit.unitPositionY];
		Node target = graph [x, y];

		dist [source] = 0;
		prev [source] = null;

		//Initializing everything to have INFINITY distance, since we don't know 
		//any better right now. Also, it's possible that some nodes CAN'T be
		//reched from the source, which would make INFINITY a reasonable value
		foreach (Node v in graph) {
			if (v != source) {
				dist [v] = Mathf.Infinity;
				prev [v] = null;
			}
			unvisited.Add (v);
		}

		while (unvisited.Count > 0) {
			// "u" is going to be the unvisited node with the smallest distance
			Node u = null;

			foreach (Node possibleU in unvisited) {
				if (u == null || dist [possibleU] < dist [u]) {
					u = possibleU;
				}
			}

			if (u == target) {
				break;
			}


			unvisited.Remove (u);

			foreach (Node v in u.neighbours) {
				//float alt = dist[u] + u.DistanceTo(v);
				float alt = dist [u] + v.movementCost;
				if (alt < dist [v]) {
					dist [v] = alt;
					prev [v] = u;
				}
			}
		}

		if (prev [target] == null) {
			// No route between our target an the source

			return;
		}

		List<Node> currentPath = new List<Node> ();

		Node curr = target;

		//Step thought the "prev" chain and add it to our path
		while (curr != null) {
			currentPath.Add (curr);
			curr = prev [curr];
		}

		//Right now, currentPath describes a route from our target to our source
		//So we need to rever it!

		currentPath.Reverse ();
		#endregion

		unit.currentPath = currentPath;
	}

	//Classe que gera as conexões dos grafos
	public void GeneratePathFindingGraph ()
	{
		// Verificando as passagens antes de fazer o grafo
		foreach (var node in graph) {
			if (node.lXcY == "passage") {
				if ((node.x > 0 && graph [node.x - 1, node.y].moveable) && (node.x < mapWidth - 1 && graph [node.x + 1, node.y].moveable)) {
					initialState.Add ("passage", graph [node.x - 1, node.y].lXcY, graph [node.x + 1, node.y].lXcY);
					initialState.Add ("passage", graph [node.x + 1, node.y].lXcY, graph [node.x - 1, node.y].lXcY);
				}

				if ((node.y > 0 && graph [node.x, node.y - 1].moveable) && (node.y < mapHeight - 1 && graph [node.x, node.y + 1].moveable)) {
					initialState.Add ("passage", graph [node.x, node.y - 1].lXcY, graph [node.x, node.y + 1].lXcY);
					initialState.Add ("passage", graph [node.x, node.y + 1].lXcY, graph [node.x, node.y - 1].lXcY);
				}
			}
		}

		// Calculando vizinhança dos tiles
		for (int x = 0; x < mapWidth; x++) {
			for (int y = 0; y < mapHeight; y++) {
				if (graph [x, y].moveable) {
					#region 4-way connection version:
					if (x > 0 && graph [x - 1, y].moveable) {
						graph [x, y].neighbours.Add (graph [x - 1, y]);
						initialState.Add ("neighbour", graph [x, y].lXcY, graph [x - 1, y].lXcY);
					}
					if (x < mapWidth - 1 && graph [x + 1, y].moveable) {
						graph [x, y].neighbours.Add (graph [x + 1, y]);
						initialState.Add ("neighbour", graph [x, y].lXcY, graph [x + 1, y].lXcY);
					}

					if (y > 0 && graph [x, y - 1].moveable) {
						graph [x, y].neighbours.Add (graph [x, y - 1]);
						initialState.Add ("neighbour", graph [x, y].lXcY, graph [x, y - 1].lXcY);
					}
					if (y < mapHeight - 1 && graph [x, y + 1].moveable) {
						graph [x, y].neighbours.Add (graph [x, y + 1]);
						initialState.Add ("neighbour", graph [x, y].lXcY, graph [x, y + 1].lXcY);
					}
					#endregion
				}
			}
		}
	}

	//Classe para identificar tiles alcançáveis (Breadth-first search)
	public List<Node> TilesReachables (Unit unit, int movement)
	{
		Node start = graph [unit.unitPositionX, unit.unitPositionY];

		List<Node> visited = new List<Node> ();
		visited.Add (start);

		List<Node>[] fringes = new List<Node>[movement + 1];
		fringes [0] = new List<Node> ();
		fringes [0].Add (start);

		for (int i = 1; i <= movement; i++) {
			fringes [i] = new List<Node> ();

			foreach (Node node in fringes[i-1]) {
				foreach (Node neighbour in node.neighbours) {
					if (!visited.Contains (neighbour)) {
						visited.Add (neighbour);
						fringes [i].Add (neighbour);
					}
				}
			}
		}
		return visited;
	}


}
