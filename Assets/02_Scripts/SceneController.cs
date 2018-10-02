using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

	public int nextlevel;

	//Carrega a Scene de acordo com o valor recebido na chamada da função;
	public void LoadScene (int buildIndex)
	{
		SceneManager.LoadScene(buildIndex);
	}

	//Chamada da proxima Scene;
	public void LoadNextLevel () 
	{
		SceneManager.LoadScene(nextlevel);
	}

	//Encerramento do jogo;
	public void QuitRequest () 
	{
		Application.Quit();
	}
}
