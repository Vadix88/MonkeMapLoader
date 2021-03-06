using GorillaLocomotion;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VmodMonkeMapLoader.Behaviours
{
    public class SurfaceClimbSettings : MonoBehaviour
    {
        public bool Unclimbable = false;
        public float slipPercentage = 0.03f;
        void Start()
        {
            Surface surface = gameObject.GetComponent<Surface>();
            if (surface == null)
            {
                surface = gameObject.AddComponent<Surface>();
            }
            if (Unclimbable)
            {
                surface.slipPercentage = 0;
            }
            else
            {
                surface.slipPercentage = slipPercentage;
            }
        }
    }
}
