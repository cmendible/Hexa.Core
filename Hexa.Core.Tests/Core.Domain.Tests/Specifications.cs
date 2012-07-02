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
using System;
using System.Linq;
using System.Linq.Expressions;
using Hexa.Core.Domain.Specification;
using Hexa.Core.Tests;
using NUnit.Framework;

namespace Hexa.Core.Domain.Tests
{
    /// <summary>
    /// Summary description for SpecificationTests
    /// </summary>
    [TestFixture]
    public class SpecificationTests
    {

        [Test]
        public void DirectSpecification_Constructor_Test()
        {
            //Arrange
            DirectSpecification<Entity> adHocSpecification;
            Expression<Func<Entity,bool>> spec = s => s.Id==0;

            //Act
            adHocSpecification = new DirectSpecification<Entity>(spec);

            //Assert
            var expectedValue = adHocSpecification.GetType().GetField("_MatchingCriteria", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(adHocSpecification);
            Assert.AreSame(expectedValue, spec);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DirectSpecification_Constructor_NullSpecThrowArgumentNullException_Test()
        {
            //Arrange
            DirectSpecification<Entity> adHocSpecification;
            Expression<Func<Entity, bool>> spec = null;

            //Act
            adHocSpecification = new DirectSpecification<Entity>(spec);
        }

        [Test()]
        public void AndAlsoSpecification_Creation_Test()
        {
            //Arrange
            DirectSpecification<Entity> leftAdHocSpecification;
            DirectSpecification<Entity> rightAdHocSpecification;

            Expression<Func<Entity, bool>> leftSpec = s => s.Id == 0;
            Expression<Func<Entity, bool>> rightSpec = s => s.SampleProperty.Length > 2;

            leftAdHocSpecification = new DirectSpecification<Entity>(leftSpec);
            rightAdHocSpecification = new DirectSpecification<Entity>(rightSpec);

            //Act
            AndAlsoSpecification<Entity> composite = new AndAlsoSpecification<Entity>(leftAdHocSpecification, rightAdHocSpecification);

            //Assert
            Assert.IsNotNull(composite.SatisfiedBy());
            Assert.AreSame(leftAdHocSpecification, composite.LeftSideSpecification);
            Assert.AreSame(rightAdHocSpecification, composite.RightSideSpecification);
        }

        [Test()]
        public void OrSpecification_Creation_Test()
        {
            //Arrange
            DirectSpecification<Entity> leftAdHocSpecification;
            DirectSpecification<Entity> rightAdHocSpecification;

            Expression<Func<Entity, bool>> leftSpec = s => s.Id == 0;
            Expression<Func<Entity, bool>> rightSpec = s => s.SampleProperty.Length > 2;

            leftAdHocSpecification = new DirectSpecification<Entity>(leftSpec);
            rightAdHocSpecification = new DirectSpecification<Entity>(rightSpec);

            //Act
            OrElseSpecification<Entity> composite = new OrElseSpecification<Entity>(leftAdHocSpecification, rightAdHocSpecification);

            //Assert
            Assert.IsNotNull(composite.SatisfiedBy());
            Assert.AreSame(leftAdHocSpecification, composite.LeftSideSpecification);
            Assert.AreSame(rightAdHocSpecification, composite.RightSideSpecification);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AndAlsoSpecification_Creation_NullLeftSpecThrowArgumentNullException_Test()
        {
            //Arrange
            DirectSpecification<Entity> leftAdHocSpecification;
            DirectSpecification<Entity> rightAdHocSpecification;

            Expression<Func<Entity, bool>> leftSpec =s=>s.Id==0;
            Expression<Func<Entity, bool>> rightSpec = s => s.SampleProperty.Length > 2;

            leftAdHocSpecification = new DirectSpecification<Entity>(leftSpec);
            rightAdHocSpecification = new DirectSpecification<Entity>(rightSpec);

            //Act
            AndAlsoSpecification<Entity> composite = new AndAlsoSpecification<Entity>(null, rightAdHocSpecification);

        }

        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AndAlsoSpecification_Creation_NullRightSpecThrowArgumentNullException_Test()
        {
            //Arrange
            DirectSpecification<Entity> leftAdHocSpecification;
            DirectSpecification<Entity> rightAdHocSpecification;

            Expression<Func<Entity, bool>> rightSpec = s=>s.Id==0;
            Expression<Func<Entity, bool>> leftSpec = s => s.SampleProperty.Length > 2;

            leftAdHocSpecification = new DirectSpecification<Entity>(leftSpec);
            rightAdHocSpecification = new DirectSpecification<Entity>(rightSpec);

            //Act
            AndAlsoSpecification<Entity> composite = new AndAlsoSpecification<Entity>(leftAdHocSpecification, null);

        }

        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrSpecification_Creation_NullLeftSpecThrowArgumentNullException_Test()
        {
            //Arrange
            DirectSpecification<Entity> leftAdHocSpecification;
            DirectSpecification<Entity> rightAdHocSpecification;

            Expression<Func<Entity, bool>> leftSpec = s=>s.Id==0;
            Expression<Func<Entity, bool>> rightSpec = s => s.SampleProperty.Length > 2;

            leftAdHocSpecification = new DirectSpecification<Entity>(leftSpec);
            rightAdHocSpecification = new DirectSpecification<Entity>(rightSpec);

            //Act
            OrElseSpecification<Entity> composite = new OrElseSpecification<Entity>(null, rightAdHocSpecification);

        }

        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrSpecification_Creation_NullRightSpecThrowArgumentNullException_Test()
        {
            //Arrange
            DirectSpecification<Entity> leftAdHocSpecification;
            DirectSpecification<Entity> rightAdHocSpecification;

            Expression<Func<Entity, bool>> rightSpec = s=>s.Id==0;
            Expression<Func<Entity, bool>> leftSpec = s => s.SampleProperty.Length > 2;

            leftAdHocSpecification = new DirectSpecification<Entity>(leftSpec);
            rightAdHocSpecification = new DirectSpecification<Entity>(rightSpec);

            //Act
            OrElseSpecification<Entity> composite = new OrElseSpecification<Entity>(leftAdHocSpecification, null);

        }

        [Test]
        public void Specification_AndOperator_Test()
        {
            //Arrange
            DirectSpecification<Entity> leftAdHocSpecification;
            DirectSpecification<Entity> rightAdHocSpecification;

            Expression<Func<Entity, bool>> leftSpec = s => s.Id == 0;
            Expression<Func<Entity, bool>> rightSpec = s => s.SampleProperty.Length>2;

            Expression<Func<Entity, bool>> expected = null;
            Expression<Func<Entity, bool>> actual = null;

            //Act
            leftAdHocSpecification = new DirectSpecification<Entity>(leftSpec);
            rightAdHocSpecification = new DirectSpecification<Entity>(rightSpec);

            ISpecification<Entity> andSpec = leftAdHocSpecification & rightAdHocSpecification;
            andSpec = leftAdHocSpecification || rightAdHocSpecification;
            //Assert
            

            InvocationExpression invokedExpr = Expression.Invoke(rightSpec, leftSpec.Parameters.Cast<Expression>());
            expected = Expression.Lambda<Func<Entity, bool>>(Expression.AndAlso(leftSpec.Body, invokedExpr), leftSpec.Parameters);

            actual = andSpec.SatisfiedBy();
        }

        [Test]
        public void Specification_OrOperator_Test()
        {
            //Arrange
            DirectSpecification<Entity> leftAdHocSpecification;
            DirectSpecification<Entity> rightAdHocSpecification;

            Expression<Func<Entity, bool>> leftSpec = s => s.Id == 0;
            Expression<Func<Entity, bool>> rightSpec = s => s.SampleProperty.Length > 2;

            Expression<Func<Entity, bool>> expected = null;
            Expression<Func<Entity, bool>> actual = null;

            //Act
            leftAdHocSpecification = new DirectSpecification<Entity>(leftSpec);
            rightAdHocSpecification = new DirectSpecification<Entity>(rightSpec);

            ISpecification<Entity> orSpec = leftAdHocSpecification | rightAdHocSpecification;
            orSpec = leftAdHocSpecification || rightAdHocSpecification;

            //Assert


            InvocationExpression invokedExpr = Expression.Invoke(rightSpec, leftSpec.Parameters.Cast<Expression>());
            expected = Expression.Lambda<Func<Entity, bool>>(Expression.Or(leftSpec.Body, invokedExpr), leftSpec.Parameters);

            actual = orSpec.SatisfiedBy();


        }

        [Test()]
        public void NotSpecification_Creation_WithSpecification_Test()
        {
            //Arrange
            Expression<Func<Entity, bool>> specificationCriteria = t => t.Id == 0;
            DirectSpecification<Entity> specification = new DirectSpecification<Entity>(specificationCriteria);

            //Act
            NotSpecification<Entity> notSpec = new NotSpecification<Entity>(specification);

            Expression<Func<Entity, bool>> resultCriteria = notSpec.GetType()
                .GetField("_OriginalCriteria", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(notSpec) as Expression<Func<Entity, bool>>;

            //Assert
            Assert.IsNotNull(notSpec);
            Assert.IsNotNull(resultCriteria);
            Assert.IsNotNull(notSpec.SatisfiedBy());
            
        }

        [Test()]
        public void NotSpecification_Creation_WithCriteria_Test()
        {
            //Arrange
            Expression<Func<Entity, bool>> specificationCriteria = t => t.Id == 0;
            

            //Act
            NotSpecification<Entity> notSpec = new NotSpecification<Entity>(specificationCriteria);

            //Assert
            Assert.IsNotNull(notSpec);
            Assert.IsNotNull(notSpec.GetType().GetField("_OriginalCriteria", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(notSpec));
        }

        [Test()]
        public void NotSpecification_Creation_FromNegationOperator()
        {
            //Arrange
            Expression<Func<Entity, bool>> specificationCriteria = t => t.Id == 0;


            //Act
            DirectSpecification<Entity> spec = new DirectSpecification<Entity>(specificationCriteria);
            ISpecification<Entity> notSpec = !spec;

            //Assert
            Assert.IsNotNull(notSpec);
            
        }

        [Test()]
        public void NotSpecification_Operators()
        {
            //Arrange
            Expression<Func<Entity, bool>> specificationCriteria = t => t.Id == 0;


            //Act
            Specification<Entity> spec = new DirectSpecification<Entity>(specificationCriteria);
            Specification<Entity> notSpec = !spec;
            ISpecification<Entity> resultAnd = notSpec && spec;
            ISpecification<Entity> resultOr = notSpec || spec;

            //Assert
            Assert.IsNotNull(notSpec);
            Assert.IsNotNull(resultAnd);
            Assert.IsNotNull(resultOr);

        }

        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NotSpecification_Creation_NullSpecificationThrowArgumentNullException_Test()
        {
            //Arrange
            NotSpecification<Entity> notSpec;

            //Act
            notSpec = new NotSpecification<Entity>((ISpecification<Entity>)null);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NotSpecification_Creation_NullCriteriaThrowArgumentNullException_Test()
        {
            //Arrange
            NotSpecification<Entity> notSpec;

            //Act
            notSpec = new NotSpecification<Entity>((Expression<Func<Entity,bool>>)null);
        }

        [Test()]
        public void TrueSpecification_Creation_Test()
        {
            //Arrange
            ISpecification<Entity> trueSpec = new TrueSpecification<Entity>();
            bool expected = true;
            bool actual = trueSpec.SatisfiedBy().Compile()(new Entity());
            //Assert
            Assert.IsNotNull(trueSpec);
            Assert.AreEqual(expected,actual);
        }
    }
}
