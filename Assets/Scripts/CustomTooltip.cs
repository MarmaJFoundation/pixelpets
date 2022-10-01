using UnityEngine;
using UnityEngine.EventSystems;

public class CustomTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MainMenuController mainMenuController;
    private RectTransform rectTransform;
    [HideInInspector]
    public ElementType elementType;
    [HideInInspector]
    public int elementIndex;
    public string[] tooltipText;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        mainMenuController = FindObjectOfType<MainMenuController>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (elementIndex != 0)
        {
            tooltipText[0] = elementIndex == 1 ? $"body type:{elementType}" : $"attack type:{elementType}";
            tooltipText[1] = $"strong against:{BaseUtils.GetStrongFromType(elementType, elementIndex != 1)}";
            tooltipText[2] = $"weak against:{BaseUtils.GetWeakFromType(elementType, elementIndex != 1)}";
        }
        mainMenuController.ShowTooltip(rectTransform, tooltipText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mainMenuController.HideTooltip();
    }
}
