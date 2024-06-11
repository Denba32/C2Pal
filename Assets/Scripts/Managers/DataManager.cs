using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.Networking;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}
public class DataManager
{
    #region 데이터 저장소
    private Dictionary<int, CraftData> craftDict;
    #endregion

    public DataManager()
    {
        Init();
    }

    public void Init()
    {
        craftDict = new Dictionary<int, CraftData>();

        var craft = Resources.LoadAll<CraftData>("Data/Craft");

        for(int i = 0; i < craft.Length; i++)
        {
            craftDict.Add(craft[i].id, craft[i]);
        }
        
        return; 
    }

    public void Clear()
    {
        craftDict.Clear();
    }

    public Dictionary<int, CraftData> GetCraftData()
    {
        if (craftDict != null)
        {
            return craftDict;
        }
        else
        {
            Init();
            return craftDict;
        }
    }


    Loader LoadJson<Loader, Key, Value>(string path, bool isBinary = false) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }

    public string SaveJson(Object data)
    {
        if(data != null)
            return JsonUtility.ToJson(data);

        return "";
    }

    public void SaveData(Object data)
    {
        string path = $"{Application.persistentDataPath}/{data.name}.bin";
        if(File.Exists(path))
        {

        }
        FileStream fs = new FileStream(path, FileMode.Create);
    }

    public void LoadData()
    {

    }
}
