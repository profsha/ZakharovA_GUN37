using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class Battlefield : MonoBehaviour
{
    [Inject]
    private BattleController _battleController;
    
    // Start is called before the first frame update
    void Start()
    {
        var cells = GetComponentsInChildren<Cell>().ToList()
            .OrderBy(x => x.transform.position.x)
            .ThenBy(x => x.transform.position.z);

        var units = GetComponents<Unit>().ToList();

        foreach (var cell in cells)
        {
            var unit = units.FirstOrDefault(x => Mathf.Approximately(x.transform.position.x, cell.transform.position.x)
                                                 && Mathf.Approximately(x.transform.position.z, cell.transform.position.z));
            cell.Unit = unit;
            cell.OnCellClicked += _battleController.HandleClick;
            if (unit != null)
            {
                unit.Cell = cell;
                unit.OnUnitClicked += _battleController.HandleClick;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
