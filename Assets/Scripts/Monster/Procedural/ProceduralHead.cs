using System;
using System.Collections.Generic;
using UnityEngine;

namespace Monster.Procedural
{
    public class ProceduralHead : MonoBehaviour
    {
        [Serializable]
        public struct AnimationPose
        {
            public string name;
            public float transitionTime;
            public AnimationCurve angleCurve;
        }
        
        [SerializeField] private float positionRecordDistance;
        [SerializeField] private float positionRecordingMaxDistance;
        [SerializeField] private float backwardEvaluationDistance;
        [SerializeField] private float curveEvaluationSubstep;
        [SerializeField] private float minimumFowardValue;
        [SerializeField] private List<AnimationPose> poses;
        [SerializeField] private AnimationCurve transitionOrderCurve;
        private Dictionary<string, int> _posesDict;
        private List<Vector3> _positionRecording;
        private ProceduralBody[] _bodyParts;
        private float[] _verticalKeys;
        private float[] _forwardKeys;
        private float[] _angleKeys;
        
        private int _currentPose;
        private int _previousPose;
        private float _transitionTimer;

        private void Start()
        {
            _posesDict = new Dictionary<string, int>();

            for (var i = 0; i < poses.Count; i++)
            {
                _posesDict[poses[i].name] = i;
            }
            
            _positionRecording = new List<Vector3> { transform.position };
            _bodyParts = FindObjectsByType<ProceduralBody>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            Array.Sort(_bodyParts, (x, y) => x.OrderId.CompareTo(y.OrderId));

            _bodyParts[0].distance = 0.1f;

            for (var i = 1; i < _bodyParts.Length; i++)
            {
                _bodyParts[i].distance =
                    Vector3.Distance(_bodyParts[i].transform.position, _bodyParts[i - 1].transform.position) +
                    _bodyParts[i - 1].distance;
            }

            _verticalKeys = new float[_bodyParts.Length];
            _forwardKeys = new float[_bodyParts.Length];
            _angleKeys = new float[_bodyParts.Length];
            
            UpdateBodies();
        }

        private void Update()
        {
            if (_transitionTimer >= 0)
                _transitionTimer -= Time.deltaTime;
            
            TryRemovePoint();
            TryAddPoint();
            UpdateBodies();
        }
        
        private void TryAddPoint()
        {
            if (_positionRecording.Count <= 0)
            {
                _positionRecording.Add(transform.position);
                return;
            }

            while (true)
            {
                var diff = transform.position - _positionRecording[0];
                if (diff.magnitude < positionRecordDistance)
                    return;

                _positionRecording.Insert(0, _positionRecording[0] + diff.normalized * positionRecordDistance);
            }
        }

        private void TryRemovePoint()
        {
            while (true)
            {
                if (_positionRecording.Count < 2)
                    return;
                
                if ((_positionRecording.Count - 1) * positionRecordDistance < positionRecordingMaxDistance)
                    return;

                _positionRecording.RemoveAt(_positionRecording.Count - 1);
            }
        }

        private void UpdateBodies()
        {
            float angle;
            var vertical = 0f;
            var forward = 0f;

            var minVertical = float.MaxValue;
            var minForward = float.MaxValue;
            var maxForward = float.MinValue;
            
            for (var i = 0; i < _bodyParts.Length; i++)
            {
                var distance = _bodyParts[i].distance;

                var diff = distance;
                if (i > 0)
                    diff -= _bodyParts[i - 1].distance;

                var steps = Mathf.FloorToInt(diff / curveEvaluationSubstep);
                var lastStep = diff % curveEvaluationSubstep;
                
                for (var j = 0; j < steps; j++)
                {
                    ProgressCoordinates(distance - diff + j * curveEvaluationSubstep, curveEvaluationSubstep, out angle, ref vertical, ref forward);
                }
                
                ProgressCoordinates(distance - lastStep, lastStep, out angle, ref vertical, ref forward);

                if (vertical < minVertical)
                    minVertical = vertical;

                if (forward < minForward)
                    minForward = forward;

                if (forward > maxForward)
                    maxForward = forward;
                
                _angleKeys[i] = angle;
                _verticalKeys[i] = vertical;
                _forwardKeys[i] = forward;
            }

            for (var i = 0; i < _bodyParts.Length; i++)
            {
                _verticalKeys[i] -= minVertical;
                _verticalKeys[i] += _bodyParts[i].GetVerticalPosition();
                _forwardKeys[i] -= minForward;
                _forwardKeys[i] += minimumFowardValue;
                //_forwardKeys[i] += _bodyParts[^1].GetDistance() - maxForward;

                var position = GetRecordPoint(_forwardKeys[i]);
                position.y = _verticalKeys[i];

                Vector3 normal;
                
                if (i == 0)
                {
                    var backPosition = GetRecordPoint(_forwardKeys[i] + backwardEvaluationDistance);
                    normal = position - backPosition;
                    normal.y = 0;
                    normal.Normalize();
                }
                else
                {
                    var forwardPosition = GetRecordPoint(_forwardKeys[i - 1]);
                    normal = forwardPosition - position;
                    normal.y = 0;
                    normal.Normalize();
                    normal *= Mathf.Sign(_forwardKeys[i] - _forwardKeys[i - 1]);
                }

                angle = _angleKeys[i];
                if (i > 0)
                {
                    angle = Vector2.SignedAngle(
                        new Vector2(_verticalKeys[i - 1] - _verticalKeys[i], _forwardKeys[i - 1] - _forwardKeys[i]).normalized,
                        Vector2.down);
                }

                var verticalRotation = Vector3.SignedAngle(Vector3.forward, normal, Vector3.up);

                var rotation = Quaternion.AngleAxis(verticalRotation, Vector3.up) *
                               Quaternion.AngleAxis(angle + 90, Vector3.right); //* Quaternion.AngleAxis(180, Vector3.up);
                _bodyParts[i].UpdatePosition(position, rotation);
            }
        }

        private void ProgressCoordinates(float distance, float length, out float angle, ref float vertical, ref float forward)
        {
            var time = distance / _bodyParts[^1].distance;
            var factor = Mathf.Clamp01(1 - (2 * (_transitionTimer / poses[_currentPose].transitionTime) - 1 + transitionOrderCurve.Evaluate(time)));
            angle = Mathf.LerpAngle(poses[_previousPose].angleCurve.Evaluate(time), poses[_currentPose].angleCurve.Evaluate(time), factor);
            vertical += Mathf.Sin(angle * Mathf.Deg2Rad) * length;
            forward += Mathf.Cos(angle * Mathf.Deg2Rad) * length;
        }
        
        private Vector3 GetRecordPoint(float distance)
        {
            distance -= Vector3.Distance(GetStartPoint(0), GetEndPoint(0));
            
            var remainder = (distance % positionRecordDistance) / positionRecordDistance;
            var index = Mathf.FloorToInt(distance / positionRecordDistance);

            Vector3 position;

            if (index < 0)
                position = GetStartPoint(0);
            else if (index >= _positionRecording.Count)
                position = GetEndPoint(_positionRecording.Count - 1);
            else
                position = Vector3.Lerp(GetStartPoint(index), GetEndPoint(index), remainder);

            return position;
        }

        private Vector3 GetStartPoint(int point)
        {
            return point == 0 ? transform.position : _positionRecording[point - 1];
        }

        private Vector3 GetEndPoint(int point)
        {
            return _positionRecording[point];
        }

        public ProceduralBody[] GetBodyParts()
        {
            return _bodyParts;
        }

        public void SetPose(string name)
        {
            if (!_posesDict.TryGetValue(name, out var value))
            {
                Debug.LogError($"No pose named {name} exists!");
                return;
            }

            if (_currentPose == value)
                return;

            _previousPose = _currentPose;
            _currentPose = _posesDict[name];
            _transitionTimer = poses[_currentPose].transitionTime;
        }
    }
}