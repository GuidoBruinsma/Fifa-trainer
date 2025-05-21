using UnityEngine;

/// <summary>
/// Manages UI button actions such as signing out and quitting the application.
/// </summary>
public class ButtonsManager : MonoBehaviour
{
    private ButtonsManager instance;

    /// <summary>
    /// Ensures a single instance of this manager persists across scenes.
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(this.gameObject);
    }

    /// <summary>
    /// Signs the user out via the GameManager.
    /// </summary>
    public void SignOut()
    {
        GameManager.LogOut();
    }

    /// <summary>
    /// Quits the application and signs the user out.
    /// </summary>
    public void Quit()
    {
        Application.Quit();
        SignOut();
    }
}
