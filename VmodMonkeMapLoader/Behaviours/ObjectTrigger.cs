using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VmodMonkeMapLoader.Behaviours
{
    [System.Serializable]
    public class ObjectTrigger : MonoBehaviour
    {
        public GameObject ObjectToTrigger;
        public bool OnlyTriggerOnce = false;

        private bool triggered = false;
        void OnTriggerEnter(Collider collider)
        {
            if (triggered && OnlyTriggerOnce)
                return;

            if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
                return;

            ObjectToTrigger.SetActive(false);
            ObjectToTrigger.SetActive(true);
            triggered = true;
        }
    }
}
