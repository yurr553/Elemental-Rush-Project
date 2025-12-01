using UnityEngine;
using UnityEngine.UI;

public class PowerPanel : MonoBehaviour
{
    public Image thisImage;
    public Sprite thisSprite;
    public Text thisText;
    public string thisString;


    
    void Start()
    {
        Setup();
    }

    void Setup()
    {
        thisImage.sprite = thisSprite;
        thisText.text = thisString;

    }
}
