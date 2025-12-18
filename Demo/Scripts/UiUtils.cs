using UnityEngine;

public class UiUtils : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void OpenLink()
    {
        Application.OpenURL("https://github.com/cfrBernard/UnityLiteL10n");
    }
}
