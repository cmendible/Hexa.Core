#region License

// ===================================================================================
// Copyright 2010 HexaSystems Corporation
// ===================================================================================
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// ===================================================================================
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// See the License for the specific language governing permissions and
// ===================================================================================

#endregion

using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Hexa.Core.Domain;

namespace System
{
	public static class ObjectExtensions
	{
        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T DeepClone<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
                throw new ArgumentException("The type must be serializable.", "source");

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
                return default(T);

            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static T MakeTransient<T>(this T source)
        {
            var sourceType = typeof(T);
            var baseEntityType = typeof(BaseEntity<>);

            if (sourceType.IsSubclassOfGeneric(baseEntityType))
            {
                var referenceInfos = sourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.PropertyType.IsSubclassOfGeneric(baseEntityType));
                foreach (var referenceInfo in referenceInfos)
                {
                    MakeTransient(referenceInfo.GetValue(source, null));   
                }

                var collectionInfos = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.PropertyType.GetInterface(typeof(IEnumerable).Name, true) != null && !_IsPrimitive(p.PropertyType));

                foreach (var collectionInfo in collectionInfos)
                {
                    var collection = collectionInfo.GetValue(source, null) as IEnumerable;
                    foreach (var item in collection)
                    {
                        MakeTransient(item);
                    }
                }

                _InternalMakeTransient(source);

                return source;
            }

            return source;
        }

        private static void _InternalMakeTransient<T>(T source)
        {
            var sourceType = typeof(T);
            var entityId = sourceType.GetProperty("EntityId", BindingFlags.Instance | BindingFlags.NonPublic);
            object defaultValue = entityId.PropertyType.IsValueType ? Activator.CreateInstance(entityId.PropertyType, true) : null;
            entityId.SetValue(source, defaultValue, null);

            if (sourceType.IsSubclassOfGeneric(typeof(RootEntity<>)))
            {
                var version = sourceType.GetProperty("Version", BindingFlags.Instance | BindingFlags.Public);
                version.SetValue(source, null, null);
            }

            var auditableInfo = sourceType.GetInterface(typeof(IAuditableEntity).Name, true);
            if (auditableInfo != null)
            {
                var auditable = source as IAuditableEntity;
                auditable.UpdatedAt = default(DateTime);
                auditable.UpdatedBy = null;

                var createdAt = sourceType.GetProperty("CreatedAt", BindingFlags.Instance | BindingFlags.Public);
                createdAt.SetValue(source, default(DateTime), null);

                var createdBy = sourceType.GetProperty("CreatedBy", BindingFlags.Instance | BindingFlags.Public);
                createdAt.SetValue(source, null, null);
            }
        }

        private static bool _IsPrimitive(Type t)
        {
            if (t.IsPrimitive)
                return true;

            // TODO: put any type here that you consider as primitive as I didn't
            // quite understand what your definition of primitive type is
            return new[] { 
                typeof(string), 
                typeof(ushort),
                typeof(short),
                typeof(uint),
                typeof(int),
                typeof(ulong),
                typeof(long),
                typeof(float),
                typeof(decimal),
                typeof(DateTime),
            }.Contains(t);
        }
       
	}
}
