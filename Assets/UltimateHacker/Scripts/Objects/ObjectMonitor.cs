using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UltimateHacker.Objects
{
    public class ObjectMonitor
    {
        private readonly IMonitorableObject _object;
        private readonly ObjectMonitorType _type;

        public float Period { get; private set; }

        private bool _operationPending = false;

        public ObjectMonitor(IMonitorableObject o, ObjectMonitorType type)
        {
            this.Period = this.RandomPeriod();
            this._object = o;
            this._type = type;
        }

        private float RandomPeriod()
        {
            return Random.Range(0f, 5f);
        }

        public IEnumerator Run()
        {
            if (this._operationPending)
                yield break;

            switch (this._type)
            {
                case ObjectMonitorType.None:
                    // Do nothing.
                    break;
                case ObjectMonitorType.Periodic:
                    this._operationPending = true;
                    yield return new WaitForSecondsRealtime(this.Period);
                    this._object.Restart();
                    this._operationPending = false;
                    break;
                case ObjectMonitorType.Random:
                    this._operationPending = true;
                    this.Period = this.RandomPeriod();
                    yield return new WaitForSecondsRealtime(this.Period);
                    this._object.Restart();
                    this._operationPending = false;
                    break;
                case ObjectMonitorType.Event:

                    break;
            }
        }
    }
}
