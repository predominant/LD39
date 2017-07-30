using UnityEditor;
using UnityEngine;

namespace UltimateHacker.Editor
{
    [CustomEditor(typeof(LaserBarrier))]
    public class EditorLaserBarrier : UnityEditor.Editor
    {
        public void OnSceneGUI()
        {
            var _target = (LaserBarrier) this.target;
            Handles.color = Color.white;
            Handles.Label(
                _target.transform.position + Vector3.up / 2f,
                _target.gameObject.name);
        }
    }
}