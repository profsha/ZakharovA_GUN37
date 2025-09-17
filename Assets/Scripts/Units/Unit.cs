using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private Team team = Team.White;

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

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, Time.deltaTime * speed);
            yield return null;
        }

        transform.position = targetPosition;
        OnMovementCompleted?.Invoke(this);
        Cell = cell;
    }

    public void OnPointerClick(PointerEventData eventData) => Cell.OnPointerClick(eventData);

    public void OnPointerEnter(PointerEventData eventData) => Cell.OnPointerEnter(eventData);

    public void OnPointerExit(PointerEventData eventData) => Cell.OnPointerExit(eventData);
}
