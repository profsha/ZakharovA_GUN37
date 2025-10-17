using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float viewRadius = 8f; // Радиус обзора
    public Vector3 viewCenterOffset = Vector3.zero; // Смещение центра обзора относительно позиции противника

    [SerializeField]
    private Transform playerTransform;
    private Vector3 _viewCenter; // Центр области обзора

    void Start()
    {    
        UpdateViewCenter();
    }

    void Update()
    {
        UpdateViewCenter();
    }

    void UpdateViewCenter()
    {
        _viewCenter = transform.position + viewCenterOffset;
    }

    // Проверка, видит ли противник игрока
    bool IsPlayerInSight()
    {
        if (playerTransform == null) return false;
        Vector3 directionToPlayer = playerTransform.position - _viewCenter;
        float distanceToPlayer = directionToPlayer.magnitude;

        return distanceToPlayer <= viewRadius;
    }

    void OnDrawGizmosSelected()
    {
        // Центр обзора
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_viewCenter, viewRadius);

        if (IsPlayerInSight())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_viewCenter, viewRadius);
        }
    }
}
