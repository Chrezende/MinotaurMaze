using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class BuildMazeLayout : MonoBehaviour {

	public GameObject gridElement;
	public GameObject rowGameObject;
	public GameObject planCanvas;
	public Texture2D pixelMap;


	public int width;
	public int height;

	private void Start()
	{
		planCanvas = GameObject.Find("Plan Canvas");
	}

	public void GetWidth(string widthText)
	{
		if(int.TryParse(widthText, out width))
		{
			width = int.Parse(widthText);
		}
	}

	public void GetHeight(string heightText)
	{
		if (int.TryParse(heightText, out height))
		{
			height = int.Parse(heightText);
		}
	}

	public void CreateMazeLayout()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Destroy(transform.GetChild(i).gameObject);
		}

		for (int j = 0; j < height; j++)
		{
			GameObject row = Instantiate(rowGameObject, this.transform) as GameObject;
			row.name = "Row " + j;
			for (int i = 0; i < width; i++)
			{
				GameObject element = Instantiate(gridElement, row.transform) as GameObject;
				element.name = "Col " + i;
			}
		}
	}

	public void GeneratePixelMap()
	{
		pixelMap = new Texture2D(width, height);

		Color32[] pixelsColors = new Color32[width*height];
		int index = 0;
		for (int i = 0; i < height; i++)
		{
			Transform row = transform.GetChild(transform.childCount-(i+1));
			for (int j = 0; j < width; j++)
			{
				Transform cell = row.transform.GetChild(j);
				pixelsColors[index] = cell.GetComponent<Image>().color;
				index++;

			}
			
		}


		pixelMap.name = "maze";
		pixelMap.SetPixels32(pixelsColors);
		pixelMap.filterMode = FilterMode.Point;
		pixelMap.Apply();

		RunMazeGame();
	}

	public void RunMazeGame()
	{
		Time.timeScale = 0;
		GridContructor constructor = FindObjectOfType<GridContructor>();
		constructor.pixelMap = pixelMap;

		constructor.LoadMap();
		planCanvas.GetComponent<Canvas>().enabled = true;
		GetComponentInParent<Canvas>().enabled = false;


	}

}
