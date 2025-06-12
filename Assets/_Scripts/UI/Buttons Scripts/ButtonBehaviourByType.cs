using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonBehaviourByType : MonoBehaviour
{
    private enum ButtonBehaviour
    {
        EndSession,
        EndSessionContinue
    }

    [SerializeField] private ButtonBehaviour type;

    private Button b;

    private void Start()
    {
        b = GetComponent<Button>();

        b.onClick.AddListener(() =>
        {
            switch (type)
            {
                case ButtonBehaviour.EndSession:
                    GameManager.GameSessionEnd();
                    break;
                case ButtonBehaviour.EndSessionContinue:
                    EndSessionContinue();
                    break;
            }
        });
    }

    private void EndSessionContinue()
    {
        GlobalDataManager.ClearTempData();

        //if logged in, skip login page, go to menu
        SceneTransitionManager.LoadEndSessionSceneAndUnloadGameplay(0);
    }
}
