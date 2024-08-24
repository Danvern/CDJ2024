using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private string sceneName;

    // This method can be called via a UnityEvent to load the specified scene
    public void Load()
    {
		Time.timeScale = 1.0f; //TODO: normalized timescale manipulation
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is empty! Please assign a scene name.");
        }
    }

    // Optional method to set the scene name programmatically
    public void SetSceneName(string newSceneName)
    {
        sceneName = newSceneName;
    }
}
