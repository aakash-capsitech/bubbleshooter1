using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    void Start()
    {
        Button btn = GetComponent<Button>();

        btn.onClick.AddListener(ExitGame);
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked! Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
