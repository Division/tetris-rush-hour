using UnityEngine;
using System.Collections.Generic;

public class GamePlayController : MonoBehaviour {
	
	//**************************************************************************
	//
	// WaveProps
	//
	//**************************************************************************
	
	private class WaveProps
	{
		public float timeBeforeRush;
		public float rushDuration;
		public int bombsOnStart;
		public int waveIndex;
		public float fallInterval;
		
		public WaveProps(float inTimeBeforeRush, float inRushDuration, float inFallInterval, int inBombsOnStart, int inWaveIndex)
		{
			timeBeforeRush = inTimeBeforeRush;
			rushDuration = inRushDuration;
			bombsOnStart = inBombsOnStart;
			waveIndex = inWaveIndex;
			fallInterval = inFallInterval;
		}
	}
	
	//**************************************************************************
	//
	// Public
	//
	//**************************************************************************
	
	//-------------------
	// Const
	public const string HANDLE_ROWS_CLEARED = "HandleRowsCleared";
	public const float MIN_FALL_INTERVAL = 0.015f;
	public const float MAX_FALL_INTERVAL = 0.5f;
	
	//-------------------
	// Editor connections
	public BlockController blockController;
	public InGameUI gameUI;
	
	//-------------------
	// Properties
	public bool isRushing { get; set; }
	public int currentWaveIndex { get; set; }
	public int figuresSpawned { get; set; }
	public int score { get; set; }
	public System.Type nextFigureType { get; set; }
	
	public void Reset()
	{
		blockController.Reset();
		isRushing = false;
		figuresSpawned = 0;
		score = 0;
		currentWave = waveProps[0];
		waveStartTime = Time.time;
		PrepareNextFigure();
		currentWaveIndex = -1;
		//currentWaveIndex = 12;
		ApplyNextWave();
	}
	
	//--------------------------------------------------------------------------
	
	public void PrepareNextFigure()
	{
		nextFigureType = GetNextFigure();
		gameUI.UpdateNextFigure(nextFigureType);
	}
	
	//**************************************************************************
	//
	// MonoBehaviour
	//
	//**************************************************************************
	
	void Awake()
	{
		InitWaveProps();
		InitFigures();
	}
	
	//--------------------------------------------------------------------------
	
	void Update()
	{
		if (!blockController.enabled)
		{
			return;
		}
		
		UpdateState();
		UpdateUI();
	}
	
	//**************************************************************************
	//
	// Private
	//
	//**************************************************************************
	
	private List<WaveProps> waveProps;
	private WaveProps currentWave;
	private List<System.Type[]> figureTypes;
	private float waveStartTime;
	
	//**************************************************************************
	// Init
	
	private void InitWaveProps()
	{
 		waveProps = new List<WaveProps>();
		WaveProps prop;
		
		prop = new WaveProps(30, 8, 0.1f, 0, 0);
		waveProps.Add(prop);
		
		prop = new WaveProps(20, 12, 0.1f, 0, 1);
		waveProps.Add(prop);
		
		prop = new WaveProps(20, 10, 0.09f, 0, 2);
		waveProps.Add(prop);
		
		prop = new WaveProps(20, 10, 0.08f, 0, 3);
		waveProps.Add(prop);
		
		prop = new WaveProps(5, 15, 0.08f, 0, 4);
		waveProps.Add(prop);
		
		prop = new WaveProps(20, 15, 0.08f, 0, 5);
		waveProps.Add(prop);
		
		prop = new WaveProps(10, 10, 0.07f, 0, 7);
		waveProps.Add(prop);
		
		prop = new WaveProps(20, 7, 0.06f, 0, 8);
		waveProps.Add(prop);
		
		prop = new WaveProps(15, 10, 0.06f, 0, 9);
		waveProps.Add(prop);
		
		prop = new WaveProps(25, 5, 0.05f, 0, 10);
		waveProps.Add(prop);
		
		prop = new WaveProps(15, 7, 0.06f, 0, 11);
		waveProps.Add(prop);
		
		prop = new WaveProps(25, 10, 0.06f, 0, 14);
		waveProps.Add(prop);
		
		prop = new WaveProps(30, 10, 0.06f, 0, 15);
		waveProps.Add(prop);
	}
	
	//--------------------------------------------------------------------------
	
	private void InitFigures()
	{
		figureTypes = new List<System.Type[]>();
		
		/*figureTypes.Add(new System.Type[]
		{
			typeof(BlockShooter),
			typeof(ZigZagLeftFigure)
		});*/
		
		figureTypes.Add(new System.Type[]
		{
			typeof(PlankFigure),typeof(PlankFigure), typeof(PlankFigure), typeof(PlankFigure), 
			typeof(QuadFigure),typeof(QuadFigure),typeof(QuadFigure),typeof(QuadFigure),
			typeof(PiercingPointFigure),typeof(PiercingPointFigure),
			typeof(DoubleAngleFigure),typeof(DoubleAngleFigure),typeof(DoubleAngleFigure),typeof(DoubleAngleFigure),
			typeof(VerticalBombFigure),
			typeof(BlockShooter),typeof(BlockShooter),
			typeof(ZigZagLeftFigure),typeof(ZigZagLeftFigure),
			typeof(ZigZagRightFigure),typeof(ZigZagRightFigure),
			typeof(FigureGLeft),typeof(FigureGLeft),
			typeof(FigureGRight),typeof(FigureGRight),
		});
	}
	
	//**************************************************************************
	// State
	
	private void UpdateState()
	{
		if (isRushing)
		{
			int timeRemaining = GetRushRemainingTime();
			if (timeRemaining <= 0)
			{
				ApplyNextWave();
			}
		} 
		else {
			int timeRemaining = GetTimeBeforeRush();
			if (timeRemaining <= 0)
			{
				ApplyRushState();
			}
		}
	}
	
	//--------------------------------------------------------------------------
	
	private void ApplyRushState()
	{
		isRushing = true;
		blockController.fallInterval = GetFallInterval();
		BroadcastMessage(TetrisController.HANDLE_RUSH_STARTED);
	}
	
	//--------------------------------------------------------------------------
	
	private void ApplyNextWave()
	{
		waveStartTime = Time.time;
		isRushing = false;
		currentWaveIndex++;
		
		currentWave = waveProps[0];
		
		foreach (WaveProps wave in waveProps)
		{
			if (wave.waveIndex <= currentWaveIndex && wave.waveIndex > currentWave.waveIndex)
			{
				currentWave = wave;
			}
		}
		
		blockController.fallInterval = GetFallInterval();
		
		BroadcastMessage(TetrisController.HANDLE_RUSH_ENDED);
	}
	
	//**************************************************************************
	// Callbacks
	
	private void HandleRowsCleared(int rowsCount)
	{
		int scoreIncrease = GetScoreIncrease(rowsCount);
		score += scoreIncrease;
		gameUI.ShowScoreIncrease(scoreIncrease);
	}
	
	//**************************************************************************
	// Spawning
	
	private System.Type GetNextFigure()
	{
		System.Type result;
		
		int ind = 0;
		
		do {
			int index = Random.Range(0, figureTypes[0].Length);
			result = figureTypes[0][index];
			ind ++;
		} while (result == nextFigureType && ind < 10);
		
		return result;
	}
	
	//**************************************************************************
	// UI
	
	private void UpdateUI()
	{
		int rushTime = isRushing ? GetRushRemainingTime() : GetTimeBeforeRush();
		gameUI.UpdateTexts(score, currentWaveIndex + 1, GetCurrentSpeed(), rushTime, isRushing);
	}
	
	//**************************************************************************
	// Utils
	
	private int GetScoreIncrease(int numberRows)
	{
		float initialValue = 20;
		float rushBonus = isRushing ? 10 : 1;
		return (int)(initialValue * (currentWaveIndex + 1) * rushBonus * numberRows * numberRows);
	}
	
	//--------------------------------------------------------------------------
	
	private int GetTimeBeforeRush()
	{
		if (isRushing)
		{
			return 0;
		}
		
		return (int)(currentWave.timeBeforeRush - (Time.time - waveStartTime));
	}
	
	//--------------------------------------------------------------------------
	
	private int GetRushRemainingTime()
	{
		if (!isRushing)
		{
			return 0;
		}
		
		return (int)(currentWave.timeBeforeRush + currentWave.rushDuration - (Time.time - waveStartTime));
	}
	
	//--------------------------------------------------------------------------
	
	private float GetFallInterval()
	{
		float result;
		
		if (isRushing)
		{
			result = currentWave.fallInterval - 0.01f * GetWaveDelta();
		} else {
			result = MAX_FALL_INTERVAL - 0.02f * currentWaveIndex;
		}
		
		if (result < MIN_FALL_INTERVAL)
		{
			result = MIN_FALL_INTERVAL;
		}
		
		return result;
	}
	
	//--------------------------------------------------------------------------
	
	private int GetWaveDelta()
	{
		return currentWaveIndex - currentWave.waveIndex;
	}
	
	//--------------------------------------------------------------------------
	
	private int GetCurrentSpeed()
	{
		return (int)(10 / GetFallInterval());
	}
}
