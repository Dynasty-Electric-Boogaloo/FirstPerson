using UnityEngine;

namespace Monster.Procedural
{
    public class ProceduralArm : MonoBehaviour
    {
        [SerializeField] private Transform handTransform;
        [SerializeField] private Transform elbowPoleTransform;
        [SerializeField] private Transform shoulderTransform;
        [SerializeField] private float armLength;
        [SerializeField] private Vector2 direction;
        [SerializeField] private Vector2 handDirection;
        [SerializeField] private float forwardOffset;
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
        private bool _hasTarget;

        private Vector3 _defaultHandPosition;

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
            var position = Vector3.Lerp(_currentStickingPosition + vertical, _previousStickingPosition, forward);
            handTransform.position = float.IsNaN(position.x) ? Vector3.zero : position;
            handTransform.rotation = Quaternion.Lerp(_currentStickingRotation, _previousStickingRotation, forward);

            var projectedHand =
                Vector3.ProjectOnPlane(
                    handTransform.position - 
                    shoulderTransform.position, 
                    -_body.transform.forward) +
                shoulderTransform.position;

            elbowPoleTransform.position = Vector3.Lerp(shoulderTransform.position, projectedHand, .5f) - _body.transform.forward * .5f;
        }

        private void UpdateTargetPoint()
        {
            var shoulder = shoulderTransform.position + transform.up * forwardOffset;
            
            var rayDir = transform.up * direction.y + transform.right * direction.x;
            rayDir.Normalize();
            
            Debug.DrawRay(shoulder, rayDir * armLength);
            var hit = Physics.Raycast(shoulder, rayDir, out _raycastHit, armLength, groundMask);

            if (hit)
            {
                _targetPosition = _raycastHit.point;
                _targetNormal = _raycastHit.normal;
                _hasTarget = true;
                return;
            }

            var height = _body.GetVerticalPosition() + shoulder.y;
            var sideOffset = Mathf.Min(Mathf.Cos(Mathf.Asin( Mathf.Clamp01(height / armLength))) * armLength, maximumArmOffset);
            
            Debug.DrawRay(shoulder + rayDir * sideOffset, transform.forward * (height + .25f));
            hit = Physics.Raycast(shoulder + rayDir * sideOffset, transform.forward, out _raycastHit, Mathf.Min(armLength, height), groundMask);

            if (hit)
            {
                _targetNormal = _raycastHit.normal;
                _targetPosition = _raycastHit.point + _targetNormal * .1f;
                _hasTarget = true;
                return;
            }

            _targetNormal = _body.transform.forward;
            _targetPosition = _body.transform.position;
            _hasTarget = false;
        }

        private void UpdateStickingPoint()
        {
            if ((Vector3.Distance(_currentStickingPosition, _targetPosition) < maxStickingDistance || _transitionTimer > 0) && _hasTarget)
                return;

            _previousStickingPosition = _currentStickingPosition;
            _previousStickingRotation = _currentStickingRotation;
            _previousStickingNormal = _currentStickingNormal;
            
            var rayDir = transform.forward * handDirection.y + transform.right * handDirection.x;
            rayDir.Normalize();
            var forward = Vector3.ProjectOnPlane(rayDir, _targetNormal);
            
            var armDir = transform.up * direction.y + transform.right * direction.x;
            armDir.Normalize();
            var defaultPosition = _body.transform.position + armDir * .5f;
            
            _currentStickingPosition = _hasTarget ? _targetPosition : defaultPosition;
            _currentStickingRotation = Quaternion.LookRotation(forward, _hasTarget ? _targetNormal : _body.transform.forward);
            _currentStickingNormal = _hasTarget ? _targetNormal : _body.transform.forward;
            
            _transitionTime = Random.Range(transitionTime.x, transitionTime.y);
            _transitionTimer = _hasTarget ? _transitionTime : 0;
        }
    }
}