using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void OnStartButtonClick()
    {
        SceneManager.LoadScene("Main");
    }
}
