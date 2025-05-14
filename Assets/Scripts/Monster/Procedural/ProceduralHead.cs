using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

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
        [SerializeField] private List<AnimationPose> poses;
        [SerializeField] private AnimationCurve transitionOrderCurve;
        private Dictionary<string, int> _posesDict;
        private List<RecordPoint> _positionRecording;
        private List<Vector3> _curve;
        private float _totalDistance;
        
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
            _curve = new List<Vector3>();
            BakeCurve();
        }

        private void Update()
        {
            _transitionTimer -= Time.deltaTime;
            TryRemovePoint();
            TryAddPoint();
            BakeCurve();
            _totalDistance = GetTotalDistance();
        }
        
        private void TryAddPoint()
        {
            if (_positionRecording.Count <= 0)
            {
                _positionRecording.Add(new RecordPoint(transform));
                return;
            }

            var diff = transform.position - _positionRecording[0].Position;
            if (diff.magnitude < positionRecordDistance)
                return;
            
            _positionRecording.Insert(0, new RecordPoint(transform));
        }

        private void TryRemovePoint()
        {
            if (GetDistanceAtPoint(_positionRecording.Count - 1) < positionRecordingMaxDistance)
                return;
            
            _positionRecording.RemoveAt(_positionRecording.Count - 1);
        }

        private void BakeCurve()
        {
            _curve.Clear();
            for (var i = 0f; i <= positionRecordingMaxDistance; i += curvePointDistance)
            {
                _curve.Add(GetPointAtDistance(i).Position);
            }
        }

        private float GetDistanceAtPoint(int index)
        {
            var distance = Vector3.Distance(transform.position, _positionRecording[0].Position);
            
            for (var i = 0; i < index; i++)
            {
                var diff = _positionRecording[i].Position - _positionRecording[i + 1].Position;
                distance += diff.magnitude;
            }

            return distance;
        }

        public Vector3 GetPointOnCurve(float distance)
        {
            var total = 0f;
            
            for (var i = 1; i < _curve.Count; i++)
            {
                var previousTotal = total;
                var start = _curve[i - 1];
                var end = _curve[i];
                total += Vector3.Distance(start, end);

                if (total < distance)
                    continue;

                var factor = (distance - previousTotal) / (total - previousTotal);
                return Vector3.Lerp(start, end, factor);
            }

            return _curve[^1];
        }

        public RecordPoint GetPointAtDistance(float distance)
        {
            if (distance >= _totalDistance)
                return _positionRecording[^1];

            var time = 1 - distance / positionRecordingMaxDistance;
            var factor = 2 * (_transitionTimer / poses[_currentPose].transitionTime) - 1 + transitionOrderCurve.Evaluate(time);
            var verticalOffset = Lerpaluate(poses[_currentPose].verticalCurve, poses[_previousPose].verticalCurve, time, factor);
            var forwardOffset = Lerpaluate(poses[_currentPose].forwardCurve, poses[_previousPose].forwardCurve, time, factor);

            var total = 0f;
            
            for (var i = 0; i < _positionRecording.Count; i++)
            {
                var previousTotal = total;
                total += GetDistance(i);

                if (total < forwardOffset)
                    continue;

                factor = (forwardOffset - previousTotal) / (total - previousTotal);
                return RecordPoint.Lerp(GetStartPoint(i), GetEndPoint(i), factor).SetVerticalOffset(verticalOffset);
            }
            
            return _positionRecording[^1];
        }

        private float GetTotalDistance()
        {
            if (_positionRecording.Count < 2)
                return 0;
            
            var total = 0f;

            for (var i = 0; i < _positionRecording.Count; i++)
            {
                total += GetDistance(i);
            }

            return total;
        }
        
        private float GetDistance(int point)
        {
            return RecordPoint.Distance(GetStartPoint(point), GetEndPoint(point));
        }

        private RecordPoint GetStartPoint(int point)
        {
            return point == 0 ? new RecordPoint(transform) : _positionRecording[point - 1];
        }

        private RecordPoint GetEndPoint(int point)
        {
            return _positionRecording[point];
        }

        private float Lerpaluate(AnimationCurve a, AnimationCurve b, float time, float factor)
        {
            return Mathf.Lerp(a.Evaluate(time), b.Evaluate(time), factor);
        }

        public void SetPose(string name)
        {
            if (!_posesDict.ContainsKey(name))
            {
                Debug.LogError($"No pose named {name} exists!");
                return;
            }

            if (_currentPose == _posesDict[name])
                return;

            _previousPose = _currentPose;
            _currentPose = _posesDict[name];
            _transitionTimer = poses[_currentPose].transitionTime;
        }

        private void OnDrawGizmosSelected()
        {
            if (_positionRecording == null)
                return;
            
            for (var i = 0; i < _positionRecording.Count; i++)
            {
                Gizmos.DrawLine(GetStartPoint(i).Position, GetEndPoint(i).Position);
            }
        }
    }
}