using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject HintsPanel = null;

    static bool hideHints = false;

    private void Start()
    {
        if (hideHints)
        {
            HintsPanel.SetActive(false);
        }
    }

    public void StartGame()
    {
        hideHints = true;
        HintsPanel.SetActive(false);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

}
