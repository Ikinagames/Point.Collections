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

using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;
using System;
using Unity.Burst;
using Point.Collections.Native;

namespace Point.Collections
{
    internal sealed class CollectionUtility : CLSSingleTone<CollectionUtility>
    {
        private static Unity.Mathematics.Random m_Random;

        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
#if POINT_COLLECTIONS_NATIVE
            NativeDebug.Initialize();
#endif
            m_Random = new Unity.Mathematics.Random();
            m_Random.InitState();

            Type[] types = TypeHelper.GetTypes((other) => TypeHelper.TypeOf<IStaticInitializer>.Type.IsAssignableFrom(other));
            for (int i = 0; i < types.Length; i++)
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(types[i].TypeHandle);
            }
        }

        public int CreateHashCode() => m_Random.NextInt(int.MinValue, int.MaxValue);

        public static TypeInfo GetTypeInfo(Type type)
        {
            if (!UnsafeUtility.IsUnmanaged(type))
            {
                Debug.LogError(
                    $"Could not resovle type of {TypeHelper.ToString(type)} is not ValueType.");

                return new TypeInfo(type, 0, 0, 0);
            }

            SharedStatic<TypeInfo> typeStatic = TypeStatic.GetValue(type);

            if (typeStatic.Data.Type == null)
            {
                typeStatic.Data
                    = new TypeInfo(type, UnsafeUtility.SizeOf(type), TypeHelper.AlignOf(type), Instance.CreateHashCode());
            }

            return typeStatic.Data;
        }
    }
}
