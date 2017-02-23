//----------------------------------------------------------------------------------------------
// <copyright file="Specifications.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
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
namespace Hexa.Core.Domain.Tests
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Specification;
    using Xunit;

    /// <summary>
    /// Summary description for SpecificationTests
    /// </summary>
    public class SpecificationTests
    {
        [Fact]
        public void AndAlsoSpecification_Creation_NullLeftSpecThrowArgumentNullException_Test()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Arrange
                DirectSpecification<Entity> leftAdHocSpecification;
                DirectSpecification<Entity> rightAdHocSpecification;

                Expression<Func<Entity, bool>> leftSpec = s => s.Id == 0;
                Expression<Func<Entity, bool>> rightSpec = s => s.SampleProperty.Length > 2;

                leftAdHocSpecification = new DirectSpecification<Entity>(leftSpec);
                rightAdHocSpecification = new DirectSpecification<Entity>(rightSpec);

                // Act
                var composite = new AndAlsoSpecification<Entity>(null, rightAdHocSpecification);
            });
        }

        [Fact]
        public void AndAlsoSpecification_Creation_NullRightSpecThrowArgumentNullException_Test()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Arrange
                DirectSpecification<Entity> leftAdHocSpecification;
                DirectSpecification<Entity> rightAdHocSpecification;

                Expression<Func<Entity, bool>> rightSpec = s => s.Id == 0;
                Expression<Func<Entity, bool>> leftSpec = s => s.SampleProperty.Length > 2;

                leftAdHocSpecification = new DirectSpecification<Entity>(leftSpec);
                rightAdHocSpecification = new DirectSpecification<Entity>(rightSpec);

                // Act
                var composite = new AndAlsoSpecification<Entity>(leftAdHocSpecification, null);
            });
        }

        [Fact]
        public void AndAlsoSpecification_Creation_Test()
        {
            // Arrange
            DirectSpecification<Entity> leftAdHocSpecification;
            DirectSpecification<Entity> rightAdHocSpecification;

            Expression<Func<Entity, bool>> leftSpec = s => s.Id == 0;
            Expression<Func<Entity, bool>> rightSpec = s => s.SampleProperty.Length > 2;

            leftAdHocSpecification = new DirectSpecification<Entity>(leftSpec);
            rightAdHocSpecification = new DirectSpecification<Entity>(rightSpec);

            // Act
            var composite = new AndAlsoSpecification<Entity>(
                leftAdHocSpecification,
                rightAdHocSpecification);

            // Assert
            Assert.NotNull(composite.SatisfiedBy());
            Assert.Same(leftAdHocSpecification, composite.LeftSideSpecification);
            Assert.Same(rightAdHocSpecification, composite.RightSideSpecification);
        }

        [Fact]
        public void DirectSpecification_Constructor_NullSpecThrowArgumentNullException_Test()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Arrange
                DirectSpecification<Entity> adHocSpecification;
                Expression<Func<Entity, bool>> spec = null;

                // Act
                adHocSpecification = new DirectSpecification<Entity>(spec);
            });
        }

        [Fact]
        public void DirectSpecification_Constructor_Test()
        {
            // Arrange
            DirectSpecification<Entity> adHocSpecification;
            Expression<Func<Entity, bool>> spec = s => s.Id == 0;

            // Act
            adHocSpecification = new DirectSpecification<Entity>(spec);

            // Assert
            object expectedValue =
                adHocSpecification.GetType().GetField(
                    "matchingCriteria",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic).GetValue(
                    adHocSpecification);
            Assert.Same(expectedValue, spec);
        }

        [Fact]
        public void NotSpecification_Creation_FromNegationOperator()
        {
            // Arrange
            Expression<Func<Entity, bool>> specificationCriteria = t => t.Id == 0;

            // Act
            var spec = new DirectSpecification<Entity>(specificationCriteria);
            ISpecification<Entity> notSpec = !spec;

            // Assert
            Assert.NotNull(notSpec);
        }

        [Fact]
        public void NotSpecification_Creation_NullCriteriaThrowArgumentNullException_Test()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Arrange
                NotSpecification<Entity> notSpec;

                // Act
                notSpec = new NotSpecification<Entity>((Expression<Func<Entity, bool>>)null);
            });
        }

        [Fact]
        public void NotSpecification_Creation_NullSpecificationThrowArgumentNullException_Test()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Arrange
                NotSpecification<Entity> notSpec;

                // Act
                notSpec = new NotSpecification<Entity>((ISpecification<Entity>)null);
            });
        }

        [Fact]
        public void NotSpecification_Creation_WithCriteria_Test()
        {
            // Arrange
            Expression<Func<Entity, bool>> specificationCriteria = t => t.Id == 0;

            // Act
            var notSpec = new NotSpecification<Entity>(specificationCriteria);

            // Assert
            Assert.NotNull(notSpec);
            Assert.NotNull(
                notSpec.GetType().GetField(
                    "originalCriteria",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic).GetValue(notSpec));
        }

        [Fact]
        public void NotSpecification_Creation_WithSpecification_Test()
        {
            // Arrange
            Expression<Func<Entity, bool>> specificationCriteria = t => t.Id == 0;
            var specification = new DirectSpecification<Entity>(specificationCriteria);

            // Act
            var notSpec = new NotSpecification<Entity>(specification);

            var resultCriteria = notSpec.GetType()
                                 .GetField(
                                     "originalCriteria",
                                     BindingFlags.Instance |
                                     BindingFlags.NonPublic).
                                 GetValue(notSpec) as Expression<Func<Entity, bool>>;

            // Assert
            Assert.NotNull(notSpec);
            Assert.NotNull(resultCriteria);
            Assert.NotNull(notSpec.SatisfiedBy());
        }

        [Fact]
        public void NotSpecification_Operators()
        {
            // Arrange
            Expression<Func<Entity, bool>> specificationCriteria = t => t.Id == 0;

            // Act
            Specification<Entity> spec = new DirectSpecification<Entity>(specificationCriteria);
            Specification<Entity> notSpec = !spec;
            ISpecification<Entity> resultAnd = notSpec && spec;
            ISpecification<Entity> resultOr = notSpec || spec;

            // Assert
            Assert.NotNull(notSpec);
            Assert.NotNull(resultAnd);
            Assert.NotNull(resultOr);
        }

        [Fact]
        public void OrSpecification_Creation_NullLeftSpecThrowArgumentNullException_Test()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Arrange
                DirectSpecification<Entity> leftAdHocSpecification;
                DirectSpecification<Entity> rightAdHocSpecification;

                Expression<Func<Entity, bool>> leftSpec = s => s.Id == 0;
                Expression<Func<Entity, bool>> rightSpec = s => s.SampleProperty.Length > 2;

                leftAdHocSpecification = new DirectSpecification<Entity>(leftSpec);
                rightAdHocSpecification = new DirectSpecification<Entity>(rightSpec);

                // Act
                var composite = new OrElseSpecification<Entity>(null, rightAdHocSpecification);
            });
        }

        [Fact]
        public void OrSpecification_Creation_NullRightSpecThrowArgumentNullException_Test()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Arrange
                DirectSpecification<Entity> leftAdHocSpecification;
                DirectSpecification<Entity> rightAdHocSpecification;

                Expression<Func<Entity, bool>> rightSpec = s => s.Id == 0;
                Expression<Func<Entity, bool>> leftSpec = s => s.SampleProperty.Length > 2;

                leftAdHocSpecification = new DirectSpecification<Entity>(leftSpec);
                rightAdHocSpecification = new DirectSpecification<Entity>(rightSpec);

                // Act
                var composite = new OrElseSpecification<Entity>(leftAdHocSpecification, null);
            });
        }

        [Fact]
        public void OrSpecification_Creation_Test()
        {
            // Arrange
            DirectSpecification<Entity> leftAdHocSpecification;
            DirectSpecification<Entity> rightAdHocSpecification;

            Expression<Func<Entity, bool>> leftSpec = s => s.Id == 0;
            Expression<Func<Entity, bool>> rightSpec = s => s.SampleProperty.Length > 2;

            leftAdHocSpecification = new DirectSpecification<Entity>(leftSpec);
            rightAdHocSpecification = new DirectSpecification<Entity>(rightSpec);

            // Act
            var composite = new OrElseSpecification<Entity>(
                leftAdHocSpecification,
                rightAdHocSpecification);

            // Assert
            Assert.NotNull(composite.SatisfiedBy());
            Assert.Same(leftAdHocSpecification, composite.LeftSideSpecification);
            Assert.Same(rightAdHocSpecification, composite.RightSideSpecification);
        }

        [Fact]
        public void Specification_AndOperator_Test()
        {
            // Arrange
            DirectSpecification<Entity> leftAdHocSpecification;
            DirectSpecification<Entity> rightAdHocSpecification;

            Expression<Func<Entity, bool>> leftSpec = s => s.Id == 0;
            Expression<Func<Entity, bool>> rightSpec = s => s.SampleProperty.Length > 2;

            Expression<Func<Entity, bool>> expected = null;
            Expression<Func<Entity, bool>> actual = null;

            // Act
            leftAdHocSpecification = new DirectSpecification<Entity>(leftSpec);
            rightAdHocSpecification = new DirectSpecification<Entity>(rightSpec);

            ISpecification<Entity> andSpec = leftAdHocSpecification & rightAdHocSpecification;
            andSpec = leftAdHocSpecification || rightAdHocSpecification;

            // Assert

            InvocationExpression invokedExpr = Expression.Invoke(rightSpec, leftSpec.Parameters.Cast<Expression>());
            expected = Expression.Lambda<Func<Entity, bool>>(Expression.AndAlso(

                           leftSpec.Body, invokedExpr),
                       leftSpec.Parameters);

            actual = andSpec.SatisfiedBy();
        }

        [Fact]
        public void Specification_OrOperator_Test()
        {
            // Arrange
            DirectSpecification<Entity> leftAdHocSpecification;
            DirectSpecification<Entity> rightAdHocSpecification;

            Expression<Func<Entity, bool>> leftSpec = s => s.Id == 0;
            Expression<Func<Entity, bool>> rightSpec = s => s.SampleProperty.Length > 2;

            Expression<Func<Entity, bool>> expected = null;
            Expression<Func<Entity, bool>> actual = null;

            // Act
            leftAdHocSpecification = new DirectSpecification<Entity>(leftSpec);
            rightAdHocSpecification = new DirectSpecification<Entity>(rightSpec);

            ISpecification<Entity> orSpec = leftAdHocSpecification | rightAdHocSpecification;
            orSpec = leftAdHocSpecification || rightAdHocSpecification;

            // Assert

            InvocationExpression invokedExpr = Expression.Invoke(rightSpec, leftSpec.Parameters.Cast<Expression>());
            expected = Expression.Lambda<Func<Entity, bool>>(Expression.Or(

                           leftSpec.Body, invokedExpr),
                       leftSpec.Parameters);

            actual = orSpec.SatisfiedBy();
        }

        [Fact]
        public void TrueSpecification_Creation_Test()
        {
            // Arrange
            ISpecification<Entity> trueSpec = new TrueSpecification<Entity>();
            bool expected = true;
            bool actual = trueSpec.SatisfiedBy().Compile()(new Entity());

            // Assert
            Assert.NotNull(trueSpec);
            Assert.Equal(expected, actual);
        }
    }
}