using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Restarter : MonoBehaviour
{
    [Inject]
    private Controls.GameActions _game;
    [Inject]
    SceneController _scene;

    [SerializeField]
    private int delay;
    
    private Coroutine _fillingCoroutine;
    
    [SerializeField]
    private Image fillingImage; 
    [SerializeField]
    private Image image;
    
    private TextMeshProUGUI _textMesh;

    void Start()
    {
        _textMesh = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_game.Restart.WasPressedThisFrame())
        {
            image.enabled = true;
            _textMesh.enabled = true;
            _fillingCoroutine = StartCoroutine(FillAmount());
        }

        if (_game.Restart.WasReleasedThisFrame())
        {
            StopCoroutine(_fillingCoroutine);
            fillingImage.fillAmount = 0;
            image.enabled = false;
            _textMesh.enabled = false;
        }
    }
    
    private IEnumerator FillAmount()
    {
        while (fillingImage.fillAmount < 1)
        {
            fillingImage.fillAmount += Time.deltaTime / delay;
            yield return null;
        }

        _scene.OpenGameScene();
    }
}
