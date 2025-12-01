using UnityEngine;
using System.Collections.Generic;

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
    List<Requirement> reqs = new List<Requirement>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //set manually in reqs, default 0
        for (int i = 0; i < req.Count; i++) {
            reqs.Add(new Requirement(req[i], itemAttributes[i]));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //when placed, update, add 1 to each relevant category
        //if all reqs >= wanted, win
    }

}
