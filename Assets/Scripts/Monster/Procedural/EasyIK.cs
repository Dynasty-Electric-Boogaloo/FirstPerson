﻿using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Monster.Procedural
{
    public class EasyIK : MonoBehaviour
    {
        [Header("IK properties")]
        public int numberOfJoints = 2;
        public Transform ikTarget;
        public int iterations = 10;
        public float tolerance = 0.05f;
        private Transform[] _jointTransforms;
        private Vector3 _startPosition;
        private Vector3[] _jointPositions;
        private float[] _boneLength;
        private float _jointChainLength;
        private float _distanceToTarget;
        private Quaternion[] _startRotation;
        private Vector3[] _jointStartDirection;
        private Quaternion _ikTargetStartRot;
        private Quaternion _lastJointStartRot;

        [Header("Pole target (3 joint chain)")]
        public Transform poleTarget;

        [Header("Debug")]
        public bool debugJoints = true;
        public bool localRotationAxis = false;

        // Remove this if you need bigger gizmos:
        [Range(0.0f, 1.0f)]
        public float gizmoSize = 0.05f;
        public bool poleDirection = false;
        public bool poleRotationAxis = false;

        private void Awake()
        {
            // Let's set some properties
            _jointChainLength = 0;
            _jointTransforms = new Transform[numberOfJoints];
            _jointPositions = new Vector3[numberOfJoints];
            _boneLength = new float[numberOfJoints];
            _jointStartDirection = new Vector3[numberOfJoints];
            _startRotation = new Quaternion[numberOfJoints];
            _ikTargetStartRot = ikTarget.rotation;

            var current = transform;

            // For each bone calculate and store the lenght of the bone
            for (var i = 0; i < _jointTransforms.Length; i += 1)
            {
                _jointTransforms[i] = current;
                // If the bones lenght equals the max lenght, we are on the last joint in the chain
                if (i == _jointTransforms.Length - 1)
                {
                    _lastJointStartRot = current.rotation;
                    return;
                }
                // Store length and add the sum of the bone lengths
                else
                {
                    _boneLength[i] = Vector3.Distance(current.position, current.GetChild(0).position);
                    _jointChainLength += _boneLength[i];

                    _jointStartDirection[i] = current.GetChild(0).position - current.position;
                    _startRotation[i] = current.rotation;
                }
                // Move the iteration to next joint in the chain
                current = current.GetChild(0);
            }
        }

        private void PoleConstraint()
        {
            if (poleTarget != null && numberOfJoints < 4)
            {
                // Get the limb axis direction
                var limbAxis = (_jointPositions[2] - _jointPositions[0]).normalized;

                // Get the direction from the root joint to the pole target and mid joint
                var poleDirection = (poleTarget.position - _jointPositions[0]).normalized;
                var boneDirection = (_jointPositions[1] - _jointPositions[0]).normalized;
            
                // Ortho-normalize the vectors
                Vector3.OrthoNormalize(ref limbAxis, ref poleDirection);
                Vector3.OrthoNormalize(ref limbAxis, ref boneDirection);

                // Calculate the angle between the boneDirection vector and poleDirection vector
                var angle = Quaternion.FromToRotation(boneDirection, poleDirection);

                // Rotate the middle bone using the angle
                _jointPositions[1] = angle * (_jointPositions[1] - _jointPositions[0]) + _jointPositions[0];
            }
        }

        private void Backward()
        {
            // Iterate through every position in the list until we reach the start of the chain
            for (var i = _jointPositions.Length - 1; i >= 0; i -= 1)
            {
                // The last bone position should have the same position as the ikTarget
                if (i == _jointPositions.Length - 1)
                {
                    _jointPositions[i] = ikTarget.transform.position;
                }
                else
                {
                    _jointPositions[i] = _jointPositions[i + 1] + (_jointPositions[i] - _jointPositions[i + 1]).normalized * _boneLength[i];
                }
            }
        }

        private void Forward()
        {
            // Iterate through every position in the list until we reach the end of the chain
            for (var i = 0; i < _jointPositions.Length; i += 1)
            {
                // The first bone position should have the same position as the startPosition
                if (i == 0)
                {
                    _jointPositions[i] = _startPosition;
                }
                else
                {
                    _jointPositions[i] = _jointPositions[i - 1] + (_jointPositions[i] - _jointPositions[i - 1]).normalized * _boneLength[i - 1];
                }
            }
        }

        private void SolveIK()
        {
            // Get the jointPositions from the joints
            for (var i = 0; i < _jointTransforms.Length; i += 1)
            {
                _jointPositions[i] = _jointTransforms[i].position;
            }
            // Distance from the root to the ikTarget
            _distanceToTarget = Vector3.Distance(_jointPositions[0], ikTarget.position);

            // IF THE TARGET IS NOT REACHABLE
            if (_distanceToTarget > _jointChainLength)
            {
                // Direction from root to ikTarget
                var direction = ikTarget.position - _jointPositions[0];

                // Get the jointPositions
                for (var i = 1; i < _jointPositions.Length; i += 1)
                {
                    _jointPositions[i] = _jointPositions[i - 1] + direction.normalized * _boneLength[i - 1];
                }
            }
            // IF THE TARGET IS REACHABLE
            else
            {
                // Get the distance from the leaf bone to the ikTarget
                var distToTarget = Vector3.Distance(_jointPositions[_jointPositions.Length - 1], ikTarget.position);
                float counter = 0;
                // While the distance to target is greater than the tolerance let's iterate until we are close enough
                while (distToTarget > tolerance)
                {
                    _startPosition = _jointPositions[0];
                    Backward();
                    Forward();
                    counter += 1;
                    // After x iterations break the loop to avoid an infinite loop
                    if (counter > iterations)
                    {
                        break;
                    }
                }
            }
            // Apply the pole constraint
            PoleConstraint();

            // Apply the jointPositions and rotations to the joints
            for (var i = 0; i < _jointPositions.Length - 1; i += 1)
            {
                _jointTransforms[i].position = _jointPositions[i];
                var targetRotation = Quaternion.FromToRotation(_jointStartDirection[i], _jointPositions[i + 1] - _jointPositions[i]);
                _jointTransforms[i].rotation = targetRotation * _startRotation[i];
            }
            // Let's constrain the rotation of the last joint to the IK target and maintain the offset.
            var offset = _lastJointStartRot * Quaternion.Inverse(_ikTargetStartRot);
            _jointTransforms.Last().rotation = ikTarget.rotation * offset;
        }

        private void Update()
        {
            SolveIK();
        }

        public float GetChainLength()
        {
            return _jointChainLength;
        }

        // Visual debugging
        private void OnDrawGizmos()
        {   
            if (debugJoints == true)
            {   
                var current = transform;
                var child = transform.GetChild(0);

                for (var i = 0; i < numberOfJoints; i += 1)
                {
                    if (i == numberOfJoints - 2)
                    {
                        var length = Vector3.Distance(current.position, child.position);
                        DrawWireCapsule(current.position + (child.position - current.position).normalized * length / 2, Quaternion.FromToRotation(Vector3.up, (child.position - current.position).normalized), gizmoSize, length, Color.cyan);
                        break;
                    }
                    else
                    {
                        var length = Vector3.Distance(current.position, child.position);
                        DrawWireCapsule(current.position + (child.position - current.position).normalized * length / 2, Quaternion.FromToRotation(Vector3.up, (child.position - current.position).normalized), gizmoSize, length, Color.cyan);
                        current = current.GetChild(0);
                        child = current.GetChild(0);
                    }
                }
            }

            if (localRotationAxis == true)
            {    
                var current = transform;
                for (var i = 0; i < numberOfJoints; i += 1)
                {
                    if (i == numberOfJoints - 1)
                    {
                        DrawHandle(current);
                    }
                    else
                    {
                        DrawHandle(current);
                        current = current.GetChild(0);
                    }
                }
            }

            var start = transform;
            var mid = start.GetChild(0);
            var end = mid.GetChild(0);

            if (poleRotationAxis == true && poleTarget != null && numberOfJoints < 4)
            {    
                Handles.color = Color.white;
                Handles.DrawLine(start.position, end.position);
            }

            if (poleDirection == true && poleTarget != null && numberOfJoints < 4)
            {    
                Handles.color = Color.grey;
                Handles.DrawLine(start.position, poleTarget.position);
                Handles.DrawLine(end.position, poleTarget.position);
            }

        }

        private void DrawHandle(Transform debugJoint)
        {
            Handles.color = Handles.xAxisColor;
            Handles.ArrowHandleCap(0, debugJoint.position, debugJoint.rotation * Quaternion.LookRotation(Vector3.right), gizmoSize, EventType.Repaint);
                
            Handles.color = Handles.yAxisColor;
            Handles.ArrowHandleCap(0, debugJoint.position, debugJoint.rotation * Quaternion.LookRotation(Vector3.up), gizmoSize, EventType.Repaint);

            Handles.color = Handles.zAxisColor;
            Handles.ArrowHandleCap(0, debugJoint.position, debugJoint.rotation * Quaternion.LookRotation(Vector3.forward), gizmoSize, EventType.Repaint);
        }

        public static void DrawWireCapsule(Vector3 pos, Quaternion rot, float radius, float height, Color color = default(Color))
        {
            Handles.color = color;
            var angleMatrix = Matrix4x4.TRS(pos, rot, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix))
            {
                var pointOffset = (height - (radius * 2)) / 2;

                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, radius);
                Handles.DrawLine(new Vector3(0, pointOffset, -radius), new Vector3(0, -pointOffset, -radius));
                Handles.DrawLine(new Vector3(0, pointOffset, radius), new Vector3(0, -pointOffset, radius));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, radius);

                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, radius);
                Handles.DrawLine(new Vector3(-radius, pointOffset, 0), new Vector3(-radius, -pointOffset, 0));
                Handles.DrawLine(new Vector3(radius, pointOffset, 0), new Vector3(radius, -pointOffset, 0));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, radius);

                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, radius);
            }
        }
    }
}