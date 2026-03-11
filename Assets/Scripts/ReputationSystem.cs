using UnityEngine;
using System.Collections.Generic;

public class ReputationSystem : MonoBehaviour
{
    public int clientRequirementMax = 500;
    public int cohesionBonusMax = 250;
    public int budgetEfficiencyMax = 250;

    int clientRequirements = 0;
    int cohesionBonus = 0;
    int budgetEfficiency = 0;
    int totalReputation = 0;
    
    RequirementSystem rs;
    LevelManager lm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rs = FindFirstObjectByType<RequirementSystem>();
        lm = FindFirstObjectByType<LevelManager>();
    }

    public int CalculateTotalReputation()
    {
        clientRequirements = CalculateClientRequirementsScore();
        cohesionBonus = CalculateCohesionBonus();
        budgetEfficiency = CalculateBudgetEfficiency();

        return clientRequirements + cohesionBonus + budgetEfficiency;
    }

    // total fulfilled / total reqs * max
    int CalculateClientRequirementsScore()
    {
        int totalReqsMet = rs.TotalRequirementsMet();
        int totalReqs = rs.req.Count;

        return totalReqsMet * clientRequirementMax / totalReqs;
    }

    // attr with most freq / total num of attrs * max
    int CalculateCohesionBonus()
    {
        Dictionary<FurnitureAttribute, int> attributeCounts = rs.GetAttributeCounts();

        int maxCount = 0;
        int totalAttrs = 0;

        foreach (var pair in attributeCounts)
        {
            totalAttrs += pair.Value;
            if (pair.Value > maxCount)
            {
                maxCount = pair.Value;
            }
        }

        if (totalAttrs == 0) return 0;
        return maxCount * cohesionBonusMax / totalAttrs;
    }

    // budget left / total budget * max
    int CalculateBudgetEfficiency()
    {
        int budget = lm.GetStartingBalance();
        int currentBudget = lm.GetCurrentBalance();

        if (currentBudget <= 0) return 0;
        return currentBudget * budgetEfficiencyMax / budget;
    }

    // getters
    public int GetClientRequirementsScore() {
        clientRequirements = CalculateClientRequirementsScore();
        return clientRequirements;
    }

    public int GetCohesionBonus() {
        cohesionBonus = CalculateCohesionBonus();
        return cohesionBonus;
    }

    public int GetBudgetEfficiency() {
        budgetEfficiency =  CalculateBudgetEfficiency();
        return budgetEfficiency;
    }
}
