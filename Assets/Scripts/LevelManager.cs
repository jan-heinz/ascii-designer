using System.IO.Compression;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public string nextLevel;

    [Header("Player Balance")]
    // unique to each level
    // total amount of money player needs to manage
    [SerializeField] private int startingBalance = 1000;
    public TextMeshProUGUI balanceText;

    private int currentBalance;


    void Start()
    {
        currentBalance = startingBalance;
        balanceText.text = currentBalance.ToString();
    }

    /* =================== LEVELS =================== */

    //load a new scene based on name
    void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name);
    }

    //changes the level name
    public void ChangeNextLevel(string name)
    {
        nextLevel = name;
    }

    //loads the scene specified by nextLevel
    public void LoadNextLevel()
    {
        if (nextLevel.Length > 0)
        {
            LoadSceneByName(nextLevel);
        }
        else
        {
            Debug.Log("No next level scene specified");
        }
    }

    /* =================== PLAYER BALANCE =================== */

    // does the player have enough money to buy this item?
    public bool CanAfford(int cost)
    {
        if (cost > currentBalance) Debug.Log("ERROR: Not enough money to afford.");
        return cost <= currentBalance;
    }


    // deduct item cost from balance
    // technically setter for current balance
    public void PurchaseItem(int cost)
    {
        if (CanAfford(cost))
        {
            currentBalance -= cost;
            balanceText.text = currentBalance.ToString();
            Debug.Log("Successfully purchased!");
        }
    }

    // add item cost to balance
    // technically setter for current balance
    public void ReturnItem(int cost)
    {
        int newBalance = currentBalance + cost;
        if (newBalance > startingBalance)
        {
            Debug.Log("ERROR: Cannot return item.\nNew balance is larger than starting balance, " + newBalance + " > " + startingBalance);
            return;
        }

        currentBalance += cost;
        balanceText.text = currentBalance.ToString();
        Debug.Log("Successfully returned!");
    }

    // getter for the current balance
    public int GetCurrentBalance()
    {
        return currentBalance;
    }
}
