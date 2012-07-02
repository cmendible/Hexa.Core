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


namespace Hexa.Core.Domain
{
/// <summary>
/// Contract for UnitOfWork pattern. For more
/// references see http://martinfowler.com/eaaCatalog/unitOfWork.html or
/// http://msdn.microsoft.com/en-us/magazine/dd882510.aspx
/// In this solution sample Unit Of Work is implemented out-of-box in
/// ADO.NET Entity Framework persistence engine. But for academic
/// purposed and for mantein PI ( Persistence Ignorant ) in Domain
/// this pattern is implemented.
/// </summary>
    public interface IUnitOfWork : System.IDisposable
    {
        /// <summary>
        /// Commit all changes made in  a container.
        /// </summary>
        ///<remarks>
        /// If entity have fixed properties and optimistic concurrency problem exists
        /// exception is thrown
        ///</remarks>
        void Commit();

        /// <summary>
        /// Rollback changes not stored in databse at
        /// this moment. See references of UnitOfWork pattern
        /// </summary>
        void RollbackChanges();

        /// <summary>
        /// Create a object set for a type TEntity
        /// </summary>
        /// <typeparam name="TEntity">Type of elements in object set</typeparam>
        /// <returns>Object set of type {TEntity}</returns>
        IEntitySet<TEntity> CreateSet<TEntity>() where TEntity : class;
    }

}
