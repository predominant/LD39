using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateHacker
{
    public class PlayerController : MonoBehaviour
    {
        public bool ControlEnabled = true;

        public float MoveSpeed = 2f;
        public float TurnSpeed = 2f;

        private Rigidbody _rigidbody;
        private Vector3 _inputForce = Vector3.zero;
        private float _inputAngularForce = 0f;

        private Vector3 _oldMousePosition = Vector3.zero;

        public IActionTarget ActionTarget;

        private void Awake()
        {
            this._rigidbody = this.GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            this.ProcessControls();
        }

        private void ProcessControls()
        {
            if (!this.ControlEnabled)
                return;

            this._inputForce = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
                this._inputForce += this.transform.forward * this.MoveSpeed;

            if (Input.GetKey(KeyCode.S))
                this._inputForce -= this.transform.forward * this.MoveSpeed;

            if (Input.GetKey(KeyCode.D))
                this._inputForce += this.transform.right * this.MoveSpeed;

            if (Input.GetKey(KeyCode.A))
                this._inputForce -= this.transform.right * this.MoveSpeed;

            if (Input.GetKey(KeyCode.Space))
                this._inputForce += Vector3.up * this.MoveSpeed;

            if (Input.GetKey(KeyCode.LeftControl))
                this._inputForce -= Vector3.up * this.MoveSpeed;

            this._inputAngularForce = Input.GetAxis("Mouse X") * this.TurnSpeed;

            if (Input.GetKey(KeyCode.E))
                if (this.ActionTarget != null)
                    this.ActionTarget.DoAction();

            if (Input.GetKeyDown(KeyCode.RightBracket))
                if (this.gameObject.layer == 10)
                    this.gameObject.layer = 0;
                else
                    this.gameObject.layer = 10;
        }

        private void FixedUpdate()
        {
            if (!this.ControlEnabled)
                return;

            this._rigidbody.AddForce(this._inputForce);
            this._rigidbody.AddTorque(this.transform.up * this._inputAngularForce);
        }
    }
}