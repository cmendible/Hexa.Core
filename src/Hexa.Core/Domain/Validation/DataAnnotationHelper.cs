//----------------------------------------------------------------------------------------------
// <copyright file="DataAnnotationHelper.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Validation
{
    using System;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;

    /// <summary>
    /// Static class capable of readinng de DataAnnotations of a type and return a list of corresponding IValidationInfos.
    /// </summary>
    internal static class DataAnnotationHelper
    {
        public static string ParseDisplayName(Type entityType, string propertyName)
        {
            string displayName = propertyName;

            DisplayAttribute displayAttribute =  entityType.GetProperties()
                                                .Where(p => p.Name == propertyName)
                                                .SelectMany(p => p.GetCustomAttributes().OfType<DisplayAttribute>()).FirstOrDefault();

            if (displayAttribute != null)
            {
                displayName = displayAttribute.Name;
            }

            return displayName;
        }
    }
}