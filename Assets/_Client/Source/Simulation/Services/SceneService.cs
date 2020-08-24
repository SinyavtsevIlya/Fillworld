using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Zenject;
using UniRx;
using DG.Tweening;
using System;

public class SceneService : IInitializable
{
    const float FadeDuration = 0.8f;

    [Inject] Canvas _canvas;
    [Inject] GameObject _loadingPanel;
    [InjectOptional] string sortingLayer;

    CanvasGroup _fadeCanvasGroup;
    Tweener _fade;
    IDisposable _disposable;

    public void Initialize()
    {
        CreatePanel();

        _disposable = Observable.TimerFrame(1).Subscribe(_ => 
        {
            if (_fade != null) _fade.Kill();
            _fade = _fadeCanvasGroup.DOFade(0f, FadeDuration);
        });
    }

    public void LoadScene(string name, bool isFadeEnabled = true)
    {
        if (isFadeEnabled)
        {
            if (_fade != null) _fade.Kill();
            _fade = _fadeCanvasGroup.DOFade(1f, FadeDuration).OnComplete(() => SceneManager.LoadScene(name));
        }
        else
        {
            SceneManager.LoadScene(name);
        }
    }

    void CreatePanel()
    {
        var panel = GameObject.Instantiate<GameObject>(_loadingPanel, _canvas.transform, false);
        var rectTr = panel.GetComponent<RectTransform>();
        rectTr.anchorMin = new Vector2(0, 0);
        rectTr.anchorMax = new Vector2(1, 1);
        _fadeCanvasGroup = rectTr.GetComponent<CanvasGroup>();
        var canasOverride = panel.AddComponent<Canvas>();
        canasOverride.overrideSorting = true;
        if (sortingLayer != null) canasOverride.sortingLayerName = sortingLayer;
        canasOverride.sortingOrder = 300;
    }
}