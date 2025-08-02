using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
	public class PositionSaver : MonoBehaviour
	{
		[System.Serializable]
		public struct Data
		{
			public Vector3 Position;
			public float Time;
		}

		[Tooltip("Для заполнения этого поля используйте контекстное меню в инспекторе и выберите 'Create File'")]
		[ReadOnly, SerializeField]
		private TextAsset _json;

		[SerializeField, HideInInspector]
		public List<Data> Records;

		private void Awake()
		{
			//todo comment: Что будет, если в теле этого условия не сделать выход из метода?
			//метод продолжит выполняться и выйдет exception nullreference
			if (_json == null)
			{
				gameObject.SetActive(false);
				Debug.LogError("Please, create TextAsset and add in field _json");
				return;
			}
			
			JsonUtility.FromJsonOverwrite(_json.text, this);
			//todo comment: Для чего нужна эта проверка (что она позволяет избежать)?
			//нужна, чтобы избежать NullReferenceException, если десериализация не заполнит поле Records
			if (Records == null)
				Records = new List<Data>(10);
		}

		private void OnDrawGizmos()
		{
			//todo comment: Зачем нужны эти проверки (что они позволляют избежать)?
			//чтобы избежать NullReferenceException и пустого списка
			if (Records == null || Records.Count == 0) return;
			var data = Records;
			var prev = data[0].Position;
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(prev, 0.3f);
			//todo comment: Почему итерация начинается не с нулевого элемента?
			//потому что нулевой используется в prev
			for (int i = 1; i < data.Count; i++)
			{
				var curr = data[i].Position;
				Gizmos.DrawWireSphere(curr, 0.3f);
				Gizmos.DrawLine(prev, curr);
				prev = curr;
			}
		}
		
#if UNITY_EDITOR
		[ContextMenu("Create File")]
		private void CreateFile()
		{
			//todo comment: Что происходит в этой строке?
			//File.Create возвращает FileStream, который позволяет записывать данные в файл Path.txt.
			var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));
			//todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав) 
			//Освобождает системные ресурсы, связанные с файловым потоком. Это может привести к ошибкам при попытке повторного доступа к файлу.
			stream.Dispose();
			UnityEditor.AssetDatabase.Refresh();
			//В Unity можно искать объекты по их типу, для этого используется префикс "t:"
			//После нахождения, Юнити возвращает массив гуидов (которые в мета-файлах задаются, например)
			var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
			foreach (var guid in guids)
			{
				//Этой командой можно получить путь к ассету через его гуид
				var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				//Этой командой можно загрузить сам ассет
				var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
				//todo comment: Для чего нужны эти проверки?
				//проверяют что ассет найден и он тот что нужен
				if(asset != null && asset.name == "Path")
				{
					_json = asset;
					UnityEditor.EditorUtility.SetDirty(this);
					UnityEditor.AssetDatabase.SaveAssets();
					UnityEditor.AssetDatabase.Refresh();
					//todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
					//потому что нужный файл мы уже нашли
					return;
				}
			}
		}

		private void OnDestroy()
		{
			//todo logic...
			if (Records == null || Records.Count == 0) return;
			var text = JsonUtility.ToJson(this, true);
			var path = UnityEditor.AssetDatabase.GetAssetPath(_json);
			path = Path.Combine(Application.dataPath.Replace("Assets", ""), path);
			File.WriteAllText(path, text);
			UnityEditor.EditorUtility.SetDirty(_json);
			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
		}
#endif
	}
}