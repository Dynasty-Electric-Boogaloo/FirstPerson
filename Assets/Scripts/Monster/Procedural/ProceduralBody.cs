using UnityEngine;

namespace Monster.Procedural
{
    public class ProceduralBody : MonoBehaviour
    {
        [SerializeField] private ProceduralHead targetHead;
        [SerializeField] private float distance;
        [SerializeField] private float hoverHeight;
        [SerializeField] private float gravity;
        [SerializeField] private AnimationCurve movementSpeed;

        private void FixedUpdate()
        {
            UpdatePosition();
            UpdateRotation();
        }

        private void UpdatePosition()
        {
            var targetPosition = targetHead.GetPointAtDistance(distance);
            targetPosition.Position.y += hoverHeight;
            if (float.IsNaN(targetPosition.Position.x) || float.IsNaN(targetPosition.Position.z))
                return;
            
            transform.position = targetPosition.Position;

            /*var targetPosition = GetTargetPosition();

            var targetDiff = targetPosition - transform.position;
            targetDiff.y = 0;

            var position = Vector3.Lerp(transform.position, targetPosition,
                movementSpeed.Evaluate(targetDiff.magnitude - distance) * Time.fixedDeltaTime);
            position.y = hoverHeight;

            _rigidbody.linearVelocity = (position - transform.position) / Time.fixedDeltaTime;
            transform.position = new Vector3(transform.position.x, hoverHeight, transform.position.z);*/
        }

        private void UpdateRotation()
        {
            /*transform.rotation = Quaternion.Slerp(transform.rotation, targetPoint.rotation, 
                 rotationSpeed.Evaluate(Quaternion.Angle(transform.rotation, targetPoint.rotation)) * Time.fixedDeltaTime);*/
        }
    }
}