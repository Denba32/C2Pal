using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public ItemData item;

    public Button button;
    public Image icon;
    public TMP_Text quantityText;
    private Outline outline;

    public UIInventory inventory;

    public int index;
    public bool equipped;
    public int quantity;

    private Vector2 lastPos;


    private void Awake()
    {
        outline = GetComponent<Outline>();
    }
    private void OnEnable()
    {
        outline.enabled = equipped;
    }

    public void Set()
    {
        icon.gameObject.SetActive(true);
        icon.sprite = item.icon;
        quantityText.text = quantity > 1 ? quantity.ToString() : string.Empty;

        if(outline != null)
        {
            outline.enabled = equipped;
        }
    }

    public void Clear()
    {
        item = null;
        icon.sprite = null;
        icon.gameObject.SetActive(false);
        quantityText.text = string.Empty;
    }

    public bool HasItem() => item != null;


    public void OnClickButton()
    {
        // inventory.SelectItem(index);
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    { 

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData != null)
            icon.transform.position = eventData.position;
    }


    public void OnEndDrag(PointerEventData eventData)
    {

    }
}
