using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateHacker.Extensions
{
    public static class GameObjectExtensions
    {
        public static bool IsPlayer(this GameObject g)
        {
            return g.layer == LayerMask.NameToLayer("Player");
        }
    }
}