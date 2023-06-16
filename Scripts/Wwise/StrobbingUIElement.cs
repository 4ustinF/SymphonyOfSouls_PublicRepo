using UnityEngine;
using UnityEngine.UI;

public class StrobbingUIElement : MonoBehaviour
{
    private Image _image = null;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void ChangeColor()
    {
        _image.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0f, 1f);
    }
}