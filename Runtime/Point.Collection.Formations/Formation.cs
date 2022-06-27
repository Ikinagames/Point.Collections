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
using UnityEngine.Assertions;

namespace Point.Collections.Formations
{
    public class Formation : IFormation, ITransformation
    {
        private IFormation m_Parent;
        private List<IFormation> m_Children;
        private IFormationGroupProvider m_GroupProvider;
        private ITransformation m_TransformationProvider;

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
        public ITransformation TransformationProvider
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
                if (TransformationProvider != null)
                {
                    TransformationProvider.parent = value;
                    return;
                }
                SetParent(value);
            }
        }
        public IReadOnlyList<IFormation> children => m_Children;

        #region ITransformation

        private float3 _localPosition;
        private quaternion _localRotation;
        private float3 _localScale;

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

        ITransformation ITransformation.parent { get => parent; set => parent = value as IFormation; }

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
                return math.fastinverse(worldToLocal);
            }
        }

        public float3 position
        {
            get
            {
                if (parent == null) return INTERNAL_LocalPosition;

                return math.mul(parent.localToWorld, new float4(INTERNAL_LocalPosition, 1)).xyz;
            }
            set
            {
                if (parent == null)
                {
                    INTERNAL_LocalPosition = value;
                    return;
                }

                INTERNAL_LocalPosition = math.mul(parent.worldToLocal, new float4(value, 1)).xyz;
            }
        }
        public float3 localPosition { get => INTERNAL_LocalPosition; set => INTERNAL_LocalPosition = value; }
        public quaternion rotation
        {
            get
            {
                if (parent == null) return INTERNAL_LocalRotation;

                return math.mul(INTERNAL_LocalRotation, parent.rotation);
            }
            set
            {
                if (parent == null)
                {
                    INTERNAL_LocalRotation = value;
                    return;
                }

                INTERNAL_LocalRotation = math.mul(parent.rotation, value);
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
                return parent.lossyScale / localScale;
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
            m_Parent = parent;

            int index = m_Parent.AddChildWithoutNotification(this);
            float3 localPosition = m_Parent.GroupProvider.CalculateOffset(index, this);

            if (TransformationProvider == null)
            {
                this._localPosition = localPosition;
                return;
            }
            
            float3 worldPosition = math.mul(m_Parent.localToWorld, new float4(localPosition, 1)).xyz;
            TransformationProvider.SetPosition(worldPosition);
        }
        public void RemoveFromHierarchy()
        {
            if (m_Parent == null) return;

            m_Parent.RemoveChild(this);
            m_Parent = null;
        }

        public void Refresh()
        {
            float3 localPosition;
            for (int i = 0; i < m_Children.Count; i++)
            {
                localPosition = m_GroupProvider.CalculateOffset(i, m_Children[i]);

                if (m_Children[i].TransformationProvider == null)
                {
                    m_Children[i].localPosition = localPosition;
                    continue;
                }

                float3 worldPosition = math.mul(localToWorld, new float4(localPosition, 1)).xyz;
                m_Children[i].TransformationProvider.SetPosition(worldPosition);
            }
        }
    }
}
