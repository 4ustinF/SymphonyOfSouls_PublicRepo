using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlButtonTemplate : MonoBehaviour
{
    [SerializeField] private Button _button = null;
    [SerializeField] private TMP_Text _text = null;
    [SerializeField] private Image _warningImage = null;

    public Button Button => _button;
    public TMP_Text TextElement => _text;

    public void UpdateWarningStatus(bool status, bool isCurrentlySelected = false)
    {
        _warningImage.color = isCurrentlySelected ? Color.white : Color.red;
        _warningImage.gameObject.SetActive(status);
    }

    private void Awake()
    {
        _warningImage.gameObject.SetActive(false);
    }
}
