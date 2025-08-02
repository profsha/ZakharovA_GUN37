using System;
using UnityEngine;

namespace DefaultNamespace
{
	[RequireComponent(typeof(PositionSaver))]
	public class ReplayMover : MonoBehaviour
	{
		private PositionSaver _save;

		private int _index;
		private PositionSaver.Data _prev;
		private float _duration;

		private void Start()
		{
			////todo comment: зачем нужны эти проверки?
			// проверяют что есть сохраненные точки для передвижения
			if (!TryGetComponent(out _save) || _save.Records.Count == 0)
			{
				Debug.LogError("Records incorrect value", this);
				//todo comment: Для чего выключается этот компонент?
				//чтобы не выполнялся update, в котором _save используется
				enabled = false;
			}
		}

		private void Update()
		{
			var curr = _save.Records[_index];
			//todo comment: Что проверяет это условие (с какой целью)? 
			//когда текущее время больше сохраненного то следует переход к следующей точке
			if (Time.time > curr.Time)
			{
				_prev = curr;
				_index++;
				//todo comment: Для чего нужна эта проверка?
				//если следующей точки нету - компонент выключается и в пишется сообщение в лог
				if (_index >= _save.Records.Count)
				{
					enabled = false;
					Debug.Log($"<b>{name}</b> finished", this);
				}
			}
			//todo comment: Для чего производятся эти вычисления (как в дальнейшем они применяются)?
			//отношение текущего времени к сохраненному - чтобы было понятно в какой точке пути находимся
			var delta = (Time.time - _prev.Time) / (curr.Time - _prev.Time);
			//todo comment: Зачем нужна эта проверка?
			//при делении на 0 может быть значение NaN, которое нельзя использовать в дальнейших расчетах
			if (float.IsNaN(delta)) delta = 0f;
			//todo comment: Опишите, что происходит в этой строчке так подробно, насколько это возможно
			//считается новое положение объекта интерполяцией между предыдущей сохраненной точкой и текущей
			//delta используется как показатель пройденного пути
			transform.position = Vector3.Lerp(_prev.Position, curr.Position, delta);
		}
	}
}