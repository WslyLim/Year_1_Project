using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour {

    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    private void Awake() {
        startButton.onClick.AddListener(() => {
			Loader.Load(Loader.Scene.InGameScene);
		});
        quitButton.onClick.AddListener(() => {
		    Application.Quit();
		});
		
		Time.timeScale = 1f;
    }
}