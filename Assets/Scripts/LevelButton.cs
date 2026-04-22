using System.Diagnostics;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour
{
    public string levelName;

    LevelManager lm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lm = FindFirstObjectByType<LevelManager>();
        GetComponent<Button>().onClick.AddListener(() => SwitchLevels());
    }

    void SwitchLevels()
    {
        lm.ChangeNextLevel(levelName);
        lm.LoadNextLevel();
    }
}
