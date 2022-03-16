using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private NetworkingManager networkManager;

    private void OnApplicationQuit()
    {
        OnDisconnectClick();
    }

    public async void OnStartClick()
    {
        bool success = await networkManager.Connect();
        
        if(success)
        {
            uiManager.StartGame();
        }
    }

    public async void OnDisconnectClick()
    {
        bool success = await networkManager.Disconnect();

        if (success)
        {
            uiManager.QuitGame();
        }
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
        Application.Quit();
    }
}
