using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float rayDistance = 1f;
    [SerializeField] private float turnSpeed = 50f;
    [SerializeField] private float randomTurnInterval = 5f;
    [SerializeField] private Transform leftSideTransform;
    [SerializeField] private Transform rightSideTransform;

    private bool isTurning = false;
    private float nextRandomTurn = 0f;
    private int turnDirection = 1;

    void Start()
    {
        SetNextRandomTurn();
    }

    void Update()
    {
        Move();
        CheckObstacles();
        RandomTurn();
    }

    void Move()
    {
        if (!isTurning)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else
        {
            transform.Rotate(0, turnSpeed * Time.deltaTime * turnDirection, 0);
        }
    }

    void CheckObstacles()
    {
        turnDirection = Physics.Raycast(leftSideTransform.position, leftSideTransform.forward, rayDistance ) ? 1 : 
                        Physics.Raycast(rightSideTransform.position, rightSideTransform.forward, rayDistance) ? -1 :
                        Physics.Raycast(transform.position, transform.forward, rayDistance) ? Random.Range(0, 2) * 2 - 1 : 0;

        bool obstacleDetected = turnDirection != 0;

        if (obstacleDetected && !isTurning)
        {
            StartTurning();
        }
        else if (!obstacleDetected && isTurning)
        {
            isTurning = false;
        }

        // Визуализация лучей
        Debug.DrawRay(transform.position, transform.forward * rayDistance, obstacleDetected ? Color.red : Color.green);
        Debug.DrawRay(leftSideTransform.position, leftSideTransform.forward * rayDistance, obstacleDetected ? Color.red : Color.green);
        Debug.DrawRay(rightSideTransform.position, rightSideTransform.forward * rayDistance, obstacleDetected ? Color.red : Color.green);
    }

    void StartTurning()
    {
        if (!isTurning)
        {
            isTurning = true;
        }
    }

    void RandomTurn()
    {
        if (!isTurning && Time.time >= nextRandomTurn)
        {
            turnDirection = Random.Range(0, 2) * 2 - 1;
            StartTurning();
            SetNextRandomTurn();
        }
    }

    void SetNextRandomTurn()
    {
        nextRandomTurn = Time.time + Random.Range(randomTurnInterval * 0.8f, randomTurnInterval * 1.2f);
    }
}
