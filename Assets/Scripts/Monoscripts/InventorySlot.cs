using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField]
    private Image image;

    public void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
    }
    
}
