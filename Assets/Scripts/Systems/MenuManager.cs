using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class MenuManager : MonoBehaviour
{
    public static MenuManager SM_Instance;

    public Canvas[] subCanvases;

    private void Start ()
    {
        if (SM_Instance == null)
            SM_Instance = this;
        else Destroy(this);
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void AddScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void ToggleCanvas(Canvas canvas)
    {
        for (int i = 0; i < subCanvases.Length; i++)
        {
            if(subCanvases[i] == canvas)
                subCanvases[i].gameObject.SetActive(true);
            else
                subCanvases[i].gameObject.SetActive(false);
        }
    }
}
