using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class shapeScript : MonoBehaviour 
{
	
	private LineRenderer line;
	private List<Vector3> tempShapeList;
	private List< List<Vector3> > dotCollection;

	private int currentDrawingShape;

	void Start () 
	{
		line = gameObject.AddComponent<LineRenderer> ();
		line.material = new Material (Shader.Find ("Particles/Additive"));
		line.SetVertexCount (0);
		line.SetWidth (0.2f, 0.2f);
		line.SetColors (Color.yellow, Color.yellow);
		line.useWorldSpace = true;
		dotCollection = new List<List<Vector3>> ();

		currentDrawingShape = 0;
		
		tempShapeList= readFromStringtoListVector3 ("0.0, 0.0, 2.0, 0.0, 0.0, 2.0, 0.0, 0.0,");
		dotCollection.Add (tempShapeList);

		tempShapeList= readFromStringtoListVector3 ("0.0, 0.0, 2.0, 0.0, 2.0, 2.0, 0.0, 2.0, 0.0, 0.0,");
		dotCollection.Add (tempShapeList);

		tempShapeList= readFromStringtoListVector3 ("0.0, 0.0, 2.0, 0.0, 5.0, 3.0, 3.0, 3.0, 0.0, 0.0,");
		dotCollection.Add (tempShapeList);

		tempShapeList= readFromStringtoListVector3 ("0.0, 0.0, 6.0, 0.0, 4.5, 3.0, 1.5, 3.0, 0.0, 0.0,");
		dotCollection.Add (tempShapeList);

		
	}

	public void enableLineRenderer(bool state)
	{
		line.enabled = state;
	}
	public void drawShape(int shapeNum)
	{
		if (shapeNum >= dotCollection.Count || shapeNum < 0)
			return;

		currentDrawingShape = shapeNum;

		line.SetVertexCount (dotCollection[shapeNum].Count+1);
		for (int i=0; i<dotCollection[shapeNum].Count; i++) 
		{
			line.SetPosition (i, dotCollection [shapeNum] [i]);
		}
		line.SetPosition (dotCollection[shapeNum].Count, dotCollection [shapeNum].First());
	}
	public void addPoints(string shapeString)
	{
		List<Vector3> tempShapeList;
		tempShapeList= readFromStringtoListVector3 (shapeString);
		
		dotCollection.Add (tempShapeList);
	}
	public void addPoints(List<Vector3> inputList)
	{
		if (inputList.Count > 0) 
		{
			List<Vector3> tempList = new List<Vector3> (inputList);
			if (inputList.Count > 0)
				dotCollection.Add (tempList);
		}
	}
	public void deleteLastPoint()
	{
		if (dotCollection.Count > 1)
			dotCollection.RemoveAt (dotCollection.Count - 1);
	}
	public int shapesCount()
	{
		return dotCollection.Count;
	}

	public List<Vector3>  currentFigure()
	{
		return dotCollection [currentDrawingShape];
	}

	public List<Vector3> readFromStringtoListVector3(string shapeString)
	{
		int counterBegin = 0;
		int counterEnd = 0;
		List<Vector3> temp=new List<Vector3>();
		List<float> tempFloatList=new List<float>();
		string tempString;
		
		while(counterBegin<shapeString.Length)
		{
			if((counterEnd=shapeString.IndexOf((','),counterBegin))>0)
			{
				if(counterEnd>counterBegin)
				{
					tempString=shapeString.Substring(counterBegin,counterEnd-counterBegin);
					tempFloatList.Add (Convert.ToSingle (tempString));
				}
				counterBegin=counterEnd+1;
			}
			else
				break;
		}
		
		for (int i=0; i<tempFloatList.Count; i+=2) 
		{
			temp.Add(new Vector3(tempFloatList[i],tempFloatList[i+1],0.0f));
			Debug.Log(temp[temp.Count-1]);
		}
		return temp;
	}
	
}
