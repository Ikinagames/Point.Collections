// Copyright 2021 Ikina Games
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

using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    /// <summary>
    /// <see cref="PointSetupWizard"/> 에 표시되는 모든 메뉴들의 기본 클래스입니다.
    /// </summary>
    /// <remarks>
    /// 이것을 상속받으면 <see cref="PointSetupWizard"/> 에 새로운 메뉴를 추가 할 수 있습니다. 
    /// 따로 등록할 필요없습니다.
    /// </remarks>
    public abstract class SetupWizardMenuItem : IComparable<SetupWizardMenuItem>
    {
        private Action<GUIContent> showNotificationDelegate;
        private Action removeNotificationDelegate;

        /// <summary>
        /// 이 메뉴의 표시될 이름입니다.
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// 이 메뉴의 상단 탭 순서입니다.
        /// </summary>
        public abstract int Order { get; }

        /// <summary>
        /// 이 메뉴의 루트 <see cref="VisualElement"/> 입니다.
        /// </summary>
        public VisualElement Root { get; private set; }

        internal void Initialize(PointSetupWizard setupWizard)
        {
            Root = CreateVisualElement();

            showNotificationDelegate = setupWizard.ShowNotification;
            removeNotificationDelegate = setupWizard.RemoveNotification;
        }

        /// <summary>
        /// 이 메뉴가 처음 화면에 표시될 때 한번만 실행되는 메소드
        /// </summary>
        public virtual void OnVisible() { }
        /// <summary>
        /// 이 메뉴가 포커싱되었을 때 한번만 실행되는 메소드
        /// </summary>
        public virtual void OnFocus() { }
        /// <summary>
        /// 이 메뉴가 포커싱을 잃었을 때 한번만 실행되는 메소드
        /// </summary>
        public virtual void OnLostFocus() { }
        /// <summary>
        /// 이 메뉴를 보고 있을 때 매 프레임마다 실행되는 메소드
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// <see cref="PointSetupWizard"/> 에 알람을 보냅니다.
        /// </summary>
        /// <param name="content"></param>
        protected virtual void ShowNotification(GUIContent content) => showNotificationDelegate.Invoke(content);
        /// <summary>
        /// 현재 <see cref="PointSetupWizard"/> 의 알람을 제거합니다.
        /// </summary>
        protected virtual void RemoveNotification() => removeNotificationDelegate.Invoke();

        public abstract bool Predicate();
        /// <summary>
        /// <see cref="VisualElement"/> 로 메뉴를 그릴 수 있습니다.
        /// </summary>
        /// <remarks>
        /// <see langword="null"/> 이 아닐 경우, IMGUI 를 사용하지 않습니다.
        /// </remarks>
        /// <returns></returns>
        protected virtual VisualElement CreateVisualElement() => null;
        /// <summary>
        /// IMGUI
        /// </summary>
        public virtual void OnGUI() { }

        int IComparable<SetupWizardMenuItem>.CompareTo(SetupWizardMenuItem other)
        {
            if (Order < other.Order) return -1;
            else if (Order > other.Order) return 1;
            return 0;
        }
    }
}
