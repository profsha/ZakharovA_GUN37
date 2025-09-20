using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell: MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private MeshRenderer focused;
    [SerializeField]
    private MeshRenderer selected;
    [field: SerializeField]
    public Team HomeTeam { get; private set; }

    public Dictionary<NeighbourType, Cell> Neighbours { get; set; }

    public Unit Unit { get; set; }
    
    public event Action<Cell> OnCellClicked;

    public void SetSelected(bool selected, Material material = null)
    {
        this.selected.enabled = selected;
        this.selected.sharedMaterial = material;
    }

    public void OnPointerClick(PointerEventData eventData) => OnCellClicked?.Invoke(this);

    public void OnPointerEnter(PointerEventData eventData) => focused.enabled = true;

    public void OnPointerExit(PointerEventData eventData) => focused.enabled = false;
}
