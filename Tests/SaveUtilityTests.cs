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

using NUnit.Framework;
using Point.Collections.IO;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Point.Collections.Tests
{
    public sealed class SaveUtilityTests
    {
        [Test]
        public void Comparson()
        {
            const int iteration = 5000;

            long playerPrefTime, saveDataTime;
            HighPrecisionTimer timer = new HighPrecisionTimer();

            timer.Begin();

            for (int i = 0; i < iteration; i++)
            {
                PlayerPrefs.SetString("stringTest", "none");
                PlayerPrefs.Save();
            }

            PlayerPrefs.GetString("stringTest");

            timer.End();
            playerPrefTime = timer.TimeStamp;
            Debug.Log($"playerPrefs {iteration} : {timer.TimeStamp}");
            timer.Reset();

            timer.Begin();

            var id = new Identifier("stringTest");
            SaveData data = new SaveData("main", new KeyValuePair<Identifier, System.Type>[]
            {
                new KeyValuePair<Identifier, System.Type>(id, TypeHelper.TypeOf<FixedString128Bytes>.Type)
            });
            data.Load();

            for (int i = 0; i < iteration; i++)
            {
                data.SetValue(id, "none");
                data.Save();
            }

            timer.End();
            saveDataTime = timer.TimeStamp;
            Debug.Log($"saveData {iteration} : {timer.TimeStamp}");
            timer.Reset();

            if (playerPrefTime < saveDataTime)
            {
                Debug.Log($"winner : playerPref");
            }
            else if (playerPrefTime > saveDataTime)
            {
                Debug.Log($"winner : saveData");
            }
        }
    }
}

#endif