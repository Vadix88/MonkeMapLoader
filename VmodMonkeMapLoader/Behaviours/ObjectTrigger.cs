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
        public bool DisableObject = false;
        public bool OnlyTriggerOnce = false;

        private bool triggered = false;
        void Start()
        {
            if (!DisableObject) ObjectToTrigger.SetActive(false);
            else ObjectToTrigger.SetActive(true);
        }
        void OnTriggerEnter(Collider collider)
        {
            if (triggered && OnlyTriggerOnce)
                return;

            if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
                return;

            ObjectToTrigger.SetActive(DisableObject);
            ObjectToTrigger.SetActive(!DisableObject);

            triggered = true;
        }
    }
}
