using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController controller;
    public PlayerCondition condition;
    public PlayerInventory inventory = new PlayerInventory();

    public Transform dropPosition;

    public Equipment primaryWeapon;
    public Equipment secondaryWeapon;

    public Action<ItemData> onAddItem;

    public ItemData ITData;

    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
    }

    private void OnEnable()
    {
        onAddItem += inventory.AddItem;
    }

    private void OnDisable()
    {
        onAddItem -= inventory.AddItem;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            inventory.CheckItems();
        }
    }

}