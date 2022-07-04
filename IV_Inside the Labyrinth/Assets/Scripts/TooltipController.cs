using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text_TMP;

    private static TooltipController instance;
    private RectTransform rectTransform;
    private float textOffset = 4f;
    private float maxtWidth = 400f;

    private void Awake()
    {
        instance = this;
        rectTransform = GetComponent<RectTransform>();
        instance.gameObject.SetActive(false);
    }

    public static void ShowTooltip(string tooltipText, Vector3 position)
    {
        instance.ShowTooltip_private(tooltipText, position);
    }

    public static void HideTooltip()
    {
        instance.gameObject.SetActive(false);
    }

    private void ShowTooltip_private(string tooltipText, Vector3 position)
    {
        if(tooltipText.Length == 0)
        {
            return;
        }
        text_TMP.text = tooltipText;
        Vector2 size = new Vector2(Mathf.Clamp(text_TMP.preferredWidth, 10, maxtWidth) + 2 * textOffset, 
            text_TMP.preferredHeight + 2 * textOffset);
        //text_TMP.GetComponent<RectTransform>().sizeDelta = size;
        rectTransform.sizeDelta = size;
        rectTransform.position = position;
        gameObject.SetActive(true);
    }
}
