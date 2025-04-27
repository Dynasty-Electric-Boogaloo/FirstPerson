using System;
using UnityEngine;

namespace Interactables
{
	public class InteractableManager : MonoBehaviour
	{
		private static InteractableManager _instance;
		private Interactable[] _interactables;

		private void Awake()
		{
			if (_instance == null)
				_instance = this;
		}

		private void OnDestroy()
		{
			if (_instance == this)
				_instance = null;
		}

		private void Start()
		{
			_interactables = FindObjectsByType<Interactable>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
		}

		private void Update()
		{
			if (_interactables.Length > 0)
				return;
			
			_interactables = FindObjectsByType<Interactable>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
		}

		public static void Restore()
		{
			if (!_instance)
				return;

			if (_instance._interactables == null)
				return;
			
			foreach (var interactable in _instance._interactables)
			{
				interactable.Restore();
			}
		}
	}
}