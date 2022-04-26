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

#if UNITY_2020_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

using GraphProcessor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Point.Collections.Graphs
{
    public class VisualGraphLogicProcessor : VisualGraphProcessor
    {
        private EntryNode[] m_EntryNodes;

        public VisualGraphLogicProcessor(VisualGraph graph) : base(graph)
        {
        }
        public override void Run()
        {
            for (int i = 0; i < m_EntryNodes.Length; i++)
            {
                BaseNode current = m_EntryNodes[i];
                Execute(current);
            }
        }
        public override void UpdateComputeOrder()
        {
            m_EntryNodes = graph.nodes.Where(t => t is EntryNode).Select(t => (EntryNode)t).ToArray();
        }

        private void Execute(BaseNode current)
        {
            try
            {
                current.OnProcess();
            }
            catch (System.Exception ex)
            {
                PointHelper.LogError(Channel.Core, 
                    $"Fatal Error. From {caller.GetType().Name}");
                Debug.LogException(ex);

                return;
            }

            if (current is IConditionalNode conditional)
            {
                var iter = conditional.GetExecutableNodes();
                if (!iter.Any())
                {
                    //$"?? {current.name}".ToLogError();
                    return;
                }

                foreach (var outputNode in iter)
                {
                    Execute(outputNode);
                }
            }
            else
            {
                foreach (var outputNode in current.GetOutputNodes())
                {
                    Execute(outputNode);
                }
            }
        }
    }
    public abstract class VisualGraphProcessor : BaseGraphProcessor
    {
        private object m_Caller;

        protected object caller => m_Caller;

        protected VisualGraphProcessor(BaseGraph graph) : base(graph) { }

        internal void Initialize(object caller)
        {
            m_Caller = caller;

            OnInitialize(caller);
        }
        internal void Reserve()
        {
            OnReserve();

            m_Caller = null;
        }

        /// <summary>
        /// 프로세서가 실행되기전 가장 처음으로 실행되는 함수입니다.
        /// </summary>
        /// <param name="caller"></param>
        protected virtual void OnInitialize(object caller) { }
        /// <summary>
        /// 프로세서가 모두 동작하고, 맨 마지막에 실행되는 함수입니다.
        /// </summary>
        protected virtual void OnReserve() { }
    }
}

#endif
