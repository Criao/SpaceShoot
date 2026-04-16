using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TitleScreenUIHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private HowToPlayDialog howToPlayDialog;
    public void StartNewScene()
    {
        Debug.Log("按钮被点击了");
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

}
