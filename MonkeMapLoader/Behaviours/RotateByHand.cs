using UnityEngine;

namespace VmodMonkeMapLoader.Behaviours
{
    public class RotateByHand : MonoBehaviour
    {
        private Vector3 _midPoint = Vector3.zero;
        private float _angle;
        private Vector3 _startVector;

        // Angular Momentum Stuff
        private Rigidbody _rigidbody;
        float _deltaAngle;
        float _previousAngle;

        void Awake()
        {
            var renderer = GetComponent<Renderer>();
            if(renderer == null) return;

            _midPoint = renderer.bounds.center;

            // Rigidbody Stuff
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null) _rigidbody = gameObject.AddComponent<Rigidbody>();
            _rigidbody.useGravity = false; // don't want it falling through the floor lmao
            _rigidbody.angularDrag = 0.8f; // to get that sweet, sweet speed reduction over time
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
                return;
            if(_midPoint == Vector3.zero)
                return;

            _angle = transform.rotation.eulerAngles.y;
            _startVector = collider.ClosestPoint(_midPoint) - _midPoint;
            _previousAngle = _angle;
        }

        void OnTriggerStay(Collider collider)
        {
            if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
                return;
            if (_midPoint == Vector3.zero)
                return;

            var currentVector = collider.ClosestPoint(_midPoint) - _midPoint;
            var vectorAngle = Vector3.SignedAngle(currentVector, _startVector, Vector3.up);

            // MoveRotation is preferred over changing the transform's rotation directly because you get interpolation
            _rigidbody.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles.x, _angle - vectorAngle, transform.rotation.eulerAngles.z));

            // Save the change in angle
            _deltaAngle = vectorAngle - _previousAngle;
            _previousAngle = vectorAngle;
        }

        void OnTriggerExit(Collider collider)
        {
            if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
                return;
            if (_midPoint == Vector3.zero)
                return;

            // Calculate angular velocity based on the last delta angle from OnTriggerStay
            var angularVelocity = Vector3.up * (-_deltaAngle / Time.fixedDeltaTime);
            _rigidbody.angularVelocity = angularVelocity;
        }
    }
}