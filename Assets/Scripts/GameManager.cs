using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour{
	
	public static GameManager Instance { get; private set; }
	
	
	public event EventHandler OnStateChanged;
	public event EventHandler OnGamePaused;
	public event EventHandler OnGameUnpaused;
	
	private enum State {
		GamePlaying,
		GameOver,
	}
	
	private State state;
	private float gamePlayingTimer;
	private float gamePlayingTimerMax = 10f;
	private bool isGamePaused = false;
	
	private void Awake() {
		Instance = this;
		gamePlayingTimer = gamePlayingTimerMax;
		state = State.GamePlaying;
	}
	
	private void Start() {
		GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
	}
	
	private void GameInput_OnPauseAction(object sender, EventArgs e) {
		TogglePauseGame();
	}
	
	private void Update() {
		switch (state) {
			case State.GamePlaying:
				gamePlayingTimer -= Time.deltaTime;
				if (gamePlayingTimer < 0f) {
					state = State.GameOver;
					OnStateChanged?.Invoke(this, EventArgs.Empty);
				}
				break;
			case State.GameOver:
				break;
		}
	}
	
	public bool IsGameOver() {
		return state == State.GameOver;
	}
	
	public float GetGamePlayingTimerNormalized(){
		return 1 - gamePlayingTimer / gamePlayingTimerMax;
	}
	
	public void TogglePauseGame(){
		isGamePaused = !isGamePaused;
		if (isGamePaused) {
			Time.timeScale = 0f;
			OnGamePaused?.Invoke(this, EventArgs.Empty);
		} else {
			Time.timeScale = 1f;
			OnGameUnpaused?.Invoke(this, EventArgs.Empty);
		}
	}
}
