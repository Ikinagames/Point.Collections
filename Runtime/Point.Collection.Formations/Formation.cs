// Copyright 2022 Ikina Games
// Author : Seung Ha Kim (Syadeu)
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#if UNITY_COLLECTIONS
#endif
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

namespace Point.Collections.Formations
{
    public class Formation : IFormation, ITransform
    {
        private IFormation m_Parent;
        private List<IFormation> m_Children;
        private IFormationGroupProvider m_GroupProvider;
        private ITransform m_TransformationProvider;

        public string DisplayName { get; set; }
        public IFormationGroupProvider GroupProvider
        {
            get => m_GroupProvider;
            set
            {
                bool wasHasProvider = m_GroupProvider != null;
                if (value == null && m_GroupProvider != null)
                {
                    m_GroupProvider.children = null;
                }
                m_GroupProvider = value;
                if (m_GroupProvider != null)
                {
                    if (m_Children == null) m_Children = new List<IFormation>();
                    m_GroupProvider.children = m_Children;

                    if (wasHasProvider)
                    {
                        Refresh();
                    }
                }
            }
        }
        public ITransform TransformationProvider
        {
            get => m_TransformationProvider;
            set
            {
                m_TransformationProvider = value;
            }
        }

        public IFormation parent
        {
            get
            {
                if (TransformationProvider != null)
                {
                    return TransformationProvider.parent as IFormation;
                }
                return m_Parent;
            }
            set
            {
                //if (TransformationProvider != null)
                //{
                //    if (parent == null)
                //    {
                //        "remove parent".ToLog();
                //        RemoveFromHierarchy();
                //        return;
                //    }
                //    TransformationProvider.parent = value;

                //    int index = value.AddChildWithoutNotification(this);
                //    float3 localPosition = value.GroupProvider.CalculateOffset(index, this);

                //    if (TransformationProvider == null)
                //    {
                //        this._localPosition = localPosition;
                //        return;
                //    }

                //    float3 worldPosition = math.mul(value.localToWorld, new float4(localPosition, 1)).xyz;
                //    TransformationProvider.SetPosition(worldPosition);
                //    return;
                //}
                SetParent(value);
            }
        }
        public IReadOnlyList<IFormation> children => m_Children;

        public bool updateRotation { get; set; } = true;
        public bool alignRotationOnStop { get; set; } = true;

        public float currentSpeed { get; set; }
        public float targetSpeed { get; set; }
        public float3 currentVelocity { get; set; }

        #region ITransformation

        private float3 _localPosition;
        private quaternion _localRotation;
        private float3 _localScale;

        private float3 INTERNAL_Position
        {
            get
            {
                if (TransformationProvider == null)
                {
                    return _localPosition;
                }
                return TransformationProvider.position;
            }
            set
            {
                if (TransformationProvider == null)
                {
                    _localPosition = value;
                    return;
                }
                TransformationProvider.position = value;
            }
        }
        private float3 INTERNAL_LocalPosition
        {
            get
            {
                if (TransformationProvider == null)
                {
                    return _localPosition;
                }
                return TransformationProvider.localPosition;
            }
            set
            {
                if (TransformationProvider == null)
                {
                    _localPosition = value;
                    return;
                }
                TransformationProvider.localPosition = value;
            }
        }
        private quaternion INTERNAL_Rotation
        {
            get
            {
                if (TransformationProvider == null)
                {
                    return _localRotation;
                }
                return TransformationProvider.rotation;
            }
            set
            {
                if (TransformationProvider == null)
                {
                    _localRotation = value;
                    return;
                }
                TransformationProvider.rotation = value;
            }
        }
        private quaternion INTERNAL_LocalRotation
        {
            get
            {
                if (TransformationProvider == null)
                {
                    return _localRotation;
                }
                return TransformationProvider.localRotation;
            }
            set
            {
                if (TransformationProvider == null)
                {
                    _localRotation = value;
                    return;
                }
                TransformationProvider.localRotation = value;
            }
        }
        private float3 INTERNAL_LocalScale
        {
            get
            {
                if (TransformationProvider == null)
                {
                    return _localScale;
                }
                return TransformationProvider.localScale;
            }
            set
            {
                if (TransformationProvider == null)
                {
                    _localScale = value;
                    return;
                }
                TransformationProvider.localScale = value;
            }
        }

        ITransform ITransform.parent { get => parent; set => parent = value as IFormation; }

        public float4x4 localToWorld
        {
            get
            {
                if (TransformationProvider != null)
                {
                    return TransformationProvider.localToWorld;
                }
                return new float4x4(new float3x3(rotation), position);
            }
        }
        public float4x4 worldToLocal
        {
            get
            {
                if (TransformationProvider != null)
                {
                    return TransformationProvider.worldToLocal;
                }
                return math.inverse(localToWorld);
            }
        }

        public float3 position
        {
            get
            {
                if (parent == null) return INTERNAL_Position;

                return math.mul(parent.localToWorld, new float4(INTERNAL_Position, 1)).xyz;
            }
            set
            {
                if (parent == null)
                {
                    INTERNAL_Position = value;
                    return;
                }

                INTERNAL_Position = math.mul(parent.worldToLocal, new float4(value, 1)).xyz;
            }
        }
        public float3 localPosition { get => INTERNAL_LocalPosition; set => INTERNAL_LocalPosition = value; }
        public quaternion rotation
        {
            get
            {
                if (parent == null) return INTERNAL_Rotation;

                return math.mul(INTERNAL_Rotation, parent.rotation);
            }
            set
            {
                if (parent == null)
                {
                    INTERNAL_Rotation = value;
                    return;
                }

                INTERNAL_Rotation = math.mul(parent.rotation, value);
            }
        }
        public quaternion localRotation { get => INTERNAL_LocalRotation; set => INTERNAL_LocalRotation = value; }
        public float3 eulerAngles
        {
            get => rotation.Euler() * Math.Rad2Deg;
            set
            {
                float3 temp = value * Math.Deg2Rad;
                // .001f
                temp = math.round(temp * 1000) * 0.001f;
                rotation = quaternion.EulerZXY(temp);
            }
        }
        public float3 localEulerAngles
        {
            get => localRotation.Euler() * Math.Rad2Deg;
            set
            {
                float3 temp = value * Math.Deg2Rad;
                // .001f
                temp = math.round(temp * 1000) * 0.001f;
                localRotation = quaternion.EulerZXY(temp);
            }
        }
        public float3 lossyScale
        {
            get
            {
                if (parent == null)
                {
                    return localScale;
                }
                return parent.localScale / localScale;
            }
            set
            {
                if (parent == null)
                {
                    localScale = value;
                    return;
                }
                lossyScale = math.mul(parent.lossyScale, value);
            }
        }
        public float3 localScale { get => INTERNAL_LocalScale; set => INTERNAL_LocalScale = value; }

        public void SetPosition(float3 position)
        {
            if (TransformationProvider == null)
            {
                this.position = position;
                return;
            }
            TransformationProvider.SetPosition(position);
        }

        #endregion

        #region IEnumerable<IFormation>

        public IEnumerator<IFormation> GetEnumerator() => children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        public int AddChild(IFormation child)
        {
            Assert.IsNotNull(child);
            Assert.IsNotNull(GroupProvider,
                $"target parent\'s Group provider is null. set provider before declaring this formation as leader");

            child.parent = this;
            int index = m_Children.Count - 1;

            return index;
        }
        public bool RemoveChild(IFormation child)
        {
            if (m_Children == null) return false;

            bool result = m_Children.Remove(child);

            return result;
        }
        int IFormation.AddChildWithoutNotification(IFormation child)
        {
            if (m_Children == null) m_Children = new List<IFormation>();

            Assert.IsFalse(m_Children.Contains(child),
                $"this leader already has child {child.DisplayName}.");

            int index = m_Children.Count;
            m_Children.Add(child);
            return index;
        }
        public void ClearChildren()
        {
            if (m_Children == null) return;

            foreach (IFormation child in children)
            {
                child.parent = null;
            }
            m_Children.Clear();
        }
        public void SetParent(IFormation parent)
        {
            if (parent == null)
            {
                RemoveFromHierarchy();
                return;
            }

            ITransform targetParent;
            if (TransformationProvider == null)
            {
                m_Parent = parent;
                targetParent = parent;
            }
            else
            {
                targetParent = parent.TransformationProvider == null ? parent : parent.TransformationProvider;
                TransformationProvider.parent = targetParent;
            }

            int index = parent.AddChildWithoutNotification(this);
            float3 localPosition = parent.GroupProvider.CalculateOffset(index, this);
            localPosition = parent.GroupProvider.UpdatePosition(index, this, localPosition);

            if (TransformationProvider == null)
            {
                this._localPosition = localPosition;
                return;
            }

            float3 worldPosition = math.mul(targetParent.localToWorld, new float4(localPosition, 1)).xyz;
            TransformationProvider.SetPosition(worldPosition);
        }
        public void RemoveFromHierarchy()
        {
            if (m_Parent == null) return;

            m_Parent.RemoveChild(this);
            m_Parent = null;
        }

        void IFormation.UpdateCurrentSpeed(float accel)
        {
            currentSpeed = math.lerp(currentSpeed, targetSpeed, Time.deltaTime * accel);
        }

        const float c_TakeAsStopDistance = .01f * .01f;

        public bool Refresh()
        {
            bool finished;
            if (updateRotation)
            {
                finished = RefreshWithRotation();
            }
            else
            {
                finished = RefreshNormal();
            }

            return finished;
        }
        private bool RefreshNormal()
        {
            float3 localPosition, worldPosition, direction;
            float remainSqr;
            int finished = 0;
            for (int i = 0; i < m_Children.Count; i++)
            {
                localPosition = m_GroupProvider.CalculateOffset(i, m_Children[i]);
                direction = (localPosition - m_Children[i].localPosition);
                remainSqr = ((UnityEngine.Vector3)(direction)).sqrMagnitude;


                if (m_Children[i].TryGetUnityTransformProvider(out UnityTransformProvider unityTransform) &&
                    unityTransform.NavMeshAgent != null)
                {
                    float destSqr = (unityTransform.NavMeshAgent.destination - (Vector3)m_Children[i].position).sqrMagnitude;
                    if (remainSqr <= destSqr)
                    {
                        finished++;

                        if (unityTransform.NavMeshAgent.hasPath)
                        {
                            unityTransform.NavMeshAgent.ResetPath();
                        }

                        continue;
                    }

                    unityTransform.NavMeshAgent.updateRotation = updateRotation;
                    worldPosition = math.mul(localToWorld, new float4(localPosition, 1)).xyz;
                    unityTransform.SetPosition(worldPosition);
                    continue;
                }
                else
                {
                    if (remainSqr <= c_TakeAsStopDistance)
                    {
                        finished++;
                        continue;
                    }
                }

                bool isStopping = m_GroupProvider.StopDistance * m_GroupProvider.StopDistance > remainSqr;

                if (isStopping)
                {
                    m_Children[i].targetSpeed = 0;
                }
                else
                {
                    m_Children[i].targetSpeed = m_GroupProvider.Speed;
                    m_Children[i].currentVelocity = direction;
                }
                m_Children[i].UpdateCurrentSpeed(m_GroupProvider.Acceleration);

                localPosition = m_GroupProvider.UpdatePosition(i, m_Children[i], localPosition);

                // Set actual transformation data

                if (m_Children[i].TransformationProvider == null)
                {
                    m_Children[i].localPosition = localPosition;
                    continue;
                }

                worldPosition = math.mul(localToWorld, new float4(localPosition, 1)).xyz;
                m_Children[i].TransformationProvider.SetPosition(worldPosition);
            }

            return finished == m_Children.Count;
        }
        private bool RefreshWithRotation()
        {
            float3 localPosition, worldPosition, direction;
            float remainSqr;
            int finished = 0;
            for (int i = 0; i < m_Children.Count; i++)
            {
                localPosition = m_GroupProvider.CalculateOffset(i, m_Children[i]);
                direction = (localPosition - m_Children[i].localPosition);
                remainSqr = ((UnityEngine.Vector3)(direction)).sqrMagnitude;

                if (m_Children[i].TryGetUnityTransformProvider(out UnityTransformProvider unityTransform) &&
                    unityTransform.NavMeshAgent != null)
                {
                    float destSqr = (unityTransform.NavMeshAgent.destination - (Vector3)m_Children[i].position).sqrMagnitude;
                    if (remainSqr <= destSqr)
                    {
                        finished++;

                        if (unityTransform.NavMeshAgent.hasPath)
                        {
                            unityTransform.NavMeshAgent.ResetPath();
                        }

                        continue;
                    }

                    unityTransform.NavMeshAgent.updateRotation = updateRotation;
                    worldPosition = math.mul(localToWorld, new float4(localPosition, 1)).xyz;
                    unityTransform.SetPosition(worldPosition);
                    continue;
                }
                else
                {
                    if (remainSqr <= c_TakeAsStopDistance)
                    {
                        finished++;
                        continue;
                    }
                }

                bool isStopping = m_GroupProvider.StopDistance * m_GroupProvider.StopDistance > remainSqr;

                if (isStopping)
                {
                    m_Children[i].targetSpeed = 0;
                }
                else
                {
                    m_Children[i].targetSpeed = m_GroupProvider.Speed;
                    m_Children[i].currentVelocity = direction;
                }
                m_Children[i].UpdateCurrentSpeed(m_GroupProvider.Acceleration);

                localPosition = m_GroupProvider.UpdatePosition(i, m_Children[i], localPosition);

                // Set actual transformation data

                Quaternion lookRot = Quaternion.LookRotation(direction, math.mul(localRotation, math.up()));

                if (m_Children[i].TransformationProvider == null)
                {
                    lookRot =
                        Quaternion.Lerp(m_Children[i].rotation, lookRot, Time.deltaTime * 5);

                    m_Children[i].localPosition = localPosition;
                    m_Children[i].rotation = lookRot;
                    continue;
                }

                worldPosition = math.mul(localToWorld, new float4(localPosition, 1)).xyz;
                m_Children[i].TransformationProvider.SetPosition(worldPosition);

                lookRot =
                    Quaternion.Lerp(m_Children[i].TransformationProvider.localRotation, lookRot, Time.deltaTime * 5);
                m_Children[i].TransformationProvider.localRotation = lookRot;
            }

            return finished == m_Children.Count;
        }
    }
}
