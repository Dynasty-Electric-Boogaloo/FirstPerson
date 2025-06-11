using UnityEngine;

namespace Monster.Procedural
{
    public class InverseKinematicArm : MonoBehaviour
    {
        [SerializeField] private Transform targetPoint;
        [SerializeField] private Transform bodyPoint;
        [SerializeField] private Transform shoulderPoint;
        [SerializeField] private Transform armPoint;
        [SerializeField] private Transform elbowPoint;
        [SerializeField] private Transform handPoint;
        [SerializeField] private bool leftArm;
        private float _armLength;

        private void Awake()
        {
            var upperArmLength = Vector3.Distance(armPoint.position, elbowPoint.position);
            var forearmLength = Vector3.Distance(elbowPoint.position, handPoint.position);
            _armLength = upperArmLength + forearmLength;
        }

        private void FixedUpdate()
        {
            var armSign = (leftArm ? -1 : 1);

            var targetDirection = (targetPoint.position - shoulderPoint.position).normalized;
            
            Debug.DrawRay(shoulderPoint.position, targetDirection, Color.red);
            Debug.DrawRay(shoulderPoint.position, -bodyPoint.forward, Color.blue);
            
            var shoulderRotation =
                Quaternion.AngleAxis(
                    Vector3.SignedAngle(bodyPoint.right * armSign, targetDirection, -bodyPoint.forward), 
                    -bodyPoint.forward);
            Debug.DrawRay(shoulderPoint.position, bodyPoint.right * armSign, Color.yellow);
            Debug.DrawRay(shoulderPoint.position, shoulderRotation * bodyPoint.right * armSign, Color.green);
            shoulderPoint.forward = bodyPoint.forward;
            shoulderPoint.right = shoulderRotation * (-bodyPoint.right * armSign);
        }

        public float GetArmLength()
        {
            return _armLength;
        }

        public Vector3 GetShoulderPoint()
        {
            return shoulderPoint.position;
        }
    }
}