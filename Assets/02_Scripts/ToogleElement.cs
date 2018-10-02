using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToogleElement : MonoBehaviour {

	Image thisImage;

	public Color toogleColor;

	// Use this for initialization
	void Start () {
		thisImage = GetComponent<Image>();
	
	}

	void Update()
	{
		GetComponent<BoxCollider>().size = new Vector3(GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height, 1f);
	}

	private void OnMouseDown()
	{
		toogleColor = GameObject.Find("Selected Object").GetComponent<Image>().color;
		thisImage.color = toogleColor;
	}

	private void OnMouseEnter()
	{
		if (Input.GetKey(KeyCode.Mouse0))
		{
			toogleColor = GameObject.Find("Selected Object").GetComponent<Image>().color;
			thisImage.color = toogleColor;
		}
	}

}
