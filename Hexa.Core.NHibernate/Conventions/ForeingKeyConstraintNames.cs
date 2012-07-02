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

namespace Hexa.Core.Domain
{
    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    public class ForeingKeyConstraintNames : IReferenceConvention, IHasManyConvention
    {
        #region IHasManyConvention Members

        public void Apply(IOneToManyCollectionInstance instance)
        {
            string entity = instance.EntityType.Name;
            string member = instance.Member.Name;
            string child = instance.ChildType.Name;

            instance.Key.ForeignKey(string.Format("FK_{0}{1}_{2}", entity, member, child));
        }

        #endregion

        #region IReferenceConvention Members

        public void Apply(IManyToOneInstance instance)
        {
            string entity = instance.EntityType.Name;
            string member = instance.Property.Name;

            instance.ForeignKey(string.Format("FK_{0}_{1}", entity, member));
        }

        #endregion
    }
}