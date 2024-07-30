using UnityEngine;
using UnityEngine.SceneManagement;

public class MainStory : MonoBehaviour
{
    void Start()
    {
        // Load and switch to scene "Play game" directly
        SceneManager.LoadScene("GamePlay", LoadSceneMode.Single);
    }
}
