using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoaderManager : MonoBehaviour
{
    [SerializeField] private Image _fadeImage = null;
    [SerializeField] private float _fadeOutTime = 0.1f;
    //[SerializeField] private float _fadeInTime = 0.1f;

    private StatefulObject _uiStatefulObject = null;

    public SceneLoaderManager Initialize(StatefulObject UiStatefulObject)
    {
        _uiStatefulObject = UiStatefulObject;
        _fadeImage.color = Color.clear;
        return this;
    }

    public void LoadScene(string sceneName, string UiState = "")
    {
        StartCoroutine(FadeOutRoutine(sceneName, UiState));
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        float time = 0.0f;

        while (time < _fadeOutTime)
        {
            time += Time.deltaTime;
            _fadeImage.color = new Color(0.0f, 0.0f, 0.0f, 1.0f - time / _fadeOutTime);
            yield return null;
        }

        _fadeImage.color = Color.clear;
    }

    private IEnumerator FadeOutRoutine(string sceneName, string UiState)
    {
        float time = 0.0f;

        while(time < _fadeOutTime)
        {
            time += Time.deltaTime;
            _fadeImage.color = new Color(0.0f, 0.0f, 0.0f, time / _fadeOutTime);
            yield return null;
        }

        _fadeImage.color = Color.black;
        SceneManager.LoadScene(sceneName);
        if (string.IsNullOrEmpty(UiState) == false)
        {
            _uiStatefulObject.PublicSetState(UiState);
        }
        FadeIn();
    }
}
