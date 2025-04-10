using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public void LoadScene(int index) { 
        SceneManager.LoadScene(index);
    }
}
