using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class DOTweenController : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints = new ();
    [SerializeField] private float delay = 1f;
    [SerializeField] private float duration = 5f;
    [SerializeField] private Ease ease = Ease.InOutQuad;
    
    private Sequence _sequence;
    
    // Start is called before the first frame update
    void Start()
    {
        var meshRenderer = GetComponent<Renderer>();
        _sequence = DOTween.Sequence();
        _sequence.Append(transform.DOPath(waypoints.Select(x => x.transform.position).ToArray(), duration, PathType.CatmullRom)
            .SetOptions(true)
            //кривая изменения
            .SetEase(ease));
        //интервал запуска
        _sequence.PrependInterval(delay);
        //бесконечный цикл
        _sequence.SetLoops(-1, LoopType.Restart);
        //добавляем смену масштаба
        _sequence.Join(transform.DOScale(1.2f, 0.5f).SetEase(Ease.OutBounce));
        //добавляем сменц цвета
        _sequence.Join(meshRenderer.material.DOColor(Color.red, "_Color", duration));
    }

    // Update is called once per frame
    void Update()
    {
        var speed = _sequence.timeScale;
        // кнопки + / - ускоряют / замедляют анимацию
        if (Input.GetKeyDown(KeyCode.Plus)) speed *= 1.2f;
        if (Input.GetKeyDown(KeyCode.Minus)) speed /= 1.2f;
        _sequence.timeScale = speed;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var originalPos = transform.position;
            // атака - при нажатии Space персонаж совершает резкое движение вперед и возврат
            transform.DOMove(transform.position + Vector3.forward * 2, 0.2f)
                .OnComplete(() => transform.DOMove(originalPos, 0.2f));
        }
    }
}
