using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // P�epne sc�nu na hru
    public void PlayGame()
    {
        Game.difficulty = 0;
        SceneManager.LoadScene("Game");
    }

    // Ukon�� aplikaci
    public void QuitGame()
    {
        Application.Quit();
    }

    public void Difficulty1()
    {
        Game.difficulty = 1;
        SceneManager.LoadScene("Game");
    }
    public void Difficulty2()
    {
        Game.difficulty = 2;
        SceneManager.LoadScene("Game");
    }
    public void Difficulty3()
    {
        Game.difficulty = 3;
        SceneManager.LoadScene("Game");
    }
}
