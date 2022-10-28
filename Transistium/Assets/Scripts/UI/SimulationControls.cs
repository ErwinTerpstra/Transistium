using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Transistium.UI
{
    public class SimulationControls : MonoBehaviour
    {
        [SerializeField]
        private Button playButton = null;

        [SerializeField]
        private Button pauseButton = null;

		private void Awake()
		{
            playButton.onClick.AddListener(OnPlayClicked);
            pauseButton.onClick.AddListener(OnPauseClicked);
		}

        private void OnPlayClicked()
		{

		}

        private void OnPauseClicked()
		{

		}

    }

}