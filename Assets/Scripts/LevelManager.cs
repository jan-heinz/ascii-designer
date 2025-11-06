using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string nextLevel;
    void LoadSceneByName(string name) {
        SceneManager.LoadScene(name);
    }

    public void ChangeNextLevel(string name) {
        nextLevel = name;
    }

    public void LoadNextLevel()
    {
        if (nextLevel.Length > 0) {
            LoadSceneByName(nextLevel);
        } else {
            Debug.Log("No next level scene specified");
        }
    }
}
