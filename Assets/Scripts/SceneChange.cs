using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [Header("References")]
    public string otherScene;

    public void GoToOtherScene()
    {
        SceneManager.LoadScene(otherScene);
    }
}
