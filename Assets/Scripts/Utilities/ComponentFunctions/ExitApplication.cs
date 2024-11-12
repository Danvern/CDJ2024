using UnityEngine;

public class ExitApplication : MonoBehaviour
{
    public void Exit()
    {
        Application.Quit();

        #if UNITY_EDITOR
        Debug.Log("Application.Quit() called. The application would exit if this were a build.");
        #endif
    }
}
