using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderBoardUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _backBtn = null;
    [SerializeField] private Button _level1Btn  = null;
    [SerializeField] private Button _level2Btn  = null;

    [SerializeField] private TMP_Text _level1BtnText = null;
    [SerializeField] private TMP_Text _level2BtnText = null;

    [SerializeField] private TMP_Text _score1Text = null;
    [SerializeField] private TMP_Text _score2Text = null;
    [SerializeField] private TMP_Text _score3Text = null;

    [SerializeField] private TMP_Text _time1Text = null;
    [SerializeField] private TMP_Text _time2Text = null;
    [SerializeField] private TMP_Text _time3Text = null;

    [SerializeField] private DataAsset _dataAsset = null;

    private void OnEnable()
    {
        _backBtn.onClick.AddListener(BackToLevelSelect);
        _level1Btn.onClick.AddListener(UpdateLevel1Text);
        _level2Btn.onClick.AddListener(UpdateLevel2Text);
        UpdateLevel1Text();
    }

    private void OnDisable()
    {
        _backBtn.onClick.RemoveListener(BackToLevelSelect);
        _level1Btn.onClick.RemoveListener(UpdateLevel1Text);
        _level2Btn.onClick.RemoveListener(UpdateLevel2Text);
    }

    private void BackToLevelSelect()
    {
        ServiceLocator.Get<UIManager>().ShowLevelSelect();
    }

    private void UpdateLevel1Text()
    {
        _score1Text.text = _dataAsset.GameData._level1TopDetails[0].PointsCollected.ToString();
        _score2Text.text = _dataAsset.GameData._level1TopDetails[1].PointsCollected.ToString();
        _score3Text.text = _dataAsset.GameData._level1TopDetails[2].PointsCollected.ToString();

        _time1Text.text = $"{ System.Math.Round(_dataAsset.GameData._level1TopDetails[0].TimeSpent, 2)}";
        _time2Text.text = $"{ System.Math.Round(_dataAsset.GameData._level1TopDetails[1].TimeSpent, 2)}";
        _time3Text.text = $"{ System.Math.Round(_dataAsset.GameData._level1TopDetails[2].TimeSpent, 2)}";

        _level1BtnText.fontStyle = FontStyles.Underline;
        _level2BtnText.fontStyle = FontStyles.Normal;
    }

    private void UpdateLevel2Text()
    {
        _score1Text.text = _dataAsset.GameData._level2TopDetails[0].PointsCollected.ToString();
        _score2Text.text = _dataAsset.GameData._level2TopDetails[1].PointsCollected.ToString();
        _score3Text.text = _dataAsset.GameData._level2TopDetails[2].PointsCollected.ToString();

        _time1Text.text = $"{ System.Math.Round(_dataAsset.GameData._level2TopDetails[0].TimeSpent, 2)}";
        _time2Text.text = $"{ System.Math.Round(_dataAsset.GameData._level2TopDetails[1].TimeSpent, 2)}";
        _time3Text.text = $"{ System.Math.Round(_dataAsset.GameData._level2TopDetails[2].TimeSpent, 2)}";

        _level1BtnText.fontStyle = FontStyles.Normal;
        _level2BtnText.fontStyle = FontStyles.Underline;
    }

}
