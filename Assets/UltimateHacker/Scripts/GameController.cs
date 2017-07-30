using System;
using System.Collections;
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
        public GameObject QuitUI;
        public Camera MainCamera;

        public string MainMenuScene;

        public PlayerController PlayerController;

        public void PlayerDetected(GameObject device)
        {
            this.StartCoroutine(this.LookAt(device.transform.position));
            this.StartCoroutine(this.Zoom());

            this.InteractUI.SetActive(false);
            this.TerminalUI.SetActive(false);
            this.LevelCompleteUI.SetActive(false);

            this.PlayerDetectedUI.SetActive(true);

            this.PlayerController.ControlEnabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private IEnumerator LookAt(Vector3 pos)
        {
            var direction = pos - this.PlayerController.transform.position;
            while (true)
            {
                var toRotation = Quaternion.FromToRotation(this.transform.forward, direction);
                this.MainCamera.transform.rotation = Quaternion.Lerp(
                    this.MainCamera.transform.rotation,
                    toRotation,
                    Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator Zoom()
        {
            while (this.MainCamera.fieldOfView > 30f)
            {
                this.MainCamera.fieldOfView = Mathf.Lerp(
                    this.MainCamera.fieldOfView,
                    30f,
                    Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }

        public void LevelComplete()
        {
            this.PlayerController.ControlEnabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void NextLevel()
        {
            // TODO: Um, go to the next level.
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
