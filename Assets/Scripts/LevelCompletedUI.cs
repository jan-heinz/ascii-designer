using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelCompletedUI : MonoBehaviour
{
    public TextMeshProUGUI factorsScoresText;
    public TextMeshProUGUI totalScoreText;
    public Slider totalScoreSlider;

    [Header("Client Comments")]
    public TextMeshProUGUI clientCommentText;
    public string[] threeStarComments;
    public string[] twoStarComments;
    public string[] oneStarComments;

    private int totalStars = 0;

    ReputationSystem rs;

    void Awake()
    {
        rs = FindFirstObjectByType<ReputationSystem>();
    }

    public void SetGameOverUI()
    {
        SetSlider();
        SetClientComment();
    }

    public void SetClientComment()
    {
        if (totalStars == 3)
        {
            clientCommentText.text = threeStarComments[Random.Range(0, threeStarComments.Length)];
        }
        else if (totalStars == 2)
        {
            clientCommentText.text = twoStarComments[Random.Range(0, twoStarComments.Length)];
        }
        else
        {
            clientCommentText.text = oneStarComments[Random.Range(0, oneStarComments.Length)];
        }
    }

    public void SetSlider()
    {
        // 700 - 1000 => 3 stars
        // 400 - 699 => 2 stars
        // 0 - 399 => 1 star

        int totalScore = rs.CalculateTotalReputation();
        if (totalScore >= 700 && totalScore <= 1000)
        {
            totalStars = 3;
            totalScoreSlider.value = 3;
        }
        else if (totalScore >= 400 && totalScore < 700)
        {
            totalStars = 2;
            totalScoreSlider.value = 2;
        }
        else
        {
            totalStars = 1;
            totalScoreSlider.value = 1;
        }
    }

    public void ShowScores()
    {
        int clientReqScore = rs.GetClientRequirementsScore();
        int cohesionBonus = rs.GetCohesionBonus();
        int budgetEfficiency = rs.GetBudgetEfficiency();

        factorsScoresText.text = clientReqScore
                                    + "\n" + budgetEfficiency
                                    + "\n" + cohesionBonus;
        totalScoreText.text = rs.CalculateTotalReputation().ToString();
    }
}
