using UnityEngine;

public static class PanelManager
{
    public static void OpenClosePanels(GameObject open, GameObject close, bool hasSelector)
    {
        if (open != null) open.SetActive(true);
        if (close != null) close.SetActive(false);

        if (hasSelector)
        {
            GameObject selector = FindInactiveObjectWithTag("Selector");
            if (selector != null)
            {
                selector.SetActive(true);
            }
            else
            {
                Debug.LogWarning("No GameObject with tag 'Selector' found.");
            }
        }
    }
    private static GameObject FindInactiveObjectWithTag(string tag)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.CompareTag(tag) && !go.hideFlags.HasFlag(HideFlags.HideInHierarchy))
                return go;
        }
        return null;
    }
}
