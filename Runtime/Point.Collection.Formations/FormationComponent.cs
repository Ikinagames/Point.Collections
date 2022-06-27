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
using UnityEngine;
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
                            result = new ColumnFormationGroup();
                            break;
                        case ProvideOption.Row:
                        default:
                            result = new RowFormationGroup();
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
                        TransformationProvider = new UnityTransformProvider(transform),
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
                    //m_Children[i].transform.SetParent(transform);
                    Formation.AddChild(m_Children[i].Formation);
                }

                if (m_Parent != null)
                {
                    m_Parent.AddChild(this);
                }
            }
        }
        private void OnDisable()
        {
            
        }

        public void AddChild(FormationComponent child)
        {
            if (Formation.GroupProvider == null)
            {
                Formation.GroupProvider = GroupProvider;
            }

            //child.transform.SetParent(transform);
            Formation.AddChild(child.Formation);
            if (!m_Children.Contains(child))
            {
                m_Children.Add(child);
            }
        }
    }
}
