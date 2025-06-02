using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace UI
{
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text fpsText;
		private List<float> _deltaTimes;

		private void Awake()
		{
			_deltaTimes = new List<float>();
		}

		private void Update()
		{
			_deltaTimes.Insert(0, Time.deltaTime);

			while (_deltaTimes.Count > 60)
			{
				_deltaTimes.RemoveAt(_deltaTimes.Count - 1);
			}

			var fps = 0f;
			foreach (var deltaTime in _deltaTimes)
			{
				fps += deltaTime;
			}

			fpsText.text = (1 / (fps / 60)).ToString(CultureInfo.InvariantCulture);
		}
    }
}