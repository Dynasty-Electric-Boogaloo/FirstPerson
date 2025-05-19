using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Monster.Procedural
{
    public class ProceduralHead : MonoBehaviour
    {
        public struct RecordPoint
        {
            public Vector3 Position;

            public RecordPoint(Transform transform)
            {
                Position = transform.position;
            }

            public RecordPoint SetVerticalOffset(float offset)
            {
                Position.y = offset;
                return this;
            }

            public static float Distance(RecordPoint a, RecordPoint b)
            {
                var diff = a.Position - b.Position;
                diff.y = 0;
                return diff.magnitude;
            }

            public static RecordPoint Lerp(RecordPoint a, RecordPoint b, float t)
            {
                return new RecordPoint()
                {
                    Position = Vector3.Lerp(a.Position, b.Position, t),
                };
            }
        }

        public struct CurvePoint
        {
            public Vector3 Position;
            public Quaternion Rotation;
        }

        [Serializable]
        public struct AnimationPose
        {
            public string name;
            public float transitionTime;
            public AnimationCurve verticalCurve;
            public AnimationCurve forwardCurve;
        }
        
        [SerializeField] private float positionRecordDistance;
        [SerializeField] private float positionRecordingMaxDistance;
        [SerializeField] private float curvePointDistance;
        [SerializeField] private float curvePointTotalDistance;
        [SerializeField] private float backwardEvaluationDistance;
        [SerializeField] private List<AnimationPose> poses;
        [SerializeField] private AnimationCurve transitionOrderCurve;
        [SerializeField] private Transform travelPoint;
        private Dictionary<string, int> _posesDict;
        private List<RecordPoint> _positionRecording;
        private List<CurvePoint> _curve;
        
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
            
            _positionRecording = new List<RecordPoint>();
            _positionRecording.Add(new RecordPoint(transform));
            _curve = new List<CurvePoint>();
            BakeCurve();
        }

        private void Update()
        {
            if (_transitionTimer >= 0)
                _transitionTimer -= Time.deltaTime;
            
            TryRemovePoint();
            TryAddPoint();
            BakeCurve();
        }
        
        private void TryAddPoint()
        {
            if (_positionRecording.Count <= 0)
            {
                _positionRecording.Add(new RecordPoint(transform));
                return;
            }

            while (true)
            {
                var diff = transform.position - _positionRecording[0].Position;
                if (diff.magnitude < positionRecordDistance)
                    return;

                _positionRecording.Insert(0, new RecordPoint
                {
                    Position = _positionRecording[0].Position + diff.normalized * positionRecordDistance
                });
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

        private void BakeCurve()
        {
            _curve.Clear();
            
            for (var i = 0; i * curvePointDistance <= curvePointTotalDistance; i++)
            {
                _curve.Add(GetPointAtDistance(i * curvePointDistance));
            }
        }

        public CurvePoint GetPointOnCurve(float distance)
        {
            var total = 0f;
            
            for (var i = 1; i < _curve.Count; i++)
            {
                var previousTotal = total;
                var start = _curve[i - 1];
                var end = _curve[i];
                total += Vector3.Distance(start.Position, end.Position);

                if (total < distance)
                    continue;

                var factor = (distance - previousTotal) / (total - previousTotal);
                return new CurvePoint
                {
                    Position = Vector3.Lerp(start.Position, end.Position, factor),
                    Rotation = Quaternion.Slerp(start.Rotation, end.Rotation, factor)
                };
            }

            return _curve[^1];
        }

        private Vector3 GetPointUp(int curve, float distance)
        {
            var time = 1 - distance / positionRecordingMaxDistance;
            var back = poses[curve].forwardCurve.Evaluate(time - 0.1f);
            var forth = poses[curve].forwardCurve.Evaluate(time + 0.1f);
            var sign = Sigmoid(forth - back);
            
            return Quaternion.AngleAxis(sign * 90, Vector3.right) * Vector3.forward;
        }

        private CurvePoint GetPointAtDistance(float distance)
        {
            var time = 1 - distance / positionRecordingMaxDistance;
            var factor = 2 * (_transitionTimer / poses[_currentPose].transitionTime) - 1 + transitionOrderCurve.Evaluate(time);
            
            var previousPoint = GetTransformAtDistance(_previousPose, distance);
            var currentPoint = GetTransformAtDistance(_currentPose, distance);

            return new CurvePoint
            {
                Position = Vector3.Lerp(currentPoint.Position, previousPoint.Position, factor),
                Rotation = Quaternion.Slerp(currentPoint.Rotation, previousPoint.Rotation, factor)
            };
        }

        private CurvePoint GetTransformAtDistance(int pose, float distance)
        {
            var forward = GetTransformedRecordPoint(pose, distance);
            var backward = GetTransformedRecordPoint(pose, distance + backwardEvaluationDistance);
            
            var up = GetPointUp(pose, distance);
            var normal = (forward.Position - backward.Position).normalized;
            
            return new CurvePoint
            {
                Position = forward.Position,
                Rotation = Quaternion.LookRotation(normal, up)
            };
        }

        private RecordPoint GetTransformedRecordPoint(int curve, float distance)
        {
            //distance 
            var time = 1 - distance / positionRecordingMaxDistance;
            var verticalOffset = poses[curve].verticalCurve.Evaluate(time);
            var forwardOffset = poses[curve].forwardCurve.Evaluate(time);
            forwardOffset -= RecordPoint.Distance(GetStartPoint(0), GetEndPoint(0));
            
            var remainder = (forwardOffset % positionRecordDistance) / positionRecordDistance;
            var index = Mathf.FloorToInt(forwardOffset / positionRecordDistance);

            if (index < 0)
                return GetStartPoint(0).SetVerticalOffset(verticalOffset);
            
            if (index >= _positionRecording.Count)
                return GetEndPoint(_positionRecording.Count - 1).SetVerticalOffset(verticalOffset);
            
            return RecordPoint.Lerp(GetStartPoint(index), GetEndPoint(index), remainder).SetVerticalOffset(verticalOffset);
        }

        private RecordPoint GetStartPoint(int point)
        {
            return point == 0 ? new RecordPoint(transform) : _positionRecording[point - 1];
        }

        private RecordPoint GetEndPoint(int point)
        {
            return _positionRecording[point];
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
        
        private static float _twoOverSqrtPi = 2.0f / Mathf.Sqrt(Mathf.PI);
        private const float Fraction = 11.0f / 123.0f;
        
        private float Sigmoid(float value)
        {
            return MathF.Tanh(_twoOverSqrtPi * (value + Fraction * Mathf.Pow(value, 3)));
        }

        private void OnDrawGizmosSelected()
        {
            if (_positionRecording == null)
                return;
            
            for (var i = 0; i < _positionRecording.Count; i++)
            {
                Gizmos.DrawLine(GetStartPoint(i).Position, GetEndPoint(i).Position);
            }

            Gizmos.color = Color.blue;
            for (var i = 0; i < _curve.Count - 1; i++)
            {
                Gizmos.DrawLine(_curve[i].Position, _curve[i + 1].Position);
            }
        }
    }
}