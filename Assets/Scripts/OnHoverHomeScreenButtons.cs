using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class OnHoverHomeScreenButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string buttonText;
    public AudioClip? onClickSFX;
    TextMeshProUGUI textComponent;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // add brackets for player feedback
        textComponent.text = "[" + buttonText + "]";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // remove brackets, revert text to normal
        textComponent.text = buttonText;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // play click sound
        if (onClickSFX) AudioManager.Instance.PlaySFX(onClickSFX);
    }

}