using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseSystem : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public static bool IsPaused = false;

    [SerializeField] private Button optionsButton;


    private void Awake()
    {
        optionsButton.onClick.AddListener(() => {
            OptionsUI.Instance.Show();
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        pausePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (IsPaused) 
            {
                ResumeGame();    
            }
            else if(!IsPaused)
            {
                PauseGame();
            }
        }   
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale= 0f;
        IsPaused= true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale= 1f;
        IsPaused= false;
    }


    public void ExitGame()
    {
        Application.Quit();
    }
}
