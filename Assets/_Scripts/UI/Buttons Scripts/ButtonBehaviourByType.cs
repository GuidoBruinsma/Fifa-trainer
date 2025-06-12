using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonBehaviourByType : MonoBehaviour
{
    private enum ButtonBehaviour
    {
        LoadScene,
        EndSessionContinue
    }

    [SerializeField] private ButtonBehaviour type;

    [Tooltip("Leave -1 if type != LoadScene")]
    [Header("Leave -1 if type != LoadScene")]
    [SerializeField] private int loadSceneIndex = -1;

    private Button b;

    private void Start()
    {
        b = GetComponent<Button>();

        b.onClick.AddListener(() =>
        {
            switch (type)
            {
                case ButtonBehaviour.LoadScene:
                    LoadScene(loadSceneIndex);
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

        SceneTransitionManager.LoadEndSessionSceneAndUnloadGameplay(0);
    }

    private void LoadScene(int index)
    {
        if (index < 0 || index > SceneManager.sceneCountInBuildSettings - 1)
        {
            Debug.LogError($"Invalid scene index on {transform.parent.name}");
            return;
        }
        else {
            SceneTransitionManager.LoadEndSessionSceneAndUnloadGameplay(index);
        }
    }
}
