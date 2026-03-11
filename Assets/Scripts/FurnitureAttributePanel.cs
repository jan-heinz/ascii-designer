using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FurnitureAttributePanel : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI attributeList;

    public void ShowAttributes(string name, string list)
    {
        gameObject.SetActive(true);

        itemName.text = name;
        attributeList.text = list;
    }

    public void HideAttributes()
    {
        gameObject.SetActive(false);
    }
}
