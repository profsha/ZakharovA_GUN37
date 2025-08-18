using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Loader : MonoBehaviour
{
    [Inject]
    private SceneController _sceneController;
    
    public void LoadMainScene() => _sceneController.OpenMainScene();
    
    public void LoadGameScene() => _sceneController.OpenGameScene();
}
