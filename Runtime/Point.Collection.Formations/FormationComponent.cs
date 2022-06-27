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

namespace Point.Collections.Formations
{
    public class FormationComponent : PointMonobehaviour
    {
        public enum ProvideOption
        {
            Row,
            Column,
        }

        [SerializeField] private string m_DisplayName;
        [SerializeField] private ProvideOption m_ProvideOption;

        [SerializeField] private List<FormationComponent> m_Children;

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
                    };
                }
                return m_Formation;
            }
        }

        public void AddChild(FormationComponent child)
        {
            if (Formation.GroupProvider == null)
            {
                Formation.GroupProvider = GroupProvider;
            }
            if (m_Children == null) m_Children = new List<FormationComponent>();

            Formation.AddChild(child.Formation);
            m_Children.Add(child);
        }
    }
}
