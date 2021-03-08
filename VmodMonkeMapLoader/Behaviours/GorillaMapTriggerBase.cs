using UnityEngine;
using GorillaLocomotion;
using Logger = VmodMonkeMapLoader.Helpers.Logger;

namespace VmodMonkeMapLoader.Behaviours
{
    public class GorillaMapTriggerBase : MonoBehaviour
    {
        public GorillaTouchType TouchType = GorillaTouchType.Any;
        public float Delay = 0f;
        //public float Cooldown = 1f;

        private float _touchedTime = 0f;
        private bool _isTriggering = false;
        private Collider _collider;

        void OnTriggerEnter(Collider collider)
        {
            if(_isTriggering) return;
            if (TouchType == GorillaTouchType.Any && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null && collider.GetComponentInParent<Player>() == null) return;
            else if (TouchType == GorillaTouchType.Hands && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null) return;
            else if (TouchType == GorillaTouchType.Head
                     && (collider.GetComponentInParent<Player>() == null
                         || (collider.GetComponentInParent<Player>() != null && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null))
                     ) return;

            _isTriggering = true;
            _collider = collider;

            if (Delay == 0f)
            {
                Trigger(collider);
                _isTriggering = false;
                _collider = null;
            }
            else _touchedTime = 0f;
        }

        void OnTriggerStay(Collider collider)
        {
            if (Delay == 0f) return;
            if (TouchType == GorillaTouchType.Any && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null && collider.GetComponentInParent<Player>() == null) return;
            else if (TouchType == GorillaTouchType.Hands && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null) return;
            else if (TouchType == GorillaTouchType.Head
                     && (collider.GetComponentInParent<Player>() == null
                         || (collider.GetComponentInParent<Player>() != null && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null))
            ) return;

            _touchedTime += Time.fixedDeltaTime;

            if (_touchedTime >= Delay)
            {
                _touchedTime = 0f;
                Trigger(collider);
                _isTriggering = false;
                _collider = null;
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if (TouchType == GorillaTouchType.Any && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null && collider.GetComponentInParent<Player>() == null) return;
            else if (TouchType == GorillaTouchType.Hands && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null) return;
            else if (TouchType == GorillaTouchType.Head
                     && (collider.GetComponentInParent<Player>() == null
                         || (collider.GetComponentInParent<Player>() != null && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null))
            ) return;
            if (_collider != null && _collider != collider) return;

            _isTriggering = false;
            _collider = null;
        }

        public virtual void Trigger(Collider collider)
        {
            // override this method to do stuff
            Logger.LogText("Triggered: " + collider.gameObject.name + "   Filter: " + TouchType);
        }
    }

    public enum GorillaTouchType
    {
        Any,
        Head,
        Hands
    }
}