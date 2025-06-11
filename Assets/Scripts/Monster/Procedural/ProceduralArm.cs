using UnityEngine;

namespace Monster.Procedural
{
    public class ProceduralArm : MonoBehaviour
    {
        [SerializeField] private Transform handTransform;
        [SerializeField] private Vector2 direction;
        [SerializeField] private Vector2 handDirection;
        [SerializeField] private float forwardOffset;
        [SerializeField] private InverseKinematicArm ikArm;
        [SerializeField] private float maximumArmOffset;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float maxStickingDistance;
        [SerializeField] private Vector2 transitionTime;
        [SerializeField] private AnimationCurve transitionFoward;
        [SerializeField] private AnimationCurve transitionHeight;
        private ProceduralBody _body;
        private RaycastHit _raycastHit;
        private Vector3 _targetPosition;
        private Vector3 _targetNormal;
        private Vector3 _currentStickingPosition;
        private Vector3 _currentStickingNormal;
        private Quaternion _currentStickingRotation;
        private Vector3 _previousStickingPosition;
        private Vector3 _previousStickingNormal;
        private Quaternion _previousStickingRotation;
        private float _transitionTimer;
        private float _transitionTime;

        private void Start()
        {
            direction.Normalize();
            handDirection.Normalize();
            _body = GetComponent<ProceduralBody>();
            handTransform.SetParent(null);
            _currentStickingRotation = Quaternion.identity;
            _previousStickingRotation = Quaternion.identity;
        }

        private void FixedUpdate()
        {
            UpdateTargetPoint();
            UpdateStickingPoint();

            if (_transitionTimer > 0)
                _transitionTimer -= Time.fixedDeltaTime;

            var factor = _transitionTimer / _transitionTime;
            var normal = Vector3.Slerp(_currentStickingNormal, _previousStickingNormal, factor);
            var vertical = normal * transitionHeight.Evaluate(factor);
            var forward = transitionFoward.Evaluate(factor);
            handTransform.position = Vector3.Lerp(_currentStickingPosition + vertical, _previousStickingPosition, forward);
            handTransform.rotation = Quaternion.Lerp(_currentStickingRotation, _previousStickingRotation, forward);
        }

        private void UpdateTargetPoint()
        {
            var shoulder = ikArm.GetShoulderPoint() + transform.up * forwardOffset; //transform.position + transform.rotation * shoulderOffset;
            
            var rayDir = transform.up * direction.y + transform.right * direction.x;
            rayDir.Normalize();
            
            Debug.DrawRay(shoulder, rayDir * ikArm.GetArmLength());
            var hit = Physics.Raycast(shoulder, rayDir, out _raycastHit, ikArm.GetArmLength(), groundMask);

            if (hit)
            {
                _targetPosition = _raycastHit.point;
                _targetNormal = _raycastHit.normal;
                return;
            }

            var height = _body.GetVerticalPosition() + shoulder.y;
            var sideOffset = Mathf.Min(Mathf.Cos(Mathf.Asin( Mathf.Clamp01(height / ikArm.GetArmLength()))) * ikArm.GetArmLength(), maximumArmOffset);
            
            Debug.DrawRay(shoulder + rayDir * sideOffset, transform.forward * (height + .25f));
            hit = Physics.Raycast(shoulder + rayDir * sideOffset, transform.forward, out _raycastHit, Mathf.Min(ikArm.GetArmLength(), height), groundMask);

            if (hit)
            {
                _targetPosition = _raycastHit.point;
                _targetNormal = _raycastHit.normal;
                return;
            }
            
            _targetPosition = Vector3.zero;
        }

        private void UpdateStickingPoint()
        {
            if (Vector3.Distance(_currentStickingPosition, _targetPosition) < maxStickingDistance || _transitionTimer > 0)
                return;

            _previousStickingPosition = _currentStickingPosition;
            _previousStickingRotation = _currentStickingRotation;
            _previousStickingNormal = _currentStickingNormal;
            
            var rayDir = transform.forward * handDirection.y + transform.right * handDirection.x;
            rayDir.Normalize();
            var forward = Vector3.ProjectOnPlane(rayDir, _targetNormal);
            
            _currentStickingPosition = _targetPosition;
            _currentStickingRotation = Quaternion.LookRotation(forward, _targetNormal);
            _currentStickingNormal = _targetNormal;
            
            _transitionTime = Random.Range(transitionTime.x, transitionTime.y);
            _transitionTimer = _transitionTime;
        }
    }
}