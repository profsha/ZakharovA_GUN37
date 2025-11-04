using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMover : MonoBehaviour
{
    [Header("Feel")]
    [SerializeField] float moveSpeed   = 10f;   // скорость W/A/S/D
    [SerializeField] float boostMul    = 3f;    // ускорение при LeftShift
    [SerializeField] float orbitSpeed  = 3f;    // чувствительность орбиты
    [SerializeField] float panSpeed    = 0.15f; // чувствительность стрейфа
    [SerializeField] float zoomSpeed   = 0.8f;  // чувствительность зума
    [SerializeField] float zoomMin     = 1f;    // минимальная дистанция
    [SerializeField] float zoomMax     = 200f;  // максимальная дистанция

    Camera cam;          // камера, которую двигаем
    Vector3 pivot;       // точка, вокруг которой орбит
    float distance;      // текущее расстояние до pivot

    // текущие кэши состояний
    Vector2 lastMouse;
    bool    isOrbiting;  // ЛКМ или Alt+ЛКМ
    bool    isPanning;   // ПКМ
    bool    isBoosting;  // LeftShift

    // InputSystem действия (создаём в инспекторе или кодом)
    InputAction orbit, pan, zoom, move, boost, escape;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = GetComponentInChildren<Camera>();
        pivot = transform.position + transform.forward * 10f; // стартовый фокус
        distance = Vector3.Distance(transform.position, pivot);

        // создаём экшены прямо в коде (можно сделать через .inputactions-ассет)
        var map = new InputActionMap("Camera");

        orbit  = map.AddAction("orbit",  binding: "<Mouse>/leftButton");
        pan    = map.AddAction("pan",    binding: "<Mouse>/rightButton");
        zoom   = map.AddAction("zoom",   binding: "<Mouse>/scroll");
        move   = map.AddAction("move",   binding: "2DVector", expectedControlLayout: "Vector2");
        boost  = map.AddAction("boost",  binding: "<Keyboard>/leftShift");
        escape = map.AddAction("escape", binding: "<Keyboard>/escape");

        move.AddCompositeBinding("2DVector")
            .With("Up",    "<Keyboard>/w")
            .With("Down",  "<Keyboard>/s")
            .With("Left",  "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        map.Enable();
    }

    void Update()
    {
        Vector2 mouse = Mouse.current.position.ReadValue();
        Vector2 delta = mouse - lastMouse;
        lastMouse = mouse;

        isOrbiting = orbit.ReadValue<float>() > 0.5f &&
                     (Keyboard.current.altKey.isPressed == false ||
                      Mouse.current.leftButton.isPressed);
        isPanning  = pan.ReadValue<float>()  > 0.5f;
        isBoosting = boost.ReadValue<float>() > 0.5f;

        // 1) Орбита (ЛКМ)
        if (isOrbiting)
        {
            float yaw   = -delta.x * orbitSpeed;
            float pitch = -delta.y * orbitSpeed;

            transform.RotateAround(pivot, Vector3.up,    yaw);
            transform.RotateAround(pivot, transform.right, pitch);

            // не даём перевернуться «вверх ногами»
            float angle = Vector3.Angle(transform.up, Vector3.up);
            if (angle < 5f || angle > 175f)
                transform.RotateAround(pivot, transform.right, -pitch);

            distance = Vector3.Distance(transform.position, pivot);
        }

        // 2) Стрейф (ПКМ) – двигаем камеру и pivot вместе
        if (isPanning)
        {
            Vector3 right = -delta.x * panSpeed * transform.right;
            Vector3 up    = -delta.y * panSpeed * transform.up;
            Vector3 shift = right + up;
            transform.position += shift;
            pivot += shift;
        }

        // 3) Зум колёсиком
        float wheel = zoom.ReadValue<Vector2>().y * zoomSpeed * 0.01f;
        distance = Mathf.Clamp(distance - wheel, zoomMin, zoomMax);
        transform.position = pivot - transform.forward * distance;

        // 4) W/A/S/D + Q/E
        Vector2 wasd = move.ReadValue<Vector2>(); // x = A/D, y = W/S
        float speed = (isBoosting ? moveSpeed * boostMul : moveSpeed) * Time.deltaTime;

        Vector3 moveDir =
            transform.forward * wasd.y +
            transform.right   * wasd.x;

        if (Keyboard.current.qKey.isPressed) moveDir += Vector3.down;
        if (Keyboard.current.eKey.isPressed) moveDir += Vector3.up;

        transform.position += moveDir * speed;
        pivot += moveDir * speed; // фокус едет вместе с камерой
    }
}

