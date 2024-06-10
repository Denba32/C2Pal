using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    List<PopupUI> popupUI = new List<PopupUI>();
    SceneUI sceneUI = null;

    private int currentOrder;

    private UIInventory inventory;
    public UIInventory Inventory
    {
        get
        {
            if(inventory == null)
            {
                inventory = ShowPopupUI<UIInventory>();
                inventory.gameObject.SetActive(false);
                Util.ActiveCursor(false);
            }
            return inventory;
        }
    }

    private UICraft craft;
    
    public UICraft Craft
    {
        get
        {
            if (craft == null)
            {
                craft = ShowPopupUI<UICraft>();
                craft.gameObject.SetActive(false);
                craft.Init();
                Util.ActiveCursor(false);
                
            }
            return craft;
        }
    }

    private MainSceneUI mainUI;

    public MainSceneUI MainUI
    {
        get
        {
            if (mainUI == null)
            {
                mainUI = ShowSceneUI<MainSceneUI>();
                mainUI.gameObject.SetActive(true);
                Util.ActiveCursor(false);
            }
            return mainUI;
        }
    }

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
        
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        
        if (sort)
        {
            currentOrder++;
            canvas.sortingOrder = currentOrder;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public void ShowInventory()
    {
        if(inventory == null)
        {
            inventory = ShowPopupUI<UIInventory>();
            inventory.gameObject.SetActive(true);
            GameManager.Instance.GamePause();
        }
        else
        {
            if (inventory.gameObject.activeInHierarchy)
            {
                inventory.gameObject.SetActive(false);
                popupUI.RemoveAt(popupUI.IndexOf(inventory));
                currentOrder--;

                if (popupUI.Count <= 0)
                {
                    Util.ActiveCursor(false);
                    GameManager.Instance.GameStart();
                }
            }
            else
            {
                GameManager.Instance.GamePause();
                inventory.gameObject.SetActive(true);
                if(!popupUI.Contains(inventory))
                    popupUI.Add(inventory);
                Util.ActiveCursor(true);

            }
        }

    }

    public void ShowCraft()
    {
        if (craft == null)
        {
            craft = ShowPopupUI<UICraft>();
            craft.gameObject.SetActive(true);
            GameManager.Instance.GamePause();
        }
        else
        {
            if (craft.gameObject.activeInHierarchy)
            {
                craft.gameObject.SetActive(false);
                popupUI.RemoveAt(popupUI.IndexOf(craft));
                currentOrder--;

                if (popupUI.Count <= 0)
                {
                    Util.ActiveCursor(false);
                    GameManager.Instance.GameStart();
                }
            }
            else
            {
                GameManager.Instance.GamePause();
                craft.gameObject.SetActive(true);
                if (!popupUI.Contains(craft))
                    popupUI.Add(craft);
                Util.ActiveCursor(true);

            }
        }

    }

    public void ShowMainSceneUI()
    {
        if(mainUI == null)
        {
            mainUI = ShowSceneUI<MainSceneUI>();
            mainUI.gameObject.SetActive(true);
            sceneUI = mainUI;
        }
        else
        {
            if (mainUI.gameObject.activeInHierarchy)
            {
                mainUI.gameObject.SetActive(false);
            }
            else
            {
                mainUI.gameObject.SetActive(true);
            }
        }
    }


    public T ShowPopupUI<T>(string name = null) where T : PopupUI
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = ResourceManager.Instance.Instantiate($"UI/Popup/{name}");

        T popup = Util.GetOrAddComponent<T>(go);

        popup.Init();
        popup.sortOrder = currentOrder;

        popupUI.Add(popup);
        popupUI.Sort((x, y) => x.sortOrder.CompareTo(y.sortOrder));
        go.transform.SetParent(Root.transform);

        Util.ActiveCursor(true);

        return popup;
    }

    
    public void ClosePopupUI(PopupUI popup)
    {
        if (popupUI.Count <= 0)
        {
            return;
        }

        popupUI.Remove(popup);

        if(popup != inventory)
            ResourceManager.Instance.Destroy(popup.gameObject);

        currentOrder--;


        popupUI.Sort((x, y) => x.sortOrder.CompareTo(y.sortOrder));

        if (popupUI.Count <= 0)
        {
            Util.ActiveCursor(false);
        }
    }



    public T ShowSceneUI<T>(string name = null) where T : SceneUI
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = ResourceManager.Instance.Instantiate($"UI/Scene/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        this.sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);

        return sceneUI;
    }

    public void Clear()
    {
        popupUI.Clear();
        sceneUI = null;
    }
}
