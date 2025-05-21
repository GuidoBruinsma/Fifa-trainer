using UnityEngine;
/// <summary>
/// Manages the activation and deactivation of UI panels and related elements like selectors.
/// </summary>
public static class PanelManager
{
    /// <summary>
    /// Opens one panel, closes another, and optionally activates a selector GameObject.
    /// </summary>
    /// <param name="open">The GameObject panel to open.</param>
    /// <param name="close">The GameObject panel to close.</param>
    /// <param name="hasSelector">Whether to activate a selector tagged GameObject.</param>
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

    /// <summary>
    /// Finds an inactive GameObject in the scene with the specified tag.
    /// </summary>
    /// <param name="tag">The tag to search for.</param>
    /// <returns>The first inactive GameObject with the given tag, or null if not found.</returns>
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
