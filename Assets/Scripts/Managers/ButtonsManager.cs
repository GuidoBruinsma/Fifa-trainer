using UnityEngine;

public class ButtonsManager : MonoBehaviour
{
    private ButtonsManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(this.gameObject);
    }

    public void SignOut()
    {
        GameManager.LogOut();
    }
}
