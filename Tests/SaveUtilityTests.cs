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
        const int iteration = 5000;

        [Test]
        public void PlayerPrefsTest()
        {
            for (int i = 0; i < iteration; i++)
            {
                PlayerPrefs.SetString("stringTest", "none");
                PlayerPrefs.SetInt("intTest", 123);
                PlayerPrefs.SetFloat("floatTest", 123f);
                PlayerPrefs.Save();
            }

            PlayerPrefs.GetString("stringTest");
        }
        [Test]
        public void SaveDataTest()
        {
            var strTest = new Identifier("stringTest");
            var intTest = new Identifier("intTest");
            var floatTest = new Identifier("floatTest");
            SaveData data = new SaveData("main", new KeyValuePair<Identifier, System.Type>[]
            {
                new KeyValuePair<Identifier, System.Type>(strTest, TypeHelper.TypeOf<FixedString128Bytes>.Type),
                new KeyValuePair<Identifier, System.Type>(intTest, TypeHelper.TypeOf<int>.Type),
                new KeyValuePair<Identifier, System.Type>(floatTest, TypeHelper.TypeOf<float>.Type)
            });
            data.Load();

            for (int i = 0; i < iteration; i++)
            {
                data.SetValue(strTest, "none");
                data.SetValue(intTest, 123);
                data.SetValue(floatTest, 123f);
                data.Save();
            }
        }
        [Test]
        public void Comparson()
        {
            long playerPrefTime, saveDataTime;
            HighPrecisionTimer timer = new HighPrecisionTimer();

            timer.Begin();

            for (int i = 0; i < iteration; i++)
            {
                PlayerPrefs.SetString("stringTest", "none");
                PlayerPrefs.SetInt("intTest", 123);
                PlayerPrefs.SetFloat("floatTest", 123f);
                PlayerPrefs.Save();
            }

            PlayerPrefs.GetString("stringTest");

            timer.End();
            playerPrefTime = timer.TimeStamp;
            Debug.Log($"playerPrefs {iteration} : {timer.TimeStamp}");
            timer.Reset();

            timer.Begin();

            var strTest = new Identifier("stringTest");
            var intTest = new Identifier("intTest");
            var floatTest = new Identifier("floatTest");
            SaveData data = new SaveData("main", new KeyValuePair<Identifier, System.Type>[]
            {
                new KeyValuePair<Identifier, System.Type>(strTest, TypeHelper.TypeOf<FixedString128Bytes>.Type),
                new KeyValuePair<Identifier, System.Type>(intTest, TypeHelper.TypeOf<int>.Type),
                new KeyValuePair<Identifier, System.Type>(floatTest, TypeHelper.TypeOf<float>.Type)
            });
            data.Load();

            for (int i = 0; i < iteration; i++)
            {
                data.SetValue(strTest, "none");
                data.SetValue(intTest, 123);
                data.SetValue(floatTest, 123f);
                data.Save();
            }

            timer.End();
            saveDataTime = timer.TimeStamp;
            Debug.Log($"saveData {iteration} : {timer.TimeStamp}");
            timer.Reset();

            if (playerPrefTime < saveDataTime)
            {
                Debug.Log($"winner : playerPref, {playerPrefTime - saveDataTime}");
            }
            else if (playerPrefTime > saveDataTime)
            {
                Debug.Log($"winner : saveData, {saveDataTime - playerPrefTime}");
            }
        }
    }
}

#endif