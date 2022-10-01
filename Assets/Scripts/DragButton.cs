using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class DragButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector]
    private Image buttonImage;
    public PetTabCell petTabCell;
    public Image edgeImage;
    public Image itemImage;
    public bool rosterSlot;
    public bool draggable;
    public UnityEvent unityEvent;
    public UnityEvent rightEvent;
    public MergeController mergeController;
    public static PetTabCell hoveringItem;
    private bool startedDragging;
    private Vector3 dragStartPos;
    private void Awake()
    {
        buttonImage = GetComponent<Image>();
    }
    private void Update()
    {
        if (!draggable || petTabCell.petData.creatureSelected)
        {
            return;
        }
        if (startedDragging && Vector3.Distance(Input.mousePosition, dragStartPos) > .1f)
        {
            mergeController.BeginDrag(petTabCell.petData);
            startedDragging = false;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        bool disabledCell = !rosterSlot && petTabCell.cellState != StateType.InPool;
        if (draggable && !petTabCell.petData.creatureSelected)
        {
            startedDragging = false;
            mergeController.EndDrag();
        }
        buttonImage.material = BaseUtils.highlightUI;
        itemImage.material = disabledCell ? BaseUtils.GetGrayMaterial(petTabCell.cellState, true) : BaseUtils.highlightUI;
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            rightEvent.Invoke();
        }
        else
        {
            unityEvent.Invoke();
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        bool disabledCell = !rosterSlot && petTabCell.cellState != StateType.InPool;
        buttonImage.material = BaseUtils.normalUIMat;
        itemImage.material = disabledCell ? BaseUtils.GetGrayMaterial(petTabCell.cellState, false) : BaseUtils.normalUIMat;
        if (!draggable || petTabCell.petData.creatureSelected)
        {
            return;
        }
        startedDragging = true;
        dragStartPos = Input.mousePosition;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        hoveringItem = petTabCell;
        bool disabledCell = !rosterSlot && petTabCell.cellState != StateType.InPool;
        if (mergeController != null && mergeController.showingPreview && edgeImage != null)
        {
            edgeImage.material = BaseUtils.highLineUI;
        }
        buttonImage.material = BaseUtils.highlightUI;
        itemImage.material = disabledCell ? BaseUtils.GetGrayMaterial(petTabCell.cellState, true) : BaseUtils.highlightUI;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        bool disabledCell = !rosterSlot && petTabCell.cellState != StateType.InPool;
        if (edgeImage != null)
        {
            edgeImage.material = BaseUtils.normalUIMat;
        }
        buttonImage.material = BaseUtils.normalUIMat;
        itemImage.material = disabledCell ? BaseUtils.GetGrayMaterial(petTabCell.cellState, false) : BaseUtils.normalUIMat;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!draggable || petTabCell.petData.creatureSelected)
        {
            return;
        }
        if (hoveringItem != null && hoveringItem.rosterSlot != -1)
        {
            mergeController.OnMergeDrop(petTabCell, hoveringItem);
        }
        hoveringItem = null;
        mergeController.EndDrag();
    }
    private void OnDisable()
    {
        bool disabledCell = !rosterSlot && petTabCell.cellState != StateType.InPool;
        buttonImage.material = BaseUtils.normalUIMat;
        itemImage.material = disabledCell ? BaseUtils.GetGrayMaterial(petTabCell.cellState, false) : BaseUtils.normalUIMat;
        if (edgeImage != null)
        {
            edgeImage.material = BaseUtils.normalUIMat;
        }
    }
}
