// ===================================================================================
// Microsoft Developer & Platform Evangelism
// ===================================================================================
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ===================================================================================
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
// This code is released under the terms of the MS-LPL license,
// http://microsoftnlayerapp.codeplex.com/license
// ===================================================================================
namespace Hexa.Core.Domain.Specification
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// A Direct Specification is a simple implementation
    /// of specification that acquire this from a lambda expression
    /// in  constructor
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that check this specification</typeparam>
    public class DirectSpecification<TEntity> : Specification<TEntity>
        where TEntity : class
    {
        #region Fields

        private readonly Expression<Func<TEntity, bool>> matchingCriteria;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Default constructor for Direct Specification
        /// </summary>
        /// <param name="matchingCriteria">A Matching Criteria</param>
        public DirectSpecification(Expression<Func<TEntity, bool>> matchingCriteria)
        {
            if (matchingCriteria == null)
            {
                throw new ArgumentNullException("matchingCriteria");
            }

            this.matchingCriteria = matchingCriteria;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<TEntity, bool>> SatisfiedBy()
        {
            return this.matchingCriteria;
        }

        #endregion Methods
    }
}