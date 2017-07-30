using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltimateHacker
{
    public class GameController : MonoBehaviour
    {
        public GameObject InteractUI;
        public GameObject TerminalUI;
        public GameObject LevelCompleteUI;
        public GameObject PlayerDetectedUI;

        public string MainMenuScene;

        public PlayerController PlayerController;

        public void PlayerDetected()
        {
            this.InteractUI.SetActive(false);
            this.TerminalUI.SetActive(false);
            this.LevelCompleteUI.SetActive(false);

            this.PlayerDetectedUI.SetActive(true);

            this.PlayerController.ControlEnabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void LevelComplete()
        {
            
        }

        public void TryAgain()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Menu()
        {
            SceneManager.LoadScene(this.MainMenuScene);
        }
    }
}
