using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class GameEngine : MonoBehaviour
{
	public int window;
	public ParticleSystem _trail;
	public Camera _camera;
	public Text _score;
	public Text _timeLeft;
	
	private LineRenderer line;
	
	private List<Vector3> mousePointList;
	
	
	private Color mainColor;
	private Color grantedColor;
	private Color deniedColor;
	
	private bool isMousePress;
	private bool isAddShape;
	
	private Vector3 mousePos;
	
	private Vector2 timerPos;
	private Vector2 timerSize;
	public float timerMax = 100.0f;
	private float timer;
	private float timerDiff = 1.0f;
	
	private bool drawState;
	
	private GUIStyle labelStyle;
	private GUIStyle scoreStyle;
	private GUIStyle gameOverStyle;
	
	private int score;
	
	public string shapeField;
	
	
	private int numLevel;
	
	private shapeScript drawShapeScrypt;
	private float figureDiffRate;
	private float figureIndexRate;
	private float figureCompareRate;
	
	struct isStateChange
	{
		public bool isXDiff { get; set; }
		
		public bool isXDiffSignChange { get; set; }
		
		public bool isYDiff { get; set; }
		
		public bool isYDiffSignChange { get; set; }
		
		public bool Equals(isStateChange obj)
		{
			return
				(this.isXDiff == obj.isXDiff &&
				 this.isXDiffSignChange == obj.isXDiffSignChange &&
				 this.isYDiff == obj.isYDiff &&
				 this.isYDiffSignChange == obj.isYDiffSignChange);
		}
		
	};
	void Awake()
	{
		_trail.startLifetime = .2f;
		_trail.startSpeed = .1f;
		_trail.startSize = .5f;
		_trail.simulationSpace = ParticleSystemSimulationSpace.World;
		_trail.maxParticles = 30;
		_trail.startColor = new Color(0, 1, 1, 1);
		_trail.emissionRate = 0;
	}
	
	void Start()
	{
		_score.text = "";
		_timeLeft.text = "";
		mainColor = Color.cyan;
		grantedColor = Color.green;
		deniedColor = Color.red;
		
		line = gameObject.AddComponent<LineRenderer>();
		line.material = new Material(Shader.Find("Particles/Additive"));
		line.SetVertexCount(0);
		line.SetWidth(0.1f, 0.1f);
		line.SetColors(mainColor, mainColor);
		line.useWorldSpace = true;
		
		mousePointList = new List<Vector3>();
		
		//drawAreaRect = new Rect (shapeFieldRect.width + 80, 5, Screen.width - shapeFieldRect.width - 100, Screen.height-150);
		
		window = 1;
		
		timer = timerMax;
		
		isMousePress = false;
		isAddShape = false;
		drawState = false;
		
		
		score = 0;
		
		gameOverStyle = new GUIStyle();
		gameOverStyle.fontSize = 50;
		gameOverStyle.normal.textColor = Color.red;
		
		numLevel = 0;
		
		figureDiffRate = 10.0f;
		figureIndexRate = 1.0f;
		figureCompareRate = 5.0f;
		
		
		drawShapeScrypt = GameObject.Find("GameObject").GetComponent<shapeScript>();
	}
	
	void OnGUI()
	{
		GUI.BeginGroup(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200));
		if (window == 1)
		{
			if (GUI.Button(new Rect(10, 30, 180, 30), "Play"))
				window = 2;
			if (GUI.Button(new Rect(10, 70, 180, 30), "Exit"))
				window = 5;
		}
		
		GUI.EndGroup();
		
		if (window == 4)
		{
			drawState = false;
			line.enabled = false;
			drawShapeScrypt.enableLineRenderer(false);
			
			GUI.BeginGroup(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 100, 1000, 400));
			GUI.Label(new Rect(150.0f, 0.0f, 400.0f, 80.0f), "GAME OVER", gameOverStyle);
			GUI.Label(new Rect(180.0f, 80.0f, 300.0f, 50.0f), "Your score: " + score);
			if (GUI.Button(new Rect(200, 150, 200, 30), "Restart"))
			{
				timer = timerMax;
				timerDiff = 1.0f;
				_score.text = "";
				score = 0;
				line.SetVertexCount(0);
				mousePointList.Clear();
				window = 2;
			}
			if (GUI.Button(new Rect(200, 190, 200, 30), "Exit"))
				window = 5;
			GUI.EndGroup();
		}
		
		if (window == 5)
			Application.Quit();
		
		if (window == 2)
		{
			drawState = true;
			drawShapeScrypt.enableLineRenderer(true);
			
			if ((numLevel >= 0) && (numLevel < drawShapeScrypt.shapesCount()))
				drawShapeScrypt.drawShape(numLevel);
		}
		
	}
	
	
	
	void Update()
	{
		
		if (drawState == true)
			mousePaint(mousePointList);
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x,
		                                     Input.mousePosition.y);
		Vector3 mouse = _camera.ScreenToWorldPoint(curScreenPoint);
		_trail.transform.position = mouse;
		
		
		if (window == 2)
		{
			line.enabled = true;
			
			if (timer <= 0)
			{
				Debug.Log("Time is up");
				window = 4;
			}
			else
				timer -= Time.deltaTime;
			if (((int)(timer * 10f)) / 10f == (int)timer)
			{
				_timeLeft.text = (((int)(timer * 10f)) / 10f).ToString() + ".0";
			}
			else
			{
				_timeLeft.text = (((int)(timer * 10f)) / 10f).ToString();
			}
			
			if (!isMousePress && isAddShape)
			{
				if (pointsAnalyze(mousePointList))
				{
					numLevel = UnityEngine.Random.Range(0, drawShapeScrypt.shapesCount());
					
					line.SetColors(grantedColor, grantedColor);
					
					timer = timerMax - timerDiff;
					timerDiff += 5;
					
					score++;
					_score.text = "Score: " + score.ToString();
				}
				else
					line.SetColors(deniedColor, deniedColor);
				
				isAddShape = false;
			}
			
		}
		
		if (window == 7 || window == 6)
		{
			if (!isMousePress && isAddShape)
			{
				
				
				if (mousePointList.Count > 10)
				{
					List<Vector3> tempList;
					tempList = figureApproximation(mousePointList, figureDiffRate, figureIndexRate);
					
					if (tempList.Count > 0)
					{
						line.SetVertexCount(tempList.Count + 1);
						for (int i = 0; i < tempList.Count; i++)
							line.SetPosition(i, tempList[i]);
						line.SetPosition(tempList.Count, tempList.First());
						
						translateFigureToZero(tempList);
						drawShapeScrypt.addPoints(tempList);
					}
					isAddShape = false;
				}
			}
		}
	}
	
	public void mousePaint(List<Vector3> inputShapeList)
	{
		
		
		if (Input.GetMouseButtonDown(0))
		{
			_trail.emissionRate = 200;
			isMousePress = true;
			inputShapeList.Clear();
		}
		else if (Input.GetMouseButtonUp(0))
		{
			isMousePress = false;
			isAddShape = true;
		}
		
		
		if (isMousePress)
		{
			mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos.z = 0.0f;
			inputShapeList.Add(mousePos);
		}
		
	}
	
	public bool pointsAnalyze(List<Vector3> inputPointList)
	{
		List<Vector3> tempDrawList;
		
		if (inputPointList.Count <= 10)
			return false;
		tempDrawList = figureApproximation(inputPointList, figureDiffRate, figureIndexRate);
		
		if (tempDrawList.Count > 0)
		{
			line.SetVertexCount(tempDrawList.Count + 1);
			for (int i = 0; i < tempDrawList.Count; i++)
				line.SetPosition(i, tempDrawList[i]);
			line.SetPosition(tempDrawList.Count, tempDrawList.First());
			
			float scaleRatio = scaleRatioLeftToRightFigures(drawShapeScrypt.currentFigure(), tempDrawList);
			scaleFigure(tempDrawList, scaleRatio);
			translateFigureToZero(tempDrawList);
		}
		else
			return false;
		
		
		if (figuresCompare(drawShapeScrypt.currentFigure(), tempDrawList, figureCompareRate))
			return true;
		
		return false;
	}
	
	public void translateFigureToZero(List<Vector3> inputFigure)
	{
		if (inputFigure.Count <= 0)
			return;
		int minXIndex = 0;
		int minYIndex = 0;
		
		for (int i = 0; i < inputFigure.Count; i++)
		{
			if (inputFigure[minXIndex].x > inputFigure[i].x)
				minXIndex = i;
		}
		for (int i = 0; i < inputFigure.Count; i++)
		{
			if (inputFigure[minYIndex].y > inputFigure[i].y)
				minYIndex = i;
		}
		
		
		Vector3 minPoint = new Vector3(inputFigure[minXIndex].x, inputFigure[minYIndex].y, inputFigure[minXIndex].z);
		
		for (int i = 0; i < inputFigure.Count; i++)
		{
			inputFigure[i] = inputFigure[i] - minPoint;
		}
	}
	
	public void translateFigure(List<Vector3> inputFigure, Vector3 translatePoint)
	{
		if (inputFigure.Count <= 0)
			return;
		
		
		for (int i = 0; i < inputFigure.Count; i++)
		{
			inputFigure[i] = inputFigure[i] + translatePoint;
		}
	}
	
	private void scaleFigure(List<Vector3> inputFigure, float ratio)
	{
		if (inputFigure.Count <= 0)
			return;
		
		for (int i = 0; i < inputFigure.Count; i++)
			inputFigure[i] = inputFigure[i] * ratio;
	}
	
	private float scaleRatioLeftToRightFigures(List<Vector3> leftFigure, List<Vector3> rightFigure)
	{
		if (leftFigure.Count <= 0)
			return 0.0f;
		
		if (rightFigure.Count <= 0)
			return 0.0f;
		
		float leftFigureLenght = figureMagnitude(leftFigure);
		float rightFigureLenght = figureMagnitude(rightFigure);
		
		float scaleRatio = leftFigureLenght / rightFigureLenght;
		
		return scaleRatio;
	}
	
	private float figureMagnitude(List<Vector3> inputFigure)
	{
		if (inputFigure.Count <= 0)
			return 0.0f;
		
		int minXIndex = 0;
		int minYIndex = 0;
		
		int maxXIndex = 0;
		int maxYIndex = 0;
		
		Vector3 maxPoint;
		Vector3 minPoint;
		
		float figureLenght = 0.0f;
		
		for (int i = 0; i < inputFigure.Count; i++)
		{
			if (inputFigure[maxXIndex].x < inputFigure[i].x)
				maxXIndex = i;
		}
		for (int i = 0; i < inputFigure.Count; i++)
		{
			if (inputFigure[maxYIndex].x < inputFigure[i].x)
				maxYIndex = i;
		}
		
		
		for (int i = 0; i < inputFigure.Count; i++)
		{
			if (inputFigure[minXIndex].x > inputFigure[i].x)
				minXIndex = i;
		}
		for (int i = 0; i < inputFigure.Count; i++)
		{
			if (inputFigure[minYIndex].y > inputFigure[i].y)
				minYIndex = i;
		}
		
		maxPoint = new Vector3(inputFigure[maxXIndex].x, inputFigure[maxYIndex].y, inputFigure.First().z);
		minPoint = new Vector3(inputFigure[minXIndex].x, inputFigure[minYIndex].y, inputFigure.First().z);
		
		figureLenght = Vector3.Magnitude(maxPoint - minPoint);
		
		return figureLenght;
	}
	
	public bool figuresCompare(List<Vector3> leftFigure, List<Vector3> rightFigure, float pointDiff)
	{
		if (leftFigure.Count <= 0 || rightFigure.Count <= 0)
			return false;
		
		List<Vector3> tempLeftList = new List<Vector3>(leftFigure);
		List<Vector3> tempRightList = new List<Vector3>(rightFigure);
		
		
		
		float maxX = 0, maxY = 0;
		float minX = 0, minY = 0;
		
		float accuracyX = 0.0f, accuracyY = 0.0f;
		
		maxX = minX = leftFigure.First().x;
		maxY = minY = leftFigure.First().y;
		
		foreach (Vector3 x in leftFigure)
		{
			maxX = Mathf.Max(maxX, x.x);
			maxY = Mathf.Max(maxY, x.y);
			minX = Mathf.Min(minX, x.x);
			minY = Mathf.Min(minY, x.y);
		}
		
		accuracyX = Mathf.Abs((maxX - minX)) / pointDiff;
		accuracyY = Mathf.Abs((maxY - minY)) / pointDiff;
		
		for (int i = 0; i < tempLeftList.Count; i++)
		{
			int index;
			index = tempRightList.FindIndex(x
			                                =>
			                                {
				if (tempLeftList[i].x + accuracyX >= x.x && tempLeftList[i].x - accuracyX <= x.x)
				{
					if (tempLeftList[i].y + accuracyY >= x.y && tempLeftList[i].y - accuracyY <= x.y)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			});
			if (index == -1)
				return false;
		}
		return true;
		
	}
	
	private List<Vector3> figureApproximation(List<Vector3> incomigFigure, float accuracyRate, float indexRate)
	{
		
		List<Vector3> outputFigure = new List<Vector3>();
		float maxX = 0.0f, maxY = 0.0f;
		float minX = 0.0f, minY = 0.0f;
		
		float accuracyX = 0.0f, accuracyY = 0.0f;
		
		maxX = minX = incomigFigure.First().x;
		maxY = minY = incomigFigure.First().y;
		foreach (Vector3 x in incomigFigure)
		{
			maxX = Mathf.Max(maxX, x.x);
			maxY = Mathf.Max(maxY, x.y);
			minX = Mathf.Min(minX, x.x);
			minY = Mathf.Min(minY, x.y);
		}
		
		accuracyX = Mathf.Abs((maxX - minX)) / accuracyRate;
		accuracyY = Mathf.Abs((maxY - minY)) / accuracyRate;
		
		
		int currIndexPoint = 0;
		int indexStep = Convert.ToInt32((Mathf.Abs(maxX - minX) + Mathf.Abs(maxY - minY)) / indexRate);
		
		float Xdiff = 0.0f;
		float predXdiff = 0.0f;
		float Ydiff = 0.0f;
		float predYdiff = 0.0f;
		
		isStateChange shapeChange = new isStateChange();
		isStateChange shapeChangeBefore = new isStateChange();
		
		
		for (currIndexPoint = 0; currIndexPoint < incomigFigure.Count; currIndexPoint += indexStep)
		{
			if (incomigFigure.Count > currIndexPoint + indexStep)
			{
				Xdiff = incomigFigure[currIndexPoint].x - incomigFigure[currIndexPoint + indexStep].x;
				if (Mathf.Abs(Xdiff) > accuracyX)
				{
					shapeChange.isXDiff = true;
					if (Mathf.Sign(Xdiff) != Mathf.Sign(predXdiff))
						shapeChange.isXDiffSignChange = true;
					else
						shapeChange.isXDiffSignChange = false;
				}
				else
					shapeChange.isXDiff = false;
				
				
				Ydiff = incomigFigure[currIndexPoint].y - incomigFigure[currIndexPoint + indexStep].y;
				if (Mathf.Abs(Ydiff) > accuracyY)
				{
					shapeChange.isYDiff = true;
					if (Mathf.Sign(Ydiff) != Mathf.Sign(predYdiff))
						shapeChange.isYDiffSignChange = true;
					else
						shapeChange.isYDiffSignChange = false;
				}
				else
					shapeChange.isYDiff = false;
				
				if (!shapeChange.Equals(shapeChangeBefore))
					outputFigure.Add(incomigFigure[currIndexPoint + indexStep / 2]);
				
				predXdiff = Xdiff;
				predYdiff = Ydiff;
				shapeChangeBefore = shapeChange;
			}
		}
		
		return outputFigure;
	}
	
}
