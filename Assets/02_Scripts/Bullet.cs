using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

	void OnCollisionEnter (Collision col)
	{
		GetComponentInParent<Unit> ().unitBusy = false;
		Destroy (this.gameObject);
	}
}
