using UnityEngine;
using System.Collections.Generic;

public class BowlingGame : MonoBehaviour
{
    [SerializeField]
    private List<Pin> pins;
    [SerializeField]
    private GameObject ball;
    [SerializeField]
    private Transform ballStartPos;
    [SerializeField]
    private  float throwTimeout = 5f;
    [SerializeField]
    private float finishLineZ = -50f;

    private ScoreManager scoreManager;
    private int currentFrame = 0;
    private int currentThrow = 0;
    private List<int> rolls = new List<int>();
    private const int MaxFrames = 10;
    private bool isThrowActive = false;
    private float throwStartTime;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    void Update()
    {
        if (currentFrame >= MaxFrames) return;

        if (isThrowActive)
        {
            if (ball.transform.position.z < finishLineZ || Time.time - throwStartTime > throwTimeout)
            {
                EndThrow();
            }
        }
    }
    
    public void StartThrow()
    {
        if (isThrowActive) return;
        isThrowActive = true;
        throwStartTime = Time.time;
    }

    void EndThrow()
    {
        int fallen = GetFallenPinsCount();

        rolls.Add(fallen);

        if (currentThrow % 2 == 1)
        {
            currentFrame++;
        }
        else 
        {
            if (fallen == 10)
            {
                currentFrame++;
            }
        }

        scoreManager.UpdateScore(rolls);

        ResetBall();
        if (fallen != 10 && currentThrow % 2 == 0)
        {
            ResetPinsForNewThrow();
        }
        else
        {
            ResetAllPins();
        }

        currentThrow++;
        isThrowActive = false;
    }

    int GetFallenPinsCount()
    {
        int count = 0;
        foreach (var pin in pins)
        {
            if (pin.IsKnockedOver() && pin.isActiveAndEnabled)
            {
                count++;
            }
        }
        return count;
    }

    void ResetBall()
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        ball.transform.position = ballStartPos.position;
    }

    void ResetPinsForNewThrow()
    {
        foreach (var pin in pins)
        {
            if (!pin.IsKnockedOver())
            {
                continue;
            }
            pin.RemovePin();
        }
    }

    void ResetAllPins()
    {
        foreach (var pin in pins)
        {
            pin.ResetPin();
        }
    }
}