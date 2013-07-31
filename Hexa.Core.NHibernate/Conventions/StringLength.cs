//----------------------------------------------------------------------------------------------
// <copyright file="StringLengthConvention.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    public class StringLengthConvention : IPropertyConvention
    {
        public void Apply(IPropertyInstance target)
        {
            var attribute =
                Attribute.GetCustomAttribute(target.Property.MemberInfo, typeof(StringLengthAttribute)) as
                StringLengthAttribute;

            if (attribute != null)
            {
                target.Length(attribute.MaximumLength);
            }
        }
    }
}