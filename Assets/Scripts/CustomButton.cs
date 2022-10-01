using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [HideInInspector]
    private Image buttonImage;
    public UnityEvent unityEvent;
    public UnityEvent rightEvent;
    [HideInInspector]
    public bool deactivated;
    private void Awake()
    {
        buttonImage = GetComponent<Image>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(BounceTransform(eventData.button == PointerEventData.InputButton.Right));
        buttonImage.material = OutlineMaterial() ? BaseUtils.highLineUI : BaseUtils.highlightUI;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        buttonImage.material = OutlineMaterial() ? GetOutlineMaterial() : GetNormalMaterial();
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.material = OutlineMaterial() ? BaseUtils.highLineUI : GetHightLightMaterial();
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.material = OutlineMaterial() ? GetOutlineMaterial() : GetNormalMaterial();
    }
    private Material GetHightLineMaterial()
    {
        return deactivated ? BaseUtils.grayHighlineUIMat : BaseUtils.highLineUI;
    }
    private Material GetHightLightMaterial()
    {
        return deactivated ? BaseUtils.graylightUIMat[0] : BaseUtils.highlightUI;
    }
    private Material GetOutlineMaterial()
    {
        return deactivated ? BaseUtils.grayLineUIMat : BaseUtils.outlineUIMat;
    }
    private Material GetNormalMaterial()
    {
        return deactivated ? BaseUtils.grayscaleUIMat[0] : BaseUtils.normalUIMat;
    }
    private bool OutlineMaterial()
    {
        return buttonImage.material.GetFloat("_UseOutline") == 1;
    }
    private void OnDisable()
    {
        buttonImage.material = BaseUtils.normalUIMat;
    }
    private IEnumerator BounceTransform(bool rightClickEvent = false)
    {
        float timer = 0;
        bool calledEvent = false;
        while (timer <= 1)
        {
            transform.localScale = Vector3.LerpUnclamped(Vector3.one, Vector3.one * .92f, timer.Evaluate(CurveType.ButtonPressCurve));
            timer += Time.deltaTime * 7.5f;
            if (timer > .5f && !calledEvent)
            {
                calledEvent = true;
                if (rightClickEvent)
                {
                    if (rightEvent != null)
                    {
                        rightEvent.Invoke();
                    }
                }
                else
                {
                    if (unityEvent != null)
                    {
                        unityEvent.Invoke();
                    }
                }
            }
            yield return null;
        }
        transform.localScale = Vector3.one;
    }
}
