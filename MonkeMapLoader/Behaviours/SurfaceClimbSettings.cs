using GorillaLocomotion;
using UnityEngine;

namespace VmodMonkeMapLoader.Behaviours
{
    [System.Serializable]
    public class SurfaceClimbSettings : MonoBehaviour
    {
        public bool Unclimbable = false;
        public float slipPercentage = 0.03f;

#if PLUGIN

        void Start()
        {
            Surface surface = gameObject.GetComponent<Surface>();
            if (surface == null)
            {
                surface = gameObject.AddComponent<Surface>();
            }
            if (Unclimbable)
            {
                surface.slipPercentage = 1;
            }
            else
            {
                surface.slipPercentage = slipPercentage;
            }
        }

#endif

    }
}
