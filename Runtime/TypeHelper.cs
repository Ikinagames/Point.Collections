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
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
#if UNITY_BURST
using Unity.Burst;
#endif
#if UNITY_MATHEMATICS
using Unity.Mathematics;
#else
using math = Point.Collections.Math;
#endif
#else
#define POINT_COLLECTIONS_NATIVE
using math = Point.Collections.Math;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Point.Collections
{
    public sealed class TypeHelper
    {
        /// <summary>
        /// <seealso cref="System.Type"/>(<typeparamref name="T"/>) 에 관한 각종 유틸 메소드를 담은 Helper 입니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public sealed class TypeOf<T>
        {
            public static readonly Type Type = typeof(T);
            public static readonly string Name = Type.Name;
            public static readonly string FullName = Type.FullName;
            public static readonly bool IsAbstract = Type.IsAbstract;
            public static readonly bool IsArray = Type.IsArray;

            public static TypeInfo TypeInfo => ToTypeInfo(Type);

            private static Type[] s_Interfaces = null;
            public static Type[] Interfaces
            {
                get
                {
                    if (s_Interfaces == null) s_Interfaces = Type.GetInterfaces();
                    return s_Interfaces;
                }
            }

            private static MemberInfo[] s_Members = null;
            public static MemberInfo[] Members
            {
                get
                {
                    if (s_Members == null) s_Members = Type.GetMembers((BindingFlags)~0);
                    return s_Members;
                }
            }

            private static MethodInfo[] s_Methods = null;
            public static MethodInfo[] Methods
            {
                get
                {
                    if (s_Methods == null) s_Methods = Type.GetMethods((BindingFlags)~0);
                    return s_Methods;
                }
            }

            private static int s_Align = -1;
            public static int Align
            {
                get
                {
                    if (s_Align < 0)
                    {
                        s_Align = AlignOf(Type);
                    }
                    return s_Align;
                }
            }

            public static ConstructorInfo GetConstructorInfo(params Type[] args)
                => TypeHelper.GetConstructorInfo(Type, args);
            public static FieldInfo GetFieldInfo(string name, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) => TypeHelper.GetFieldInfo(Type, name, bindingFlags);
            public static PropertyInfo GetPropertyInfo(string name, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) => TypeHelper.GetPropertyInfo(Type, name, bindingFlags);

            /// <inheritdoc cref="TypeHelper.IsZeroSizeStruct(Type)"/>>
            public static bool IsZeroSizeStruct()
            {
                return Type.IsValueType && !Type.IsPrimitive &&
                    Type.GetFields((BindingFlags)0x34).All(fi => TypeHelper.IsZeroSizeStruct(fi.FieldType));
            }

            private static string s_ToString = string.Empty;
            public static new string ToString()
            {
                if (string.IsNullOrEmpty(s_ToString))
                {
                    s_ToString = TypeHelper.ToString(Type);
                }
                return s_ToString;
            }
        }
        /// <summary>
        /// <see cref="System.Enum"/>(<typeparamref name="T"/>) 에 관한 유틸 메소드를 담은 Helper 입니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public sealed class Enum<T> where T : struct, IConvertible
        {
            public static readonly bool IsFlag = TypeOf<T>.Type.GetCustomAttribute<FlagsAttribute>() != null;
            public static readonly int Length = ((T[])Enum.GetValues(TypeOf<T>.Type)).Length;

            public static readonly string[] Names = Enum.GetNames(TypeOf<T>.Type);
            public static readonly T[] Values = ((T[])Enum.GetValues(TypeOf<T>.Type)).ToArray();

            public static string ToString(T enumValue)
            {
                long target = Convert.ToInt64(enumValue);
                if (IsFlag)
                {
                    string temp = string.Empty;
                    for (int i = 0; i < Values.Length; i++)
                    {
                        long val = Convert.ToInt64(Values[i]);
                        if (val == 0 && !target.Equals(0)) continue;
                        if ((target & val) == val)
                        {
                            if (!string.IsNullOrEmpty(temp)) temp += ", ";
                            temp += Names[i];
                        }
                    }

                    return temp;
                }
                else
                {
                    for (int i = 0; i < Values.Length; i++)
                    {
                        if (target.Equals(Convert.ToInt64(Values[i]))) return Names[i];
                    }
                }

                throw new ArgumentException(nameof(enumValue));
            }
            public static T ToEnum(in string value)
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct AlignOfHelper<T>
        {
            public byte dummy;
            public T data;
        }

        private static readonly Assembly[] s_Assemblies = AppDomain.CurrentDomain.GetAssemblies();
        private static readonly Type[] s_AllTypes 
            = s_Assemblies
                .Where(a => !a.IsDynamic)
#if !UNITY_EDITOR
                .Where(x => x.GetCustomAttribute<InternalIgnoreTypeAttribute>() == null)
#else
#endif
                .SelectMany(a => GetLoadableTypes(a))
                .ToArray();

        private static readonly Type GenericListInterface = typeof(IList<>);
        private static readonly Type GenericCollectionInterface = typeof(ICollection<>);

        /// <summary>
        /// 현재 프로젝트의 모든 <see cref="System.Type"/> 에서 <paramref name="predictate"/> 조건으로 찾아서 반환합니다.
        /// </summary>
        /// <param name="predictate"></param>
        /// <returns></returns>
        public static Type[] GetTypes(Func<Type, bool> predictate) => s_AllTypes.Where(predictate).ToArray();
        public static IEnumerable<Type> GetTypesIter(Func<Type, bool> predictate) => s_AllTypes.Where(predictate);
        public static ConstructorInfo GetConstructorInfo(Type t, params Type[] args)
        {
            return t.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null, CallingConventions.HasThis, args, null);
        }
        public static FieldInfo GetFieldInfo(Type type, string name, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            return type.GetField(name, bindingFlags);
        }
        public static FieldInfo GetFieldInfoRecursive(Type type, string name, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var result = type.GetField(name, bindingFlags);
            if (result == null && type.BaseType != null)
            {
                return GetFieldInfoRecursive(type.BaseType, name, bindingFlags);
            }
            return result;
        }
        public static PropertyInfo GetPropertyInfo(Type type, string name, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            return type.GetProperty(name, bindingFlags);
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            // TODO: Argument validation
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        public static int SizeOf(Type type)
        {
#if UNITYENGINE
            return UnsafeUtility.SizeOf(type);
#else
            return Marshal.SizeOf(type);
#endif
        }
        public static int SizeOf<T>() where T : unmanaged
        {
#if UNITYENGINE
            return UnsafeUtility.SizeOf<T>();
#else
            return Marshal.SizeOf(TypeOf<T>.Type);
#endif
        }
        /// <summary>
        /// <paramref name="t"/> 의 Alignment 를 반환합니다.
        /// </summary>
        /// <remarks>
        /// 만약 <paramref name="t"/> 가 ReferenceType 이라면 반환 값은 무조건 0 입니다.
        /// </remarks>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int AlignOf(Type t)
        {
            if (!IsUnmanaged(t))
            {
                return 0;
            }
            
            Type temp = typeof(AlignOfHelper<>).MakeGenericType(t);
#if UNITYENGINE
            return UnsafeUtility.SizeOf(temp) - UnsafeUtility.SizeOf(t);
#else
            return Marshal.SizeOf(temp) - Marshal.SizeOf(t);
#endif
        }
        public static int AlignOf<T>()
        {
            if (!IsUnmanaged(TypeOf<T>.Type))
            {
                return 0;
            }

            Type temp = TypeOf<AlignOfHelper<T>>.Type;
#if UNITYENGINE
            return UnsafeUtility.SizeOf(temp) - UnsafeUtility.SizeOf(TypeOf<T>.Type);
#else
            return Marshal.SizeOf(temp) - Marshal.SizeOf(TypeOf<T>.Type);
#endif
        }

        public static bool IsUnmanaged(Type type)
        {
#if UNITYENGINE
            return UnsafeUtility.IsUnmanaged(type);
#else
            // primitive, pointer or enum -> true
            if (type.IsPrimitive || type.IsPointer || type.IsEnum)
                return true;

            // not a struct -> false
            if (!type.IsValueType)
                return false;

            // otherwise check recursively
            return type
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .All(f => IsUnmanaged(f.FieldType));
#endif
        }
        /// <summary>
        /// Wrapper struct (아무 ValueType 맴버도 갖지 않은 구조체) 는 C# CLS 에서 무조건 1 byte 를 갖습니다. 
        /// 해당 컴포넌트 타입이 버퍼에 올라갈 필요가 있는지를 확인하여 메모리 낭비를 줄입니다.
        /// </summary>
        /// <remarks>
        /// https://stackoverflow.com/a/27851610
        /// </remarks>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsZeroSizeStruct(Type t)
        {
            return t.IsValueType && !t.IsPrimitive &&
                t.GetFields((BindingFlags)0x34).All(fi => IsZeroSizeStruct(fi.FieldType));
        }

        /// <summary>
        /// <paramref name="type"/> 의 이름을 알아보기 쉬운 텍스트로 변환하여 반환합니다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToString(Type type)
        {
            const string c_TStart = "<";
            const string c_TEnd = ">";
            const string c_Pattern = ", {0}";

            if (type == null)
            {
                return "UNKNOWN NULL";
            }

            string output = type.Name;
            if (type.GenericTypeArguments.Length != 0)
            {
                output = output.Substring(0, output.Length - 2);

                output += c_TStart;
                output += ToString(type.GenericTypeArguments[0]);
                for (int i = 1; i < type.GenericTypeArguments.Length; i++)
                {
                    string temp = ToString(type.GenericTypeArguments[i]);
                    output += string.Format(c_Pattern, temp);
                }
                output += c_TEnd;
            }
            return output;
        }

        public static object GetDefaultValue(Type type)
        {
            if (type.IsValueType) return Activator.CreateInstance(type);
            else if (type.Equals(TypeOf<string>.Type)) return string.Empty;

            return null;
        }

#if !UNITYENGINE || !UNITY_BURST
        private static readonly TypedDictionary<TypeInfo> s_TypeInfoDictionary = new TypedDictionary<TypeInfo>();
#endif

        /// <summary>
        /// stack 에 할당될 수 있는 <see cref="System.Type"/> 의 Wrapper struct 를 반환합니다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TypeInfo ToTypeInfo(Type type)
        {
            if (!IsUnmanaged(type))
            {
                //PointCore.LogError(PointCore.LogChannel.Collections,
                //    $"Could not resolve type of {TypeHelper.ToString(type)} is not ValueType.");

                return new TypeInfo(type);
            }

#if UNITYENGINE && UNITY_BURST
            SharedStatic<TypeInfo> typeStatic = TypeStatic.GetValue(type);

            if (typeStatic.Data.Type == null)
            {
                typeStatic.Data
                    = new TypeInfo(type, SizeOf(type), AlignOf(type), CollectionUtility.CreateHashCode());
            }
            return typeStatic.Data;
#else
            if (!s_TypeInfoDictionary.TryGetValue(type, out TypeInfo value))
            {
                value = new TypeInfo(type, SizeOf(type), AlignOf(type), CollectionUtility.CreateHashCode());

                s_TypeInfoDictionary.Add(type, value);
            }

            return value;
#endif
        }

        #region Generics

        /// <summary>
        /// 타입 <paramref name="candidateType"/>이 상속받는 interface 
        /// <paramref name="openGenericInterfaceType"/> 의 제네릭 타입 값을 가져옵니다.
        /// </summary>
        /// <param name="candidateType"></param>
        /// <param name="openGenericInterfaceType">
        /// <code>typeof(IList{})</code>
        /// </param>
        /// <returns></returns>
        public static Type[] GetArgumentsOfInheritedOpenGenericInterface(Type candidateType, Type openGenericInterfaceType)
        {
            if (((object)openGenericInterfaceType == GenericListInterface || (object)openGenericInterfaceType == GenericCollectionInterface) && candidateType.IsArray)
            {
                return new Type[1]
                {
                    candidateType.GetElementType()
                };
            }

            if ((object)candidateType == openGenericInterfaceType)
            {
                return candidateType.GetGenericArguments();
            }

            if (candidateType.IsGenericType && (object)candidateType.GetGenericTypeDefinition() == openGenericInterfaceType)
            {
                return candidateType.GetGenericArguments();
            }

            Type[] interfaces = candidateType.GetInterfaces();
            foreach (Type type in interfaces)
            {
                if (type.IsGenericType)
                {
                    Type[] argumentsOfInheritedOpenGenericInterface = GetArgumentsOfInheritedOpenGenericInterface(type, openGenericInterfaceType);
                    if (argumentsOfInheritedOpenGenericInterface != null)
                    {
                        return argumentsOfInheritedOpenGenericInterface;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// <paramref name="type"/> 가 상속받는 인터페이스 중 <paramref name="openGenericInterfaceType"/> 을 제네릭 베이스로 갖는 모든 타입을 가져옵니다.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="openGenericInterfaceType"></param>
        /// <returns></returns>
        public static Type[] GetInterfacesWithOpenGenericInterface(Type type, Type openGenericInterfaceType)
        {
            List<Type> temp = new List<Type>();
            foreach (var item in type.GetInterfaces())
            {
                if (InheritsFrom(item, openGenericInterfaceType))
                {
                    temp.Add(item);
                }
            }
            return temp.ToArray();
        }
        public static Type[] GetInterfacesWithOpenGenericInterface<T>(Type type)
        {
            return GetInterfacesWithOpenGenericInterface(type, TypeOf<T>.Type);
        }

        //
        // Summary:
        //     Determines whether a type inherits or implements another type. Also include support
        //     for open generic base types such as List<>.
        //
        // Parameters:
        //   type:
        public static bool InheritsFrom<TBase>(Type type)
        {
            return InheritsFrom(type, typeof(TBase));
        }

        //
        // Summary:
        //     Determines whether a type inherits or implements another type. Also include support
        //     for open generic base types such as List<>.
        //
        // Parameters:
        //   type:
        //
        //   baseType:
        public static bool InheritsFrom(Type type, Type baseType)
        {
            if (baseType.IsAssignableFrom(type))
            {
                return true;
            }

            if (type.IsInterface && !baseType.IsInterface)
            {
                return false;
            }

            if (baseType.IsInterface)
            {
                return type.GetInterfaces().Contains(baseType);
            }

            Type type2 = type;
            while ((object)type2 != null)
            {
                if ((object)type2 == baseType)
                {
                    return true;
                }

                if (baseType.IsGenericTypeDefinition && type2.IsGenericType && (object)type2.GetGenericTypeDefinition() == baseType)
                {
                    return true;
                }

                type2 = type2.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Gets the generic type definition of an open generic base type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static Type GetGenericBaseType(Type type, Type baseType)
        {
            return GetGenericBaseType(type, baseType, out _);
        }
        /// <summary>
        /// Gets the generic type definition of an open generic base type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <param name="depthCount"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Type GetGenericBaseType(Type type, Type baseType, out int depthCount)
        {
            if ((object)type == null)
            {
                throw new ArgumentNullException("type");
            }

            if ((object)baseType == null)
            {
                throw new ArgumentNullException("baseType");
            }

            if (!baseType.IsGenericType)
            {
                throw new ArgumentException("Type " + baseType.Name + " is not a generic type.");
            }

            if (!InheritsFrom(type, baseType))
            {
                throw new ArgumentException("Type " + type.Name + " does not inherit from " + baseType.Name + ".");
            }

            Type type2 = type;
            depthCount = 0;
            while ((object)type2 != null && (!type2.IsGenericType || (object)type2.GetGenericTypeDefinition() != baseType))
            {
                depthCount++;
                type2 = type2.BaseType;
            }

            if ((object)type2 == null)
            {
                throw new ArgumentException(type.Name + " is assignable from " + baseType.Name + ", but base type was not found?");
            }

            return type2;
        }

        #endregion

        public static void AOTCodeGenerator<T>()
        {
            var temp = new AlignOfHelper<T>();
            TypeOf<T>.ToString();
        }
    }
}
