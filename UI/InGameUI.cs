using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public static InGameUI instance;
    public TextItem[] textItems;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Multiple instances of InGameUI!");
    }
    public void SetText(string key, string text)
    {
        foreach (var item in textItems)
        {
            if (item.key == key)
            {
                item.textObject.text = text;
                return;
            }
        }
        Debug.LogError("Key " + key + " not found!");
    }
}
[System.Serializable]
public struct TextItem
{
    public Text textObject;
    public string key;
}