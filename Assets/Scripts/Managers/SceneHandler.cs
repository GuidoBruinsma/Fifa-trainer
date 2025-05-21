using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles basic scene transitions and quitting the application.
/// </summary>
public class SceneHandler : MonoBehaviour
{
    /// <summary>
    /// Loads a scene by its build index.
    /// </summary>
    /// <param name="index">The build index of the scene to load.</param>
    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitApplication()
    {
        Application.Quit();
    }
}
