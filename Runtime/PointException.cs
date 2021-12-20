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
    public class PointException : Exception
    {
        public PointException(string message) : base(
            message: GetString(message)
            )
        {
        }
        public PointException(string message, Exception innerException) : base(
            message: GetString(message),
            innerException: innerException
            )
        {
        }

        protected static string GetString(string context, params string[] contexts)
        {
            const string 
                c_Space = " ",
                c_Base = "[Point]{0}";

            string sum = context;
            for (int i = 0; i < contexts.Length; i++)
            {
                sum += c_Space;
                sum += contexts[i];
            }

            return string.Format(c_Base, sum);
        }
    }
}
