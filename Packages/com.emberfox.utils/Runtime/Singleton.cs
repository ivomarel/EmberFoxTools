using System;
using UnityEngine;

namespace Utils
{
	// Singleton base class, for a new sub-systems and/or managers.
	public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		public static T instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindAnyObjectByType<T>();
				}
				return _instance;
			}
		}

		private static T _instance;

		protected virtual void Awake()
		{
			if (_instance == null)
			{
				_instance = this as T;	
			}
			else if (_instance != this)
			{
				Debug.LogWarning($"Singleton {typeof(T)} already exists, destroying {gameObject.name}");
				Destroy(gameObject);
			}
		}
	}

}
