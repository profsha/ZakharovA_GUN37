using UnityEngine;

namespace DefaultNamespace
{
	
	[RequireComponent(typeof(PositionSaver))]
	public class EditorMover : MonoBehaviour
	{
		private PositionSaver _save;
		private float _currentDelay;
		
		//todo comment: Что произойдёт, если _delay > _duration?
		//метод Update завершится до первого сохранения
		[Range(0.2f, 1.0f)]
		[SerializeField]
		private float _delay = 0.5f;
		[Min(0.2f)]
		[SerializeField]
		private float _duration = 5f;

		private void Start()
		{
			_duration = Mathf.Max(0.2f, _duration);
			
			if (_duration <= _delay)
			{
				_duration = _delay * 5f;
			}
			
			//todo comment: Почему этот поиск производится здесь, а не в начале метода Update?
			//Компонент _save не меняется во время работы скрипта, поэтому нет смысла искать его каждый раз
			_save = GetComponent<PositionSaver>();
			_save.Records.Clear();
		}

		private void Update()
		{
			_duration -= Time.deltaTime;
			if (_duration <= 0f)
			{
				enabled = false;
				Debug.Log($"<b>{name}</b> finished", this);
				return;
			}
			
			//todo comment: Почему не написать (_delay -= Time.deltaTime;) по аналогии с полем _duration?
			//_delay - фиксированное значение, к которому возврещается _currentDelay
			_currentDelay -= Time.deltaTime;
			if (_currentDelay <= 0f)
			{
				_currentDelay = _delay;
				_save.Records.Add(new PositionSaver.Data
				{
					Position = transform.position,
					//todo comment: Для чего сохраняется значение игрового времени?
					//в дальнейшем используется для воспроизведения движения объекта с правильной временной привязкой
					Time = Time.time,
				});
			}
		}
	}
}