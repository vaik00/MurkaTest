using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class main : MonoBehaviour
{
	public int window;

	private Rect windowRect = new Rect (10, 10, Screen.width - 20, Screen.height - 20);
	private Rect labelRect = new Rect (15, 35, 150, 30);
	private Rect scoreRect = new Rect (15, 85, 150, 30);
	private Rect shapeFieldRect = new Rect (20, 150, 350, 20);
	private Rect sliderRect = new Rect (10, Screen.height - 250, 100, 20);
	private Rect drawAreaRect;

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

	public Texture2D progressBarEmpty;
	public Texture2D progressBarFull;

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

		public bool isXDiffSignChange{ get; set; }

		public bool isYDiff{ get; set; }

		public bool isYDiffSignChange{ get; set; }

		public bool Equals (isStateChange obj)
		{
			return 
				(this.isXDiff == obj.isXDiff &&
				this.isXDiffSignChange == obj.isXDiffSignChange &&
				this.isYDiff == obj.isYDiff &&
				this.isYDiffSignChange == obj.isYDiffSignChange);
		}

	};


	void Start ()
	{
		mainColor = Color.cyan;
		grantedColor = Color.green;
		deniedColor = Color.red;
		
		line = gameObject.AddComponent<LineRenderer> ();
		line.SetVertexCount (0);
		line.SetWidth (0.1f, 0.1f);
		line.SetColors (mainColor, mainColor);
		line.useWorldSpace = true;

		mousePointList = new List<Vector3> ();

		drawAreaRect = new Rect (shapeFieldRect.width + 80, 5, Screen.width - shapeFieldRect.width - 100, Screen.height-150);

		window = 1;

		timer = timerMax;

		isMousePress = false;
		isAddShape = false;
		drawState = false;
		
		timerPos = new Vector2 (100.0f, 35.0f);
		timerSize = new Vector2 (150.0f, 15.0f);
		
		progressBarEmpty = Texture2D.blackTexture;
		progressBarFull = Texture2D.whiteTexture;
		
		
		labelStyle = new GUIStyle ();
		labelStyle.fontSize = 30; 
		labelStyle.normal.textColor = Color.white;
		
		
		scoreStyle = new GUIStyle ();
		scoreStyle.fontSize = 40;
		scoreStyle.normal.textColor = new Color (1.0f, 0.1f, 0.1f);

		score = 0;
		
		gameOverStyle = new GUIStyle ();
		gameOverStyle.fontSize = 50;
		gameOverStyle.normal.textColor = Color.red;
		
		numLevel = 0;

		figureDiffRate = 10.0f;
		figureIndexRate = 1.0f;
		figureCompareRate = 5.0f;

		
		drawShapeScrypt = GameObject.Find ("GameObject").GetComponent<shapeScript> ();
	}
	
	void OnGUI ()
	{
		GUI.BeginGroup (new Rect (Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200));
		if (window == 1) 
		{
			if (GUI.Button (new Rect (10, 30, 180, 30), "Play"))
				window = 2;
			if (GUI.Button (new Rect (10, 70, 180, 30), "Shape Options"))
				window = 6;
			if (GUI.Button (new Rect (10, 110, 180, 30), "Exit"))
				window = 5;
		}

		///////////////////
		/// Pause GUI
		if (window == 3) 
		{
			drawState = false;

			if (GUI.Button (new Rect (10, 30, 180, 30), "Resume"))
				window = 2;
			if (GUI.Button (new Rect (10, 70, 180, 30), "Shape Options"))
				window = 7;
			if (GUI.Button (new Rect (10, 110, 180, 30), "Exit"))
				window = 5;
		}
		GUI.EndGroup ();
		

		////////////////
		/// Game over GUI
		if (window == 4) 
		{
			drawState = false;
			line.enabled = false;
			drawShapeScrypt.enableLineRenderer (false);

			GUI.BeginGroup (new Rect (Screen.width / 2 - 300, Screen.height / 2 - 100, 1000, 400));
			GUI.Label (new Rect (150.0f, 0.0f, 400.0f, 80.0f), "GAME OVER", gameOverStyle);
			GUI.Label (new Rect (180.0f, 80.0f, 300.0f, 50.0f), "Your score: " + score, scoreStyle);
			if (GUI.Button (new Rect (200, 150, 200, 30), "Restart")) 
			{
				timer = timerMax;
				timerDiff = 1.0f;
				score = 0;
				line.SetVertexCount (0);
				mousePointList.Clear ();
				window = 2;
			}
			if (GUI.Button (new Rect (200, 190, 200, 30), "Shape Options"))
				window = 6;
			if (GUI.Button (new Rect (200, 230, 200, 30), "Exit"))
				window = 5;
			GUI.EndGroup ();
		}

		if (window == 5)
			Application.Quit ();

		////////////////////
		/// Shape add GUI (main menu)

		if (window == 6) 
		{
			drawState=false;
			line.enabled=true;
			if(drawAreaRect.Contains (Event.current.mousePosition))
				drawState=true;

			GUI.Box (new Rect (5, 5, shapeFieldRect.width + 50, 250), "");
			GUI.Label (new Rect (shapeFieldRect.x, shapeFieldRect.y - shapeFieldRect.height - 30, shapeFieldRect.width, 60), shapeFieldDescription);
			shapeField = GUI.TextField (shapeFieldRect, "0.0, 0.0, 2.0, 0.0, 0.0, 2.0, 0.0, 0.0,");
			if (GUI.Button (new Rect (shapeFieldRect.x + shapeFieldRect.width / 2 - 100, shapeFieldRect.y + shapeFieldRect.height + 10, 200, 30), "Add shape"))
				drawShapeScrypt.addPoints (shapeField);
			
			if (GUI.Button (new Rect (10, 30, 180, 30), "Back"))
			{
				window = 1;
				mousePointList.Clear ();
				line.SetVertexCount (0);
			}
			
			GUI.Box (new Rect (5, Screen.height - 300, shapeFieldRect.width + 50, 250), "Figure options");

			figureDiffRate = GUI.HorizontalSlider (sliderRect, figureDiffRate, 1.0f, 30.0f);
			GUI.Label (new Rect(sliderRect.x+sliderRect.width+10, sliderRect.y-20,sliderRect.width+170,50),sliderDiffRateDescription);

			figureIndexRate = GUI.HorizontalSlider (new Rect (sliderRect.x,sliderRect.y+70,sliderRect.width,sliderRect.height), figureIndexRate, 1.0f, 10.0f);
			GUI.Label (new Rect(sliderRect.x+sliderRect.width+10, sliderRect.y+sliderRect.height + 30, sliderRect.width+170, 50),sliderIndexRateDescription);

			figureCompareRate = GUI.HorizontalSlider (new Rect (sliderRect.x,sliderRect.y+140,sliderRect.width,sliderRect.height), figureCompareRate, 1.0f, 10.0f);
			GUI.Label (new Rect(sliderRect.x+sliderRect.width+10, sliderRect.y+sliderRect.height*2 + 80, sliderRect.width+170, 50),sliderCompareRateDescription);
			
			GUI.Box (drawAreaRect, Texture2D.blackTexture);
			drawState = GUI.Toggle (new Rect (Screen.width-280, Screen.height - 30, 100, 20), drawState, "Рисовать");
			
			
			if (GUI.Button (new Rect (Screen.width - 400, Screen.height - 100, 300, 60), "Add drawing shape")) 
				isAddShape = true;
			if (GUI.Button (new Rect (Screen.width - 650, Screen.height - 100, 200, 60), "Delete last shape")) 
				drawShapeScrypt.deleteLastPoint ();

		}

		////////////////////
		/// Shape add GUI (pause)
		if (window == 7) 
		{
			drawState=false;
			line.enabled=true;
			if(drawAreaRect.Contains (Event.current.mousePosition))
				drawState=true;

			
			GUI.Box(new Rect(5,5,shapeFieldRect.width+50,250),"");
			GUI.Label (new Rect (shapeFieldRect.x, shapeFieldRect.y - shapeFieldRect.height - 30, shapeFieldRect.width, 60), shapeFieldDescription);
			shapeField = GUI.TextField (shapeFieldRect, shapeField);
			if (GUI.Button (new Rect (shapeFieldRect.x + shapeFieldRect.width / 2 - 100, shapeFieldRect.y + shapeFieldRect.height + 10, 200, 30), "Add shape"))
				drawShapeScrypt.addPoints (shapeField);
			
			
			if (GUI.Button (new Rect (10, 30, 180, 30), "Resume"))
			{
				window = 2;
				mousePointList.Clear ();
				line.SetVertexCount (0);
			}
					
			
			GUI.Box (new Rect (5, Screen.height - 300, shapeFieldRect.width + 50, 250), "Figure options");
			
			figureDiffRate = GUI.HorizontalSlider (sliderRect, figureDiffRate, 1.0f, 30.0f);
			GUI.Label (new Rect(sliderRect.x+sliderRect.width+10, sliderRect.y-20,sliderRect.width+170,50),sliderDiffRateDescription);
			
			figureIndexRate = GUI.HorizontalSlider (new Rect (sliderRect.x,sliderRect.y+70,sliderRect.width,sliderRect.height), figureIndexRate, 1.0f, 10.0f);
			GUI.Label (new Rect(sliderRect.x+sliderRect.width+10, sliderRect.y+sliderRect.height + 30, sliderRect.width+170, 50),sliderIndexRateDescription);
			
			figureCompareRate = GUI.HorizontalSlider (new Rect (sliderRect.x,sliderRect.y+140,sliderRect.width,sliderRect.height), figureCompareRate, 1.0f, 10.0f);
			GUI.Label (new Rect(sliderRect.x+sliderRect.width+10, sliderRect.y+sliderRect.height*2 + 80, sliderRect.width+170, 50),sliderCompareRateDescription);

			GUI.Box (drawAreaRect,Texture2D.blackTexture);
			
			
			if (GUI.Button (new Rect (Screen.width-400, Screen.height-100, 300, 60), "Add drawing shape"))
			{
				isAddShape=true;
			}
			drawState = GUI.Toggle(new Rect(Screen.width,Screen.height-30,50,20),drawState,"Рисовать");
		}

		
		////////////////////
		/// main window GUI
		if (window == 2) 
		{
			windowRect = GUI.Window (0, windowRect, WindowFunction, "");
			drawShapeScrypt.enableLineRenderer (true);

			if ((numLevel >= 0) && (numLevel < drawShapeScrypt.shapesCount ()))
				drawShapeScrypt.drawShape (numLevel);
		}

	}

	void WindowFunction (int windowId)
	{
		drawState=false;
		if(windowRect.Contains (Event.current.mousePosition))
			drawState=true;



		GUI.Label (labelRect, "Timer", labelStyle);
		
		//Timer
		GUI.BeginGroup (new Rect (windowRect.x, windowRect.y, timerSize.x + labelRect.width, timerSize.y + labelRect.height));
		GUI.DrawTexture (new Rect (timerPos.x + labelRect.x, timerPos.y, timerSize.x, timerSize.y), progressBarEmpty);
		GUI.DrawTexture (new Rect (timerPos.x + labelRect.x, timerPos.y, timerSize.x * (timer / timerMax), timerSize.y), progressBarFull);
		GUI.EndGroup ();
		
		//Score
		GUI.Label (scoreRect, "Score:", labelStyle);
		GUI.Label (new Rect (scoreRect.x + scoreRect.width, scoreRect.y, 50.0f, scoreRect.height), score.ToString (), scoreStyle);
		
		
		
		if (GUI.Button (new Rect (15.0f, windowRect.height - 50, 180, 30), "Pause")) 
		{
			if (window == 2) {
				drawShapeScrypt.enableLineRenderer (false);
				window = 3;
			} else {
				drawShapeScrypt.enableLineRenderer (true);
				window = 2;
			}
		}
		
		//Change figure by button
		if (GUI.Button (new Rect (15.0f, windowRect.height - 100, 180, 30), "Change shape")) 
		{
			numLevel = UnityEngine.Random.Range (0, drawShapeScrypt.shapesCount ());
		}
	}

	void Update ()
	{
		///////////////////////////////
		/// mouse drawing
		if (drawState == true)
			mousePaint (mousePointList);
		////////////////////////////////////




		//////////////////////////
		//main window render
		if (window == 2) 
		{
			line.enabled=true;

			if (timer <= 0.0f) {
				Debug.Log ("Time is up");
				window = 4;
			} else 
				timer -= Time.deltaTime;

			if (!isMousePress && isAddShape) 
			{
				if (pointsAnalyze (mousePointList)) 
				{
					numLevel = UnityEngine.Random.Range (0, drawShapeScrypt.shapesCount ());

					line.SetColors (grantedColor,grantedColor);

					timer = timerMax - timerDiff;
					timerDiff += 5;

					score++;
				}
				else
					line.SetColors (deniedColor,deniedColor);

				isAddShape = false;
			}

		}
		//////////////////////////////////////



		//////////////////////////////////////
		/// add figures by drawing
		if (window == 7 || window == 6) 
		{
			if (!isMousePress && isAddShape) 
			{

			
				if (mousePointList.Count > 10) 
				{
					List<Vector3> tempList;
					tempList = figureApproximation (mousePointList, figureDiffRate, figureIndexRate);

					if(tempList.Count>0)
					{
						line.SetVertexCount (tempList.Count + 1);
						for (int i=0; i<tempList.Count; i++)
							line.SetPosition (i, tempList [i]);
						line.SetPosition (tempList.Count, tempList.First());

						translateFigureToZero (tempList);
						drawShapeScrypt.addPoints (tempList);
					}
					isAddShape = false;
				}
			}

		}
		///////////////////////////


	}

	public void mousePaint (List<Vector3> inputShapeList)
	{


		if (Input.GetMouseButtonDown (0)) 
		{
			isMousePress = true;
			inputShapeList.Clear ();
			line.SetVertexCount (0);
			line.SetColors (mainColor, mainColor);
		} else if (Input.GetMouseButtonUp (0)) 
		{
			isMousePress = false;
			isAddShape = true;
		}
		
		
		if (isMousePress) 
		{
			mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			mousePos.z = 0.0f;
			inputShapeList.Add (mousePos);
			
			line.SetVertexCount (inputShapeList.Count);
			line.SetPosition (inputShapeList.Count - 1, inputShapeList [inputShapeList.Count - 1]);
		}

	}

	public bool pointsAnalyze (List<Vector3> inputPointList)
	{
		List<Vector3> tempDrawList;

		if (inputPointList.Count <= 10)
			return false;

		tempDrawList = figureApproximation (inputPointList, figureDiffRate, figureIndexRate);



		if (tempDrawList.Count > 0) 
		{
			line.SetVertexCount (tempDrawList.Count + 1);
			for (int i=0; i<tempDrawList.Count; i++)
				line.SetPosition (i, tempDrawList [i]);
			line.SetPosition (tempDrawList.Count, tempDrawList.First());
		
			float scaleRatio = scaleRatioLeftToRightFigures (drawShapeScrypt.currentFigure (), tempDrawList);
			scaleFigure (tempDrawList, scaleRatio);
			translateFigureToZero (tempDrawList);
		} else
			return false;


		if (figuresCompare (drawShapeScrypt.currentFigure (), tempDrawList, figureCompareRate)) 
			return true;

		return false;
	}


	/// <summary>
	/// Translates the figure to origin.
	/// </summary>
	/// <param name="inputFigure">Input figure.</param>
	public void translateFigureToZero (List<Vector3> inputFigure)
	{
		if (inputFigure.Count <= 0)
			return;
		int minXIndex = 0;
		int minYIndex = 0;

		for (int i=0; i<inputFigure.Count; i++) {
			if (inputFigure [minXIndex].x > inputFigure [i].x)
				minXIndex = i;
		}
		for (int i=0; i<inputFigure.Count; i++) {
			if (inputFigure [minYIndex].y > inputFigure [i].y)
				minYIndex = i;
		}


		Vector3 minPoint = new Vector3 (inputFigure [minXIndex].x, inputFigure [minYIndex].y, inputFigure [minXIndex].z);

		for (int i=0; i<inputFigure.Count; i++) {
			inputFigure [i] = inputFigure [i] - minPoint;

		}

	}

	/// <summary>
	/// Translates the figure by the second param.
	/// </summary>
	/// <param name="inputFigure">Input figure.</param>
	/// <param name="translatePoint">Translate point.</param>
	public void translateFigure (List<Vector3> inputFigure, Vector3 translatePoint)
	{
		if (inputFigure.Count <= 0)
			return;


		for (int i=0; i<inputFigure.Count; i++) 
		{
			inputFigure [i] = inputFigure [i] + translatePoint;	
		}
		
	}

	/// <summary>
	/// Scales the inputFigure by ratio.
	/// </summary>
	/// <param name="inputFigure">Input figure.</param>
	/// <param name="ratio">Ratio.</param>
	private void scaleFigure (List<Vector3> inputFigure, float ratio)
	{
		if (inputFigure.Count <= 0)
			return;

		for (int i=0; i<inputFigure.Count; i++)
			inputFigure [i] = inputFigure [i] * ratio;

	}


	/// <summary>
	/// Return scale ratio left to right figures. (left/right)
	/// </summary>
	/// <returns>The ratio left to right figures.</returns>
	/// <param name="leftFigure">Left figure.</param>
	/// <param name="rightFigure">Right figure.</param>
	private float scaleRatioLeftToRightFigures (List<Vector3> leftFigure, List<Vector3> rightFigure)
	{
		if (leftFigure.Count <= 0)
			return 0.0f;

		if (rightFigure.Count <= 0)
			return 0.0f;

		float leftFigureLenght = figureMagnitude (leftFigure);
		float rightFigureLenght = figureMagnitude (rightFigure);

		float scaleRatio = leftFigureLenght / rightFigureLenght;

		return scaleRatio;
	}


	/// <summary>
	/// Calculate figure magnitude. (Distance between min left point and max right)
	/// </summary>
	/// <returns>The magnitude.</returns>
	/// <param name="inputFigure">Input figure.</param>
	private float figureMagnitude (List<Vector3> inputFigure)
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

		for (int i=0; i<inputFigure.Count; i++) {
			if (inputFigure [maxXIndex].x < inputFigure [i].x)
				maxXIndex = i;
		}
		for (int i=0; i<inputFigure.Count; i++) {
			if (inputFigure [maxYIndex].x < inputFigure [i].x)
				maxYIndex = i;
		}


		for (int i=0; i<inputFigure.Count; i++) {
			if (inputFigure [minXIndex].x > inputFigure [i].x)
				minXIndex = i;
		}
		for (int i=0; i<inputFigure.Count; i++) {
			if (inputFigure [minYIndex].y > inputFigure [i].y)
				minYIndex = i;
		}

		maxPoint = new Vector3 (inputFigure [maxXIndex].x, inputFigure [maxYIndex].y, inputFigure.First ().z);
		minPoint = new Vector3 (inputFigure [minXIndex].x, inputFigure [minYIndex].y, inputFigure.First ().z);

		figureLenght = Vector3.Magnitude (maxPoint - minPoint);

		return figureLenght;
	}



	/// <summary>
	/// Compare two figures.
	/// </summary>
	/// <returns><c>true</c>, if compare was figuresed, <c>false</c> otherwise.</returns>
	/// <param name="leftFigure">Left figure.</param>
	/// <param name="rightFigure">Right figure.</param>
	/// <param name="pointDiff">Point diff.</param>  larger more accurately
	public bool figuresCompare (List<Vector3> leftFigure, List<Vector3> rightFigure, float pointDiff)
	{
		if (leftFigure.Count <= 0 || rightFigure.Count <= 0)
			return false;

		List<Vector3> tempLeftList = new List<Vector3> (leftFigure);
		List<Vector3> tempRightList = new List<Vector3> (rightFigure);



		float maxX = 0, maxY = 0;
		float minX = 0, minY = 0;

        float accuracyX = 0.0f, accuracyY = 0.0f;
		
		maxX = minX = leftFigure.First ().x;
		maxY = minY = leftFigure.First ().y;

		foreach (Vector3 x in leftFigure) 
		{
			maxX = Mathf.Max (maxX, x.x);
			maxY = Mathf.Max (maxY, x.y);
			minX = Mathf.Min (minX, x.x);
			minY = Mathf.Min (minY, x.y);
		}

        accuracyX = Mathf.Abs((maxX - minX)) / pointDiff;
        accuracyY = Mathf.Abs((maxY - minY)) / pointDiff;

		for (int i=0; i<tempLeftList.Count; i++) 
		{
			int index;
			index = tempRightList.FindIndex (x
			                        =>
			{
                if (tempLeftList[i].x + accuracyX >= x.x && tempLeftList[i].x - accuracyX <= x.x)
                {
                    if (tempLeftList[i].y + accuracyY >= x.y && tempLeftList[i].y - accuracyY <= x.y)
                    {
						return true;
					} else {
						return false;
					}
				} else {
					return false;
				}
			});
			if (index == -1)
				return false;
		}
		return true;

	}

	/// <summary>
	/// Approximate figure.
	/// </summary>
	/// <returns>The approximation.</returns>
	/// <param name="incomigFigure">Incomig figure.</param>
    private List<Vector3> figureApproximation(List<Vector3> incomigFigure, float accuracyRate, float indexRate)
	{

		List<Vector3> outputFigure = new List<Vector3> ();
		float maxX = 0.0f, maxY = 0.0f;
		float minX = 0.0f, minY = 0.0f;

        float accuracyX = 0.0f, accuracyY = 0.0f;
		
		maxX = minX = incomigFigure.First ().x;
		maxY = minY = incomigFigure.First ().y;
		foreach (Vector3 x in incomigFigure) {
			maxX = Mathf.Max (maxX, x.x);
			maxY = Mathf.Max (maxY, x.y);
			minX = Mathf.Min (minX, x.x);
			minY = Mathf.Min (minY, x.y);
		}

        accuracyX = Mathf.Abs((maxX - minX)) / accuracyRate;
        accuracyY = Mathf.Abs((maxY - minY)) / accuracyRate;
		

		int currIndexPoint = 0;
		int indexStep = Convert.ToInt32 ((Mathf.Abs (maxX - minX) + Mathf.Abs (maxY - minY)) / indexRate);
		
		float Xdiff = 0.0f;
		float predXdiff = 0.0f;
		float Ydiff = 0.0f;
		float predYdiff = 0.0f;
		
		isStateChange shapeChange = new isStateChange ();
		isStateChange shapeChangeBefore = new isStateChange ();
		

		for (currIndexPoint=0; currIndexPoint<incomigFigure.Count; currIndexPoint+=indexStep) {
			if (incomigFigure.Count > currIndexPoint + indexStep) {
				Xdiff = incomigFigure [currIndexPoint].x - incomigFigure [currIndexPoint + indexStep].x;
                if (Mathf.Abs(Xdiff) > accuracyX)
                {
					shapeChange.isXDiff = true;
					if (Mathf.Sign (Xdiff) != Mathf.Sign (predXdiff))
						shapeChange.isXDiffSignChange = true;
					else
						shapeChange.isXDiffSignChange = false;
				} else
					shapeChange.isXDiff = false;
				
				
				Ydiff = incomigFigure [currIndexPoint].y - incomigFigure [currIndexPoint + indexStep].y;
                if (Mathf.Abs(Ydiff) > accuracyY)
                {
					shapeChange.isYDiff = true;
					if (Mathf.Sign (Ydiff) != Mathf.Sign (predYdiff))
						shapeChange.isYDiffSignChange = true;
					else
						shapeChange.isYDiffSignChange = false;
				} else
					shapeChange.isYDiff = false;
				
				if (!shapeChange.Equals (shapeChangeBefore))
					outputFigure.Add (incomigFigure [currIndexPoint + indexStep / 2]);
				
				predXdiff = Xdiff;
				predYdiff = Ydiff;
				shapeChangeBefore = shapeChange;
			}
		}

		return outputFigure;
	}
	
}
