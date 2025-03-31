using UnityEngine;

public static class PanelManager
{
    public static void OpenClosePanels(GameObject open, GameObject close)
    {
        open.SetActive(true);
        close.SetActive(false);
    }
}
