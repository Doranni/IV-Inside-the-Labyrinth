using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(1, 10)]
    [SerializeField] private string text;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipController.ShowTooltip(text, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipController.HideTooltip();
    }
}
