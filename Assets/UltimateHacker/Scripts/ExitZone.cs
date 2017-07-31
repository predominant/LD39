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

        private void OnDrawGizmos()
        {
            var oldMatrix = Gizmos.matrix;
            Gizmos.matrix = this.transform.localToWorldMatrix;

            Gizmos.color = new Color(0.6f, 0.1f, 0.6f, 0.4f);
            Gizmos.DrawCube(
                this.GetComponent<BoxCollider>().center,
                this.GetComponent<BoxCollider>().size);

            Gizmos.matrix = oldMatrix;
        }
    }
}