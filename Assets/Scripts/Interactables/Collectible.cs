using UI;
using UnityEngine;

namespace Interactables
{
	public class Collectible : Interactable
	{
		[SerializeField] private Transform modelTransform;
		[SerializeField] private Vector3 rotationSpeed;
		[SerializeField] private float bounceFrequency;
		[SerializeField] private float bounceAmplitude;

		private void Update()
		{
			modelTransform.rotation *= Quaternion.Euler(rotationSpeed * Time.deltaTime);
			modelTransform.localPosition = new Vector3(0, Mathf.Sin(Time.time * bounceFrequency * Mathf.PI) * bounceAmplitude, 0);
		}

		public override void Interact()
		{
			Break();
			UiManager.AddCollected();
		}

		public override bool IsInteractable()
		{
			return true;
		}

		public override InteractionType GetInteractionType()
		{
			return InteractionType.Collectible;
		}
	}
}