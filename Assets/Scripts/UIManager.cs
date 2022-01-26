using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField]
    private Canvas mainMenuCanvas;

    [Header("Match")]
    [SerializeField]
    private Canvas matchCanvas;
    [SerializeField]
    private Canvas pauseCanvas;
    [SerializeField]
    private Canvas gameCanvas;

    private void Awake()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        matchCanvas.gameObject.SetActive(false);
    }

    private void Start()
    {
        mainMenuCanvas.gameObject.SetActive(true);
    }

    public void StartGame()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        matchCanvas.gameObject.SetActive(true);
        pauseCanvas.gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        matchCanvas.gameObject.SetActive(false);
        mainMenuCanvas.gameObject.SetActive(true);
    }

    public void TogglePause()
    {
        pauseCanvas.gameObject.SetActive(!pauseCanvas.gameObject.activeSelf);
    }
}
