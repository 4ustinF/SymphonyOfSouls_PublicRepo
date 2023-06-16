using UnityEngine;
using UnityEngine.UI;

public class CreditsUI : MonoBehaviour
{
    [SerializeField] private Button _backBtn = null;

    private void OnEnable()
    {
        _backBtn.onClick.AddListener(BackToLevelSelect);
    }

    private void OnDisable()
    {
        _backBtn.onClick.RemoveListener(BackToLevelSelect);
    }

    private void BackToLevelSelect()
    {
        ServiceLocator.Get<UIManager>().ShowMainMenu();
    }
}
