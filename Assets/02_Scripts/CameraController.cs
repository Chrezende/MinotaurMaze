using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    

	public void Focus(Transform target, float offset)
	{
		//Vector3 direction = transform.position - target.position;
		//direction = offset * direction.normalized;
		//Vector3 moveTo = target.position + direction;
		//transform.position = new Vector3(moveTo.x, 3f, moveTo.z);
        Vector3 direction = target.transform.position;
        transform.position = direction + new Vector3(0f, offset, 0f);
        transform.LookAt(target);
	}
}
