using UnityEngine;

public class HideIfWeb : MonoBehaviour
{
    void Start()
    {
        // Check if the application is running on WebGL/HTML platform
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // Deactivate the GameObject this script is attached to
            gameObject.SetActive(false);
        }
    }
}
