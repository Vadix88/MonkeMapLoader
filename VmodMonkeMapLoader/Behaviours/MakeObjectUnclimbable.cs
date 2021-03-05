using GorillaLocomotion;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VmodMonkeMapLoader.Behaviours
{
    public class MakeObjectUnclimbable : MonoBehaviour
    {
        void Start()
        {
            Surface surface = gameObject.GetComponent<Surface>();
            if (surface == null)
            {
                surface = gameObject.AddComponent<Surface>();
            }
            surface.slipPercentage = 0;
        }
    }
}
