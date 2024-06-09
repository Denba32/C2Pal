

using UnityEngine;

[CreateAssetMenu(fileName = "CraftData", menuName = "CraftData/CraftData", order = 0)]
public class CraftData : ScriptableObject
{
    public int id;
    public string receipeName;

    public ItemData craftTarget;
    public int craftQuantity;

    public CraftIngradient[] needs;

    private void OnValidate()
    {
#if UNITY_EDITOR
        receipeName = craftTarget.displayName;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}

[System.Serializable]
public class CraftIngradient
{
    public ItemData needItem;
    public int neadedQuantity;
}
