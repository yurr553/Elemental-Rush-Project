using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    [Header("UI")]
    public Image iconImage;
    public Text countText;

    public string itemName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(Sprite sprite, string name, int count)
    {
        itemName = name;
        if (iconImage != null) iconImage.sprite = sprite;
        UpdateCount(count);
    }

    public void UpdateCount(int count)
    {
        if (countText != null)
        {
            countText.text = count.ToString();
        }
            
    }
}
