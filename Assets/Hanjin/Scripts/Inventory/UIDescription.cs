using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIDescription : MonoBehaviour
{
    public Image icon;
    public TMP_Text txtName;
    public TMP_Text txtDescription;

    private void OnDisable()
    {
        icon.sprite = null;
        txtName.text = "";
        txtDescription.text = "";
    }

    public void SetDescription(Sprite image, string name, string description)
    {
        icon.sprite = image;
        txtName.text = name;
        txtDescription.text = description;
    }
}
