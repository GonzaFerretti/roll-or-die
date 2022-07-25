using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject main;
    [SerializeField] private GameObject credits;
    
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void SetMenu(bool showCredits)
    {
        main.SetActive(!showCredits);
        credits.SetActive(showCredits);
    }
}
