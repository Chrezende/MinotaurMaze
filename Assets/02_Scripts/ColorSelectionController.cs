using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelectionController : MonoBehaviour {

	public GameObject selectedObject;

	public Color32 thisColor;

	private void Start()
	{
		selectedObject = GameObject.Find("Selected Object");
		thisColor = GetComponent<Image>().color;
	}

	public void SelectColor()
	{
		selectedObject.GetComponent<Image>().color = thisColor;
	}

}
