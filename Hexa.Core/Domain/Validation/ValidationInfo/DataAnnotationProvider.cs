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

namespace Hexa.Core.Validation
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Returns the IValidationInfos corresponding to the DataAnnotations in type Tentity
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class DataAnnotationValidationInfoProvider<TEntity> : IValidationInfoProvider
    {
        #region IValidationInfoProvider Members

        /// <summary>
        /// Gets the validation info.
        /// </summary>
        /// <returns></returns>
        public IList<IValidationInfo> GetValidationInfo()
        {
            return DataAnnotationHelper.GetValidationInfoList<TEntity>();
        }

        /// <summary>
        /// Gets the validation info.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public IList<IValidationInfo> GetValidationInfo(string propertyName)
        {
            return GetValidationInfo().Where(i => i.PropertyInfo.Name == propertyName).ToList();
        }

        #endregion
    }
}