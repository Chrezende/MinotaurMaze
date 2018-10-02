using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using UnityEngine.UI;


public class PlayerPlanner : MonoBehaviour
{

	public Unit thisUnit;
	public Player thisPlayer;
	public GridContructor grid;
	public MinotaurMaze.State kdb;

	[System.NonSerialized] private string problemInit = "";
	public string problemGoal;

	private Queue<string> planSteps;

	// Use this for initialization
	void Start()
	{
		thisUnit = GetComponent<Unit>();
		thisPlayer = GetComponent<Player>();
		grid = FindObjectOfType<GridContructor>();

		kdb = new MinotaurMaze.State(grid.initialState);


		planSteps = new Queue<string>();

		kdb.Add("visited", thisUnit.unitPositionText);
		if (thisPlayer.haveBomb)
		{
			grid.problemObjects += "\n\t\t" + "bomb - bomb" + "\n\t\t";
			kdb.Add("haveObject", "bomb");
		}

		problemGoal = "(and (atTile " + gameObject.name + " exit))";
		//StartPlanner();
	}


	public void StartPlanner()
	{
		GetPlannerProblemInit();
		WritePlanningProblem();
		SolvePlanningProblem();

		StartCoroutine("ExecutePlanningProblemSolution");
	}

	// Coloca todo o estado inicial em uma string
	void GetPlannerProblemInit()
	{
		problemInit = "";


		#region (neighbour tile1 tile2)
		Dictionary<string, List<string>> neighbourList = new Dictionary<string, List<string>>();
		if (kdb.ContainsRelation("neighbour"))
		{
			neighbourList = kdb.GetStateOfRelation("neighbour");
			foreach (var tile in neighbourList)
			{
				for (int i = 0; i < tile.Value.Count; i++)
				{
					problemInit += "(neighbour " + tile.Key + " " + tile.Value[i] + ")\t";
				}
				problemInit += "\n\t\t";
			}
		}
		#endregion

		#region (passage tile1 tile2)
		Dictionary<string, List<string>> passageList = new Dictionary<string, List<string>>();
		if (kdb.ContainsRelation("passage"))
		{
			passageList = kdb.GetStateOfRelation("passage");
			foreach (var tile in passageList)
			{
				for (int i = 0; i < tile.Value.Count; i++)
				{
					problemInit += "(passage " + tile.Key + " " + tile.Value[i] + ")\t";
				}
			}
			problemInit += "\n\t\t";
		}
		#endregion

		#region (objectOnMap object tile)
		Dictionary<string, List<string>> objectsList = new Dictionary<string, List<string>>();
		if (kdb.ContainsRelation("objectOnMap"))
		{
			objectsList = kdb.GetStateOfRelation("objectOnMap");
			foreach (var item in objectsList)
			{
				for (int i = 0; i < item.Value.Count; i++)
				{
					problemInit += "(objectOnMap " + item.Key + " " + item.Value[i] + ")\t";
				}
				problemInit += "\n\t\t";
			}
		}
		#endregion

		#region (visited ?tile) [inútil por enquanto]
		List<string> visitedList = new List<string>();
		if (kdb.ContainsVar("visited"))
		{
			visitedList = kdb.GetStateOfVar("visited");
			foreach (var tile in visitedList)
			{
				problemInit += "(visited " + tile + ")\t";
				problemInit += "\n\t\t";
			}
		}
		#endregion

		#region (tileSafe tile) [inútil por enquanto]
		List<string> tileSafeList = new List<string>();
		if (kdb.ContainsVar("tileSafe"))
		{
			tileSafeList = kdb.GetStateOfVar("tileSafe");
			foreach (var tile in tileSafeList)
			{
				problemInit += "(tileSafe " + tile + ")\t";
				problemInit += "\n\t\t";
			}
		}
		#endregion

		#region (atTile agent tile)
		Dictionary<string, List<string>> atTileList = new Dictionary<string, List<string>>();
		if (kdb.ContainsRelation("atTile"))
		{
			atTileList = kdb.GetStateOfRelation("atTile");
			foreach (var agent in atTileList)
			{
				for (int i = 0; i < agent.Value.Count; i++)
				{
					problemInit += "(atTile " + agent.Key + " " + agent.Value[i] + ")\t";
				}
				problemInit += "\n\t\t";
			}
		}
		#endregion

		#region (isAlive agent)
		List<string> isAliveList = new List<string>();
		if (kdb.ContainsVar("isAlive"))
		{
			isAliveList = kdb.GetStateOfVar("isAlive");
			foreach (var agent in isAliveList)
			{
				problemInit += "(isAlive " + agent + ")\t";
				problemInit += "\n\t\t";
			}
		}
		#endregion

		#region (haveObject object)
		List<string> haveObjectList = new List<string>();
		if (kdb.ContainsVar("haveObject"))
		{
			haveObjectList = kdb.GetStateOfVar("haveObject");
			foreach (var item in haveObjectList)
			{
				problemInit += "(haveObject " + item + ")\t";
				problemInit += "\n\t\t";
			}
		}
		#endregion

	}

	// Escreve arquivo de problema de planejamento usando os conhecimentos atuais
	void WritePlanningProblem()
	{
		//string tilesVisited = "";

		string planningProblem = "";
		planningProblem += "(define (problem maze)\n\t\n\t(:domain minotaur-maze)\n\n\t(:objects\n\t\t" + grid.problemObjects + "\t\n\t)\n\n\t(:init\n\t\t";
		planningProblem += problemInit + "\n\t)\n\n\t(:goal\n\t\t" + problemGoal + "\n\t)\n)";
		
		//GetComponent<TextHandler>().WriteString(planningProblem, "Assets/09_PDDL/p_minotaur-maze.pddl");
		GetComponent<TextHandler>().WriteString(planningProblem, Application.dataPath + "/Resources/p_minotaur-maze.pddl");
	}

	// Comanda o FF planner para resolver o problema externamente
	void SolvePlanningProblem()
	{
		Process process = new Process();
		process.StartInfo.UseShellExecute = true;
		//process.StartInfo.WorkingDirectory = "..\\Minotaur Maze\\Assets\\09_PDDL";
		process.StartInfo.WorkingDirectory = Application.dataPath + "/Resources";
		//UnityEngine.Debug.Log("Path: " + Application.dataPath + "/Resources");
		process.StartInfo.RedirectStandardOutput = false;
		process.StartInfo.CreateNoWindow = false;
		process.StartInfo.FileName = Application.dataPath + "/Resources/solvePlan.bat";
		process.Start();
		process.WaitForExit();
		ReadPlanningProblemSolution();
	}

	// Lê o arquivo de solução gerado pelo FF planner e coloca passos em um Queue
	void ReadPlanningProblemSolution()
	{
		//string textFilePath = "..\\Minotaur Maze\\Assets\\09_PDDL\\ffSolution.soln";
		string textFilePath = Application.dataPath + "/Resources/ffSolution.soln";
		var filestream = new System.IO.FileStream(textFilePath,
							 System.IO.FileMode.Open,
							 System.IO.FileAccess.Read,
							 System.IO.FileShare.ReadWrite);
		var file = new System.IO.StreamReader(filestream, System.Text.Encoding.UTF8, true, 128);

		string lineOfText = "";
		while ((lineOfText = file.ReadLine()) != null)
		{
			//Do something with the lineOfText
			string step = "";
			if (lineOfText.Split('(', ')').Length > 1)
			{
				step = lineOfText.Split('(', ')')[1];
				planSteps.Enqueue(step);
			}
		}
	}

	// Executa os passos da solução que estão no Queue
	IEnumerator ExecutePlanningProblemSolution()
	{
		while (planSteps.Count > 0)
		{
			yield return new WaitUntil(() => thisUnit.waitingTurn == false);

			if (GameObject.Find("Step-By-Step Toggle").GetComponent<Toggle>().isOn)
			{
				yield return new WaitUntil(() => FindObjectOfType<StepsController>().nextStep == true);
			}

			FindObjectOfType<StepsController>().nextStep = false;

			if (!SurroundingOk())
			{
				Replan();
			}

			yield return new WaitUntil(() => thisUnit.unitBusy == false);
			string thisStep = planSteps.Dequeue();
			string action = thisStep.Split(' ')[0];
			switch (action)
			{

				#region action moveTo
				case "MOVETO":
					//action moveTo
					//parameters: (?player - player ?from - tile ?to - tile ?minotaur - minotaur)
					//precondition:	(atTile ?player ?from) (neighbour ?from ?to)
					//				(or (not (isAlive ?minotaur))
					//					(not (atTile ?minotaur ?to))
					//				)
					//effect:	-(atTile ?from)
					//			+(atTile ?to)
					//			se (not (visited ?to))
					//				+(visited ?to)
					string thisTile = thisStep.Split(' ')[2];
					string tileToMove = thisStep.Split(' ')[3];
					Tile gameObjTileToMove = GameObject.Find(tileToMove.ToLower()).GetComponent<Tile>();

					if ((thisUnit.unitPositionText.ToLower() == thisTile.ToLower()) && (!(gameObjTileToMove.minotaurNear == 0) || !(GameObject.Find(thisStep.Split(' ')[4].ToLower()))))
					{
						kdb.Remove("atTile", gameObject.name, thisTile.ToLower());
						thisUnit.MoveUnitTo(gameObjTileToMove.tileX, gameObjTileToMove.tileY);
						kdb.Add("atTile", gameObject.name, tileToMove.ToLower());
						if (!kdb.CheckVar("visited", tileToMove.ToLower()))
						{
							kdb.Add("visited", tileToMove.ToLower());
						}
						yield return new WaitUntil(() => thisUnit.unitBusy == false);
					}
					else
					{
						Replan();
					}
					break;
				#endregion

				#region action pick
				case "PICK":
					//action pick
					//parameters: (?player - player ?object - object ?location - tile)
					//precondition: (atTile ?player ?location) (objectOnMap ?object ?location)
					//effect:		-(objectOnMap ?object ?location)
					//				+(haveObject ?object)
					if (thisUnit.unitPositionText.ToLower() == thisStep.Split(' ')[3].ToLower())
					{
						string itemName = thisStep.Split(' ')[2];
						// Procura a chave que está no mesmo tile do agente
						Collider[] itemInSpace = Physics.OverlapSphere(transform.position, 0.5f);
						for (int i = 0; i < itemInSpace.Length; i++)
						{
							if (itemInSpace[i].name == itemName.ToLower())
							{
								kdb.Remove("objectOnMap", itemName.ToLower(), thisStep.Split(' ')[3].ToLower());
								itemInSpace[i].GetComponent<Key>().Pick();
								kdb.Add("haveObject", itemName.ToLower());
							}
						}
					}
					else
					{
						Replan();
					}
					break;
				#endregion

				#region action usePassage
				case "USEPASSAGE":
					//action usePassage
					//parameters (?player - player ?from - tile ?to - tile ?key - key)
					//precondition (and (haveObject ?key) (atTile ?agent ?from) (passage ?from ?to))
					//effect	-(atTile ?agent ?from) -(haveObject ?key)
					//			+(atTile ?agent ?to)
					string tileFromName = thisStep.Split(' ')[2];
					string tileToName = thisStep.Split(' ')[3];
					Tile tileFrom = GameObject.Find(tileFromName.ToLower()).GetComponent<Tile>();
					Tile tileTo = GameObject.Find(tileToName.ToLower()).GetComponent<Tile>();

					if ((thisUnit.haveKey) && (thisUnit.unitPositionText.ToLower() == tileFromName.ToLower()))
					{
						// Adiciona conexão entre os tiles da passagem 
						grid.graph[tileFrom.tileX, tileFrom.tileY].neighbours.Add(grid.graph[tileTo.tileX, tileTo.tileY]);
						// Move o agente pela passagem
						kdb.Remove("atTile", gameObject.name, tileFromName.ToLower());
						kdb.Remove("haveObject", thisStep.Split(' ')[4].ToLower());
						thisUnit.MoveUnitTo(tileTo.tileX, tileTo.tileY);
						kdb.Add("atTile", gameObject.name, tileToName.ToLower());
						if (!kdb.CheckVar("visited", tileToName.ToLower()))
						{
							kdb.Add("visited", tileToName.ToLower());
						}
						yield return new WaitUntil(() => thisUnit.unitBusy == false);
						// Remove a conexão entre os tiles pois gastou a chave
						grid.graph[tileFrom.tileX, tileFrom.tileY].neighbours.Remove(grid.graph[tileTo.tileX, tileTo.tileY]);
					}
					else
					{
						Replan();
					}
					break;
				#endregion

				#region action throwBomb
				case "THROWBOMB":
					//action throwBomb
					//parameters (?tile - tile ?bomb - bomb ?minotaur - minotaur ?player - player ?pTile - tile)
					//precondition	(haveObject ?bomb)
					//				(isAlive ?minotaur)
					//				(atTile ?player ?pTile)
					//				(exists (?pNeighbour - tile)
					//						(and	(neighbour ?pTile ?pNeighbour)
					//								(neighbour ?pNeighbour ?tile)
					//						)
					//				)
					//				(or (atTile ?minotaur ?tile)
					//					(exists (?neighbours - tile) 
					//							(and	(neighbour ?tile ?neighbours)
					//									(atTile ?minotaur ?neighbours)
					//							)
					//					)
					//				) 
					//effect	-(haveObject ?bomb)
					//			-(isAlive ?minotaur)
					string tileToThrowBomb = thisStep.Split(' ')[1];
					Tile gameObjTileToThrowBomb = GameObject.Find(tileToThrowBomb.ToLower()).GetComponent<Tile>();
					if (thisPlayer.haveBomb && (GameObject.Find(thisStep.Split(' ')[3].ToLower())))
					{
						thisPlayer.ThrowBomb(gameObjTileToThrowBomb);
						yield return new WaitUntil(() => thisUnit.unitBusy == false);
						kdb.Remove("haveObject", thisStep.Split(' ')[2].ToLower());
						kdb.Remove("isAlive", thisStep.Split(' ')[3].ToLower());
					}
					else
					{
						Replan();
					}
					break;
				#endregion

				default:
					Replan();
					break;
			}
			
		}
		GameObject.Find("Play Canvas").GetComponent<Canvas>().enabled = false;
		GameObject.Find("Plan Finished Canvas").GetComponent<Canvas>().enabled = true;
	}

	bool SurroundingOk()
	{
		bool ok = true;
		List<Node> surroundingNodes = grid.TilesReachables(thisUnit, thisUnit.stench);
		// Para cada tile na área de visão do player
		foreach (var node in surroundingNodes)
		{
			Minotaur minotaur = FindObjectOfType<Minotaur>();
			if (minotaur)
			{
				// Se o minotauro está no tile
				if (GameObject.Find(node.lXcY).GetComponent<Tile>().minotaurNear == 0)
				{
					// e eu não sabia que ele tava vivo,
					if (!kdb.CheckVar("isAlive", minotaur.gameObject.name))
					{
						// agora sei que está vivo!
						kdb.Add("isAlive", minotaur.gameObject.name);
					}
					// e eu não sabia que ele estava nesse tile,
					if (!kdb.CheckRelation("atTile", minotaur.gameObject.name, node.lXcY))
					{
						// agora sei que está naquele tile, e tenho que repensar o plano.
						kdb.Add("atTile", minotaur.gameObject.name, node.lXcY);
						ok = false;
					}
				}
				// Se o minotauro não está no tile
				else
				{
					// e eu achava que ele estava nesse tile,
					if (kdb.CheckRelation("atTile", minotaur.gameObject.name, node.lXcY))
					{
						// agora sei que ele não está nesse tile, e tenho que repensar o plano.
						kdb.Remove("atTile", minotaur.gameObject.name, node.lXcY);
						ok = false;
					}
				}
			}
			else
			{
				// Minotauro está morto, então arrumo meu conhecimento se eu achava que ele estava em algum lugar.
				kdb.Remove("isAlive", "minotaur");
				kdb.Remove("atTile", "minotaur");
			}
		}
		return ok;
	}

	// Para a execução dos passos, limpa o Queue e pede para replanejar e executar novamente
	void Replan ()
	{
		StopCoroutine ("ExecutePlanningProblemSolution");
		planSteps.Clear ();
		StartPlanner ();
	}
}
