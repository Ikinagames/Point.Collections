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

#if UNITY_2020
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

using NUnit.Framework;
using Point.Collections.ResourceControl;
using System;
using UnityEngine;

namespace Point.Collections.Tests
{
    public class GeneralTests
    {
        [Test]
        public void ConstTypeGuidTest()
        {
            Guid guid = TypeHelper.TypeOf<AssetInfo>.Type.GUID;
            $"{guid} : {TypeHelper.TypeOf<AssetInfo>.Type.IsVisible} : {TypeHelper.TypeOf<AssetInfo>.Type.IsCOMObject}".ToLog();

            Type assetInfoType = Type.GetTypeFromCLSID(Guid.Parse("{b92cc9a9-b577-4759-b623-d794bd86d430}"));
            object assetInfoObj = Activator.CreateInstance(assetInfoType);
            Assert.AreEqual("AssetInfo", assetInfoObj.GetType().Name);
        }
    }
}

#endif