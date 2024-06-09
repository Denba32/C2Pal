using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform buttonRect;

    public UIDescription description;

    public ItemData item;

    public Button button;

    public Image icon;

    public TMP_Text quantityText;

    public Outline outline;

    public UIInventory inventory;

    public int index;
    public bool equipped;
    public int quantity;


    private void OnEnable()
    {
        outline.enabled = equipped;
        button.onClick.AddListener(OnClickButton);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClickButton);
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
        outline.enabled = false;
    }

    public bool HasItem() => item != null;


    public void OnClickButton()
    {
        ActiveDescription(false);

        ActiveSelectedPanel();
        inventory.SelectItem(index);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item != null && !inventory.selectedPanel.gameObject.activeInHierarchy)
        {
            ActiveDescription(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ActiveDescription(false);
    }


    private void ActiveSelectedPanel()
    {
        Vector3 toRightBottom = new Vector3(buttonRect.rect.width / 2, -buttonRect.rect.height / 2, 0);

        Vector3 worldPoint = buttonRect.TransformPoint(toRightBottom);

        inventory.selectedContent.transform.position = worldPoint;

    }
    private void ActiveDescription(bool active)
    {
        if(active)
        {
            Vector3 toRightBottom = new Vector3(buttonRect.rect.width / 2, -buttonRect.rect.height / 2, 0);

            Vector3 worldPoint = buttonRect.TransformPoint(toRightBottom);

            description.transform.position = worldPoint;

            description.gameObject.SetActive(true);

            description.SetDescription(item.icon, item.displayName, item.description);
        }
        else
        {
            description.gameObject.SetActive(false);
        }
    }
}