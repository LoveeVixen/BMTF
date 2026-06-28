// LOVEEVIXEN
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private GameObject container;

    private void Awake()
    {
        container = transform.GetChild(0).gameObject;
    }

    public void Open()
    {
        container.SetActive(true);
    }

    public void Close()
    {
        container.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
