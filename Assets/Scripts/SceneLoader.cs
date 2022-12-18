using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.Console;

public class SceneLoader : MonoBehaviour
{
    public void StartBtn()
    {
        //Debug.Log("1");
        SceneManager.LoadScene("MoodyNight");
    }
    public void LevelTwo()
    {
        SceneManager.LoadScene("Level2");
    }
    public void LevelThree()
    {
        SceneManager.LoadScene("Level3");
    }
    public void StartMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
