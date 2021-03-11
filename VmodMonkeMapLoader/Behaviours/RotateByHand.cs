using UnityEngine;

namespace VmodMonkeMapLoader.Behaviours
{
    public class RotateByHand : MonoBehaviour
    {
        private Vector3 _midPoint = Vector3.zero;
        private float _angle;
        private Vector3 _startVector;

        void Awake()
        {
            var renderer = GetComponent<Renderer>();
            if(renderer == null)
                return;

            _midPoint = renderer.bounds.center;
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
                return;
            if(_midPoint == Vector3.zero)
                return;

            _angle = transform.rotation.eulerAngles.y;
            _startVector = collider.ClosestPoint(_midPoint) - _midPoint;
        }

        void OnTriggerStay(Collider collider)
        {
            if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
                return;
            if (_midPoint == Vector3.zero)
                return;

            var currentVector = collider.ClosestPoint(_midPoint) - _midPoint;
            var vectorAngle = Vector3.SignedAngle(currentVector, _startVector, Vector3.up);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, _angle - vectorAngle, transform.rotation.eulerAngles.z);
        }
    }
}