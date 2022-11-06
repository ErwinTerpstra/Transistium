using System;
using System.Collections.Generic;
using Transistium.Interaction;
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

        [SerializeField]
        private Button stopButton = null;

        private TransistiumApplication application;

        private void Awake()
		{
            application = FindObjectOfType<TransistiumApplication>();

            playButton.onClick.AddListener(OnPlayClicked);
            pauseButton.onClick.AddListener(OnPauseClicked);
            stopButton.onClick.AddListener(OnStopButton);
        }

		private void Start()
		{
            UpdateButtons();
		}

		private void Update()
        {
            if (application.State == ApplicationState.SIMULATING)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    application.Pause();
            }
            else if (application.State == ApplicationState.PAUSED)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    application.Play();
            }

            UpdateButtons();
        }

		private void UpdateButtons()
		{
            var state = application.State;
            
            playButton.gameObject.SetActive(state == ApplicationState.DESIGNING || state == ApplicationState.PAUSED);
            pauseButton.gameObject.SetActive(state == ApplicationState.SIMULATING);
            stopButton.gameObject.SetActive(state == ApplicationState.SIMULATING || state == ApplicationState.PAUSED);
		}

        private void OnPlayClicked()
		{
            application.Play();
		}

        private void OnPauseClicked()
		{
            application.Pause();
        }

        private void OnStopButton()
		{
            application.Stop();
        }

    }

}