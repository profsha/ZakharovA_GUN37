using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float speed = 2.0f;
    public Team Team = Team.White;
    
    public UnitType Type = UnitType.Common;

    public Cell Cell { get; set; }

    public event Action<Unit> OnMovementStarted;
    public event Action<Unit> OnMovementCompleted;
    
    private Coroutine _movementCoroutine;

    public void MoveTo(Cell cell)
    {
        StartCoroutine(MoveToPositionCoroutine(cell));
    }
    
    private IEnumerator MoveToPositionCoroutine(Cell cell)
    {
        OnMovementStarted?.Invoke(this);

        Vector3 targetPosition = transform.position;
        targetPosition.x = cell.transform.position.x;
        targetPosition.z = cell.transform.position.z;
        
        Vector3 startPosition = transform.position;
        
        var time = Vector3.Distance(startPosition, targetPosition) / speed;

        var delta = 0f;

        while (delta < time)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, delta / time);
            delta += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        Cell = cell;
        cell.Unit = this;
        OnMovementCompleted?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData) => Cell.OnPointerClick(eventData);

    public void OnPointerEnter(PointerEventData eventData) => Cell.OnPointerEnter(eventData);

    public void OnPointerExit(PointerEventData eventData) => Cell.OnPointerExit(eventData);
}
