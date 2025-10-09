using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ToggleMap : MonoBehaviour
{
    public GameObject[] maps;
    
    private int _currentMap = 0;

    void SetActiveMap()
    {
        System.Array.ForEach(maps, x => x.SetActive(false));
        maps[_currentMap].SetActive(true);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        SetActiveMap();
    }

    public void HandleClick()
    {
        _currentMap = (_currentMap + 1) % maps.Length;
        SetActiveMap();
    }
}
