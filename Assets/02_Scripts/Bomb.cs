using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
	void OnDestroy ()
	{
		if (FindObjectOfType<Player>())
		{
			FindObjectOfType<Player>().GetComponent<Unit>().exploding = false;
		}
		
	}
}
