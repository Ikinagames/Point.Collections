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

namespace Point.Collections
{
    public static class ExtensionMethods
    {
        public static TypeInfo ToTypeInfo(this Type type) => TypeHelper.ToTypeInfo(type);

        #region IConvertible

        public static int ToInt32<T>(this T t) where T : struct, IConvertible => t.ToInt32(System.Globalization.CultureInfo.InvariantCulture);
        public static double ToDouble<T>(this T t) where T : struct, IConvertible => t.ToDouble(System.Globalization.CultureInfo.InvariantCulture);
        public static float ToSingle<T>(this T t) where T : struct, IConvertible => t.ToSingle(System.Globalization.CultureInfo.InvariantCulture);
        public static long ToInt64<T>(this T t) where T : struct, IConvertible => t.ToInt64(System.Globalization.CultureInfo.InvariantCulture);
        public static string ToString<T>(this T t) where T : struct, IConvertible => t.ToString(System.Globalization.CultureInfo.InvariantCulture);

        #endregion
    }
}
