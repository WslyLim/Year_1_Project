using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameOverUI : MonoBehaviour {

    private void Start() {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e) {
        if (GameManager.Instance.IsGameOver()) {
            Show();
			Time.timeScale = 0f;
        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
