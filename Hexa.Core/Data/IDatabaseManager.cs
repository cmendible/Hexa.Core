//----------------------------------------------------------------------------------------------
// <copyright file="IDatabaseManager.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Data
{
    /// <summary>
    /// Handles Db creation, deletion, etc.
    /// </summary>
    public interface IDatabaseManager
    {
        /// <summary>
        /// Creates the database.
        /// </summary>
        void CreateDatabase();

        /// <summary>
        /// Checks whether the database instance exists.
        /// </summary>
        /// <returns></returns>
        bool DatabaseExists();

        /// <summary>
        /// Deletes the database.
        /// </summary>
        void DeleteDatabase();

        /// <summary>
        /// Validates the database schema.
        /// </summary>
        void ValidateDatabaseSchema();
    }
}