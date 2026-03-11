using UnityEngine;
using System.Collections.Generic;
using TMPro;

public struct Requirement {
    public int have;
    public int goal;
    public FurnitureAttribute attribute;

    public Requirement(int goal, FurnitureAttribute attribute) {
        have = 0;
        this.goal = goal;
        this.attribute = attribute;
    }
}

public class RequirementSystem : MonoBehaviour
{
    //list of vars corresponding to each scriptable object attribute
    public List<FurnitureAttribute> itemAttributes;
    public List<int> req;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI infoText;
    List<Requirement> reqs = new List<Requirement>();
    public AudioClip successSFX;

    Dictionary<FurnitureAttribute, int> attributeCounts = new Dictionary<FurnitureAttribute, int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //set manually in reqs, default 0
        for (int i = 0; i < req.Count; i++) {
            reqs.Add(new Requirement(req[i], itemAttributes[i]));
        }

        TextUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        //when placed, update, add 1 to each relevant category
        //if all reqs >= wanted, win
    }

    public void CheckItem(FurnitureItem item) {
        bool allMet = true;
        for (int i = 0; i < reqs.Count; i++) {
            foreach (FurnitureAttribute att in item.itemAttributes) {
                if (reqs[i].attribute == att)
                {
                    Requirement r = reqs[i];
                    r.have++;
                    reqs[i] = r;
                }
                
                if (reqs[i].have < reqs[i].goal) {
                    allMet = false;
                }

                // add each attribute to the map
                if (attributeCounts.ContainsKey(att)) {
                    attributeCounts[att]++;
                } else {
                    attributeCounts[att] = 1;
                }
            }
        }

        TextUpdate();

        // TODO: MOVE TO LEVEL MANAGER LATER
        //check for win condition
        if (allMet)
        {
           //AudioManager.Instance.PlaySFX(successSFX);
            Debug.Log("All requirements met! Level Complete!");
        }
    }

    void TextUpdate()
    {
        int totalHave = 0;
        infoText.text = "";
        for (int i = 0; i < reqs.Count; i++)
        {
            infoText.text += reqs[i].have + "/" + reqs[i].goal + " " + reqs[i].attribute.attribute + " items.\n";

            if (reqs[i].have >= reqs[i].goal)
            {
                totalHave++;
            }
        }

        countText.text = "";
        countText.text = "Client " + totalHave + "/" + reqs.Count;
    }

    public void RemoveAttributes(FurnitureItem item) {
        for (int i = 0; i < reqs.Count; i++) {
            foreach (FurnitureAttribute att in item.itemAttributes) {
                if (reqs[i].attribute == att) {
                    Requirement r = reqs[i];
                    if (r.have > 0) r.have--;
                    reqs[i] = r;
                }

                // remove each attribute from the map
                if (attributeCounts.ContainsKey(att)) {
                    attributeCounts[att]--;

                    if (attributeCounts[att] <= 0) {
                        attributeCounts.Remove(att);
                    }
                }
            }
        }

        TextUpdate();
    }

    public int TotalRequirementsMet()
    {
        int totalMet = 0;
        for (int i = 0; i < reqs.Count; i++)
        {
            if (reqs[i].have >= reqs[i].goal)
            {
                totalMet++;
            }
        }

        return totalMet;
    }

    public Dictionary<FurnitureAttribute, int> GetAttributeCounts()
    {
        return attributeCounts;
    }
}
