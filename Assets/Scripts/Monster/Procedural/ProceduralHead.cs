using System;
using System.Collections.Generic;
using System.Linq;
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
        
        [SerializeField] private float positionRecordDistance;
        [SerializeField] private float positionRecordingMaxDistance;
        [SerializeField] private AnimationCurve verticalCurve;
        [SerializeField] private AnimationCurve forwardCurve;
        private List<RecordPoint> _positionRecording;
        private float _totalDistance;

        private void Start()
        {
            _positionRecording = new List<RecordPoint>();
            _positionRecording.Add(new RecordPoint(transform));
        }

        private void Update()
        {
            TryRemovePoint();
            TryAddPoint();
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

        public RecordPoint GetPointAtDistance(float distance)
        {
            if (distance >= _totalDistance)
                return _positionRecording[^1];

            var verticalOffset = verticalCurve.Evaluate(1 - distance / positionRecordingMaxDistance);
            var forwardOffset = forwardCurve.Evaluate(1 - distance / positionRecordingMaxDistance);

            var total = 0f;
            
            for (var i = 0; i < _positionRecording.Count; i++)
            {
                var previousTotal = total;
                total += GetDistance(i);

                if (total < forwardOffset)
                    continue;

                var factor = (forwardOffset - previousTotal) / (total - previousTotal);
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