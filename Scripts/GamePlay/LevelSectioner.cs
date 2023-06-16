using UnityEngine;

public class LevelSectioner : MonoBehaviour
{
    [SerializeField] private GameObject _firstSection = null;
    [SerializeField] private GameObject _secondSection = null;

    public void EnterNewSection()
    {
        _secondSection.SetActive(true);
        _firstSection.SetActive(false);
    }
}
