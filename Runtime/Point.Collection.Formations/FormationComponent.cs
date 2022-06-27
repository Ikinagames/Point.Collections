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

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace Point.Collections.Formations
{
    public class FormationComponent : PointMonobehaviour
    {
        public enum ProvideOption
        {
            Row,
            Column,
        }

        [SerializeField] private bool m_InitializeOnEnable = true;
        [SerializeField] private string m_DisplayName;
        [SerializeField] private ProvideOption m_ProvideOption;

        [SerializeField] private bool m_UpdateRotation = true;
        [SerializeField] private float m_Speed = 2f;
        [SerializeField] private float m_Acceleration = 1f;

        [SerializeField] private FormationComponent m_Parent;
        [SerializeField] private List<FormationComponent> m_Children = new List<FormationComponent>();

        private IFormationGroupProvider m_GroupProvider;
        private Formation m_Formation;

        public IFormationGroupProvider GroupProvider
        {
            get
            {
                if (m_GroupProvider == null)
                {
                    IFormationGroupProvider result;
                    switch (m_ProvideOption)
                    {
                        case ProvideOption.Column:
                            result = new ColumnFormationGroup()
                            {
                                StopDistance = 1f,
                                Speed = m_Speed,
                                Acceleration = m_Acceleration,
                            };
                            break;
                        case ProvideOption.Row:
                        default:
                            result = new RowFormationGroup()
                            {
                                StopDistance = 1f,
                                Speed = m_Speed,
                                Acceleration = m_Acceleration,
                            };
                            break;
                    }
                    m_GroupProvider = result;
                }
                return m_GroupProvider;
            }
            set
            {
                Assert.IsNull(m_GroupProvider);
                m_GroupProvider = value;
            }
        }
        public Formation Formation
        {
            get
            {
                if (m_Formation == null)
                {
                    m_Formation = new Formation()
                    {
                        DisplayName = m_DisplayName,
                        TransformationProvider = new UnityTransformProvider(transform, GetComponent<NavMeshAgent>()),
                        updateRotation = m_UpdateRotation,
                    };
                }
                return m_Formation;
            }
            set
            {
                Assert.IsNull(m_Formation);

                m_Formation = value;
            }
        }

        private void OnEnable()
        {
            if (m_InitializeOnEnable)
            {
                Formation.GroupProvider = GroupProvider;
                for (int i = 0; i < m_Children.Count; i++)
                {
                    AddChild(m_Children[i]);
                }
            }
        }
        private void OnDisable()
        {
            for (int i = m_Children.Count - 1; i >= 0; i--)
            {
                if (m_Children[i] == null)
                {
                    m_Children.RemoveAt(i);
                }
            }

            if (m_Formation != null)
            {
                for (int i = 0; i < m_Children.Count; i++)
                {
                    m_Children[i].m_Parent = null;
                }
                Formation.ClearChildren();

                if (m_Children.Count > 0)
                {
                    var newLeader = m_Children[0];
                    //newLeader.enabled = true;

                    m_Children.RemoveAt(0);
                    for (int i = 0; i < m_Children.Count; i++)
                    {
                        newLeader.AddChild(m_Children[i]);
                    }
                }

                m_Children.Clear();
            }
        }
        private void Update()
        {
            if (m_Formation == null) return;

            m_Formation.Refresh();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (m_Formation == null) return;

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(m_Formation.position, .5f);
            Gizmos.DrawLine(m_Formation.position, m_Formation.position + (math.up() * 2));
            for (int i = 0; i < m_Formation.children?.Count; i++)
            {
                float3 pos = m_Formation.children[i].position;
                Gizmos.DrawLine(pos, pos + (math.up() * 2));
            }
        }
#endif

        public void AddChild(FormationComponent child)
        {
            Formation.AddChild(child.Formation);

            child.m_Parent = this;
            if (!m_Children.Contains(child))
            {
                m_Children.Add(child);
            }

            //child.enabled = false;
        }
        public void RemoveChild(FormationComponent child)
        {
            Formation.RemoveChild(child.Formation);

            if (m_Children.Remove(child))
            {
                child.m_Parent = null;
                //child.enabled = true;
            }
        }
        public void ClearChildren()
        {
            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].m_Parent = null;
                //m_Children[i].enabled = true;
            }
            Formation.ClearChildren();
            m_Children.Clear();
        }
    }
}
