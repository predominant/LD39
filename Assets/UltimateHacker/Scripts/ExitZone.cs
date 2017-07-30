using System.Collections;
using System.Collections.Generic;
using UltimateHacker.Extensions;
using UnityEngine;

namespace UltimateHacker
{
    public class ExitZone : MonoBehaviour
    {
        private GameController _gameController;

        private void Awake()
        {
            this._gameController = GameObject.Find("GameController").GetComponent<GameController>();
        }

        private void OnTriggerEnter(Collider c)
        {
            if (!c.gameObject.IsPlayer())
                return;

            this._gameController.LevelComplete();
        }
    }
}