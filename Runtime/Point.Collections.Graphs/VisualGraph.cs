﻿// Copyright 2022 Ikina Games
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

#if ENABLE_NODEGRAPH &&  UNITY_2020_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

using GraphProcessor;
using System;

namespace Point.Collections.Graphs
{
    [Serializable]
    public class VisualGraph : BaseGraph
    {
        const string c_This = "This";

        //private ExposedParameter m_ThisParameter;

        protected override void OnEnable()
        {
            base.OnEnable();

            //m_ThisParameter = GetExposedParameter(c_This);
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            //m_ThisParameter = null;
        }
        public virtual void Execute(object caller, VisualGraphProcessor processor)
        {
            OnInitialize();

            OnExecute();

            //m_ThisParameter.value = caller;
            if (!SetParameterValue(c_This, caller))
            {
                PointHelper.LogError(Channel.Core,
                    $"Could not set this parameter of graph.");
            }

            //$"{GetParameterValue(c_This).ToString()}".ToLog();
            processor.Initialize(caller);
            {
                processor.UpdateComputeOrder();
                processor.Run();
            }
            processor.Reserve();
            SetParameterValue(c_This, null);
        }

        /// <summary>
        /// <see cref="VisualGraph"/> 가 실행되기전 함수입니다.
        /// </summary>
        protected virtual void OnInitialize() { }
        /// <summary>
        /// <see cref="VisualGraph"/> 가 실행될때 직전 함수입니다.
        /// </summary>
        protected virtual void OnExecute() { }
    }
}

#endif
