using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class csSceneTrans : MonoBehaviour {

    public void GotoMain()
    {
        SceneManager.LoadScene("uiMain", LoadSceneMode.Single);
    }

    public void GotoLoading()
    {
        SceneManager.LoadScene("uiLoading", LoadSceneMode.Single);
    }

    public void GotoHelp()
    {
        SceneManager.LoadScene("uiHelp", LoadSceneMode.Single);
    }

    public void GotoGame()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
