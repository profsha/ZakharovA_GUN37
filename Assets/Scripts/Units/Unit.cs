using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 _targetPoint;
    [SerializeField]
    private float speed = 2.0f;
    public Cell Cell { get; set; }

    public void MoveTo(Cell cell)
    {
        _targetPoint.x = cell.transform.position.x;
        _targetPoint.z = cell.transform.position.z;
        _targetPoint.y = transform.position.y;
        Cell = cell;
    }
    
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _targetPoint, speed * Time.deltaTime);
    }

    public void OnPointerClick(PointerEventData eventData) => Cell.OnPointerClick(eventData);

    public void OnPointerEnter(PointerEventData eventData) => Cell.OnPointerEnter(eventData);

    public void OnPointerExit(PointerEventData eventData) => Cell.OnPointerExit(eventData);
}
