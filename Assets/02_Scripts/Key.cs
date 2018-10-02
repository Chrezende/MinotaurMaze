using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
	private Unit unit;

	void OnTriggerEnter (Collider col)
	{
		unit = col.GetComponent<Unit> ();
	}

	public void Pick ()
	{
		unit.haveKey = true;
		Destroy (this.gameObject);
	}
}
