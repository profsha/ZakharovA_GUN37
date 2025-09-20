using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

public class Restarter : MonoBehaviour
{
    [Inject]
    private Controls.GameActions _game;
    [Inject]
    private SceneController _scene;

    [SerializeField]
    private int delay;
    
    private Coroutine _fillingCoroutine;
    
    private Image _fillingImage; 

    private Image _image;
    
    private TextMeshProUGUI _textMesh;

    void Start()
    {
        _image = GetComponent<Image>();
        _fillingImage = GetComponentsInChildren<Image>().FirstOrDefault(x => x.gameObject != gameObject);
        _textMesh = GetComponentInChildren<TextMeshProUGUI>();
        _game.Restart.performed += OnRestartPerformed;
        _game.Restart.canceled += OnCancelPerformed;
    }

    private void OnDestroy()
    {
        _game.Restart.performed -= OnRestartPerformed;
        _game.Restart.canceled -= OnCancelPerformed;
    }

    private void OnRestartPerformed(InputAction.CallbackContext obj)
    {
        _image.enabled = true;
        _textMesh.enabled = true;
        _fillingImage.fillAmount = 0;
        _fillingCoroutine = StartCoroutine(FillAmount());
    }

    private void OnCancelPerformed(InputAction.CallbackContext obj)
    {
        StopCoroutine(_fillingCoroutine);
        _fillingImage.fillAmount = 0;
        _image.enabled = false;
        _textMesh.enabled = false;
    }
    
    private IEnumerator FillAmount()
    {
        while (_fillingImage.fillAmount < 1)
        {
            _fillingImage.fillAmount += Time.deltaTime / delay;
            yield return null;
        }

        _scene.OpenGameScene();
    }
}
