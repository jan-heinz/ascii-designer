using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string nextLevel;

    //load a new scene based on name
    void LoadSceneByName(string name) {
        SceneManager.LoadScene(name);
    }

    //changes the level name
    public void ChangeNextLevel(string name) {
        nextLevel = name;
    }

    //loads the scene specified by nextLevel
    public void LoadNextLevel()
    {
        if (nextLevel.Length > 0) {
            LoadSceneByName(nextLevel);
        } else {
            Debug.Log("No next level scene specified");
        }
    }
}
