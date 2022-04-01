using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    public static int activeLevel;
    public static bool isPlay;
    [SerializeField] GameObject gamePanel, level, winPanel, losePanel, menuPanel;
    // Start is called before the first frame update
    void Start()
    {

        if (PlayerPrefs.HasKey("activeLevel") == true) { activeLevel = PlayerPrefs.GetInt("activeLevel"); }
        else activeLevel = 0;

        if (isPlay == true)
        {
            StartGame();
        }
    }

    // Update is called once per frame
    public void StartGame()
    {
        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        level.SetActive(true);
    }




    public void WinPanel()
    {
        winPanel.SetActive(true);
    }

    public void NextLevel()
    {
        AdsController.Instance.ShowTransition();
        MenuController.isPlay = true;
        activeLevel++;
        SceneManager.LoadScene(0);
        PlayerPrefs.SetInt("activeLevel", activeLevel);
    }

    public void Reload()
    {
        MenuController.isPlay = true;
        SceneManager.LoadScene(0);
        PlayerPrefs.SetInt("activeLevel", activeLevel);

    }

    public void LevelSkip()
    {

        if (AdsController.Instance.ShowReward(RewardState.AddMoney))
        {
            WinPanel();
        }
        else
        {
            Debug.Log("Fail Skip Level");
        }
    }


    public void MainMenu()
    {
        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
        level.SetActive(false);
    }


    public void CloseMenu(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void OpenMenu(GameObject obj)
    {
        obj.SetActive(true);

    }
}
