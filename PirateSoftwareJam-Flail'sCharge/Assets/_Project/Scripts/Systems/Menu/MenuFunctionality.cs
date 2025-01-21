using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctionality : MonoBehaviour
{
    public void LoadLevel(int _levelIndex)
    {
        if (SceneManager.GetActiveScene().buildIndex == _levelIndex)
        {
            return;
        }
        SceneManager.LoadScene(_levelIndex);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

}
