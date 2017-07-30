using System.Collections;
using System.Collections.Generic;
using UltimateHacker.Extensions;
using UnityEngine;

namespace UltimateHacker
{
    public class ElevatorDoors : MonoBehaviour
    {
        public Color Color;
        public float OpenDistance = 1.45f;
        public float CloseDistance = 0.5f;
        public float OpenSpeed = 0.05f;

        private bool _doorsOpening = false;
        private bool _doorsClosing = false;
        private bool doorsOpen = false;

        private Transform Left;
        private Transform Right;

        private void Awake()
        {
            this.Left = this.transform.Find("Left");
            this.Right = this.transform.Find("Right");
        }

        private void OnDrawGizmos()
        {
            var oldMatrix = Gizmos.matrix;
            Gizmos.matrix = this.transform.localToWorldMatrix;

            Gizmos.color = this.Color;
            Gizmos.DrawCube(new Vector3(0f, 0f, 0.5f), new Vector3(2f, 2f, 2f));

            Gizmos.matrix = oldMatrix;
        }

        private void OnTriggerEnter(Collider c)
        {
            if (!c.gameObject.IsPlayer())
                return;

            if (!this._doorsOpening)
            {
                this._doorsOpening = true;
                this.StartCoroutine(this.OpenDoors());
            }
        }

        private IEnumerator OpenDoors()
        {
            while (this._doorsClosing)
                yield return new WaitForSecondsRealtime(0.5f);

            var startPos = new Vector3(this.CloseDistance, 0f, 0f);
            var openPos = new Vector3(this.OpenDistance, 0f, 0f);
            var pos = startPos;

            while (pos.x < this.OpenDistance)
            {
                pos = Vector3.Lerp(pos, openPos, Time.deltaTime);
                this.Right.localPosition = pos;
                this.Left.localPosition = pos * -1;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}