using TMPro;
using UnityEngine;

public class TutorialAreaTrigger : MonoBehaviour
{
    [SerializeField] private TMP_Text _tipTextHolder = null;
    private string _tipText = string.Empty;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _tipTextHolder.text = _tipText;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _tipTextHolder.text = string.Empty;
        }
    }

    public void SetTipText(string newTipText)
    {
        if (string.CompareOrdinal(_tipTextHolder.text, _tipText) == 0)
        {
            _tipTextHolder.text = newTipText;
        }

        _tipText = newTipText;
    }
}
