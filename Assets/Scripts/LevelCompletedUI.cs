using UnityEngine;
using TMPro;

public class LevelCompletedUI : MonoBehaviour
{
    public TextMeshProUGUI factorsScoresText;
    public TextMeshProUGUI totalScoreText;

    ReputationSystem rs;

    void Awake()
    {
        rs = FindFirstObjectByType<ReputationSystem>();
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
