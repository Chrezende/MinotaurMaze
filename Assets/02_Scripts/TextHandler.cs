using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using System.IO;

public class TextHandler : MonoBehaviour
{
	//public TextAsset asset;

	public void WriteString (string textToWrite, string path)
	{
		//Write some text to the ActionLog.txt file
		StreamWriter writer = new StreamWriter (path, false);
		writer.WriteLine (textToWrite);
		writer.Close ();

		////Re-import the file to update the reference in the editor
		//AssetDatabase.ImportAsset (path);
	}

	public void ReadString (string path)
	{
		//Read the text from directly from the AcionLog.txt file
		StreamReader reader = new StreamReader (path);
		Debug.Log (reader.ReadToEnd ());
		reader.Close ();
	}
}
