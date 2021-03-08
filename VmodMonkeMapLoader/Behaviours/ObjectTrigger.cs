using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VmodMonkeMapLoader.Behaviours
{
    [System.Serializable]
    public class ObjectTrigger : GorillaMapTriggerBase
    {
        public GameObject ObjectToTrigger;
        public bool DisableObject = false;
        public bool OnlyTriggerOnce = false;

        private bool _triggered = false;
        void Start()
        {
            if (!DisableObject) ObjectToTrigger.SetActive(false);
            else ObjectToTrigger.SetActive(true);
        }

        public override void Trigger(Collider collider)
        {
            if (_triggered && OnlyTriggerOnce)
                return;

            ObjectToTrigger.SetActive(DisableObject);
            ObjectToTrigger.SetActive(!DisableObject);

            _triggered = true;

            base.Trigger(collider);
        }
    }
}
