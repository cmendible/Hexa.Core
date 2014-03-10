//----------------------------------------------------------------------------------------------
// <copyright file="IUnitOfWork.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Linq;

    /// <summary>
    /// Contract for UnitOfWork pattern. For more
    /// references see http://martinfowler.com/eaaCatalog/unitOfWork.html or
    /// http://msdn.microsoft.com/en-us/magazine/dd882510.aspx
    /// In this solution sample Unit Of Work is implemented out-of-box in
    /// ADO.NET Entity Framework persistence engine. But for academic
    /// purposed and for mantein PI ( Persistence Ignorant ) in Domain
    /// this pattern is implemented.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Commit all changes made in  a container.
        /// </summary>
        /// <remarks>
        /// If entity have fixed properties and optimistic concurrency problem exists
        /// exception is thrown
        /// </remarks>
        void Commit();

        /// <summary>
        /// Rollbacks the changes.
        /// </summary>
        void RollbackChanges();
    }
}