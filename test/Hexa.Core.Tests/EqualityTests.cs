namespace Hexa.Core.Domain.Tests
{
    using Hexa.Core.Domain;

    using Xunit;

    public class EqualityTests
    {
        [Fact]
        public void EqualsWithDifferentIdsInDisparateClassesReturnsFalse()
        {
            var obj1 = new SimpleDomainObject();
            var obj2 = new OtherSimpleDomainObject();

            obj1.SetId(1);
            obj2.SetId(2);

            var equality = Equals(obj1, obj2);

            Assert.Equal(false, equality);
        }

        [Fact]
        public void EqualsWithDifferentIdsReturnsFalse()
        {
            var obj1 = new SimpleDomainObject();
            var obj2 = new SimpleDomainObject();

            obj1.SetId(1);
            obj2.SetId(2);

            var equality = Equals(obj1, obj2);

            Assert.Equal(false, equality);
        }

        [Fact]
        public void EqualsWithNullObjectReturnsFalse()
        {
            const SimpleDomainObject obj1 = null;
            var obj2 = new SimpleDomainObject();

            var equality = Equals(obj1, obj2);

            Assert.Equal(false, equality);
        }

        [Fact]
        public void EqualsWithOneTransientObjectReturnsFalse()
        {
            var obj1 = new SimpleDomainObject();
            var obj2 = new SimpleDomainObject();

            obj1.SetId(1);

            var equality = Equals(obj1, obj2);

            Assert.Equal(false, equality);
        }

        [Fact]
        public void EqualsWithSameIdsInDisparateClassesReturnsFalse()
        {
            var obj1 = new SimpleDomainObject();
            var obj2 = new OtherSimpleDomainObject();

            obj1.SetId(1);
            obj2.SetId(1);

            var equality = Equals(obj1, obj2);

            Assert.Equal(false, equality);
        }

        [Fact]
        public void EqualsWithSameIdsInSubclassReturnsTrue()
        {
            var obj1 = new SimpleDomainObject();
            var obj2 = new SubSimpleDomainObject();

            obj1.SetId(1);
            obj2.SetId(1);

            var equality = Equals(obj1, obj2);

            Assert.Equal(true, equality);
        }

        [Fact]
        public void EqualsWithSameIdsReturnsTrue()
        {
            var obj1 = new SimpleDomainObject();
            var obj2 = new SimpleDomainObject();

            obj1.SetId(1);
            obj2.SetId(1);

            var equality = Equals(obj1, obj2);

            Assert.Equal(true, equality);
        }

        [Fact]
        public void EqualsWithTransientObjectsReturnsFalse()
        {
            var obj1 = new SimpleDomainObject();
            var obj2 = new SimpleDomainObject();

            var equality = Equals(obj1, obj2);

            Assert.Equal(false, equality);
        }

        [Fact]
        public void EqualsWithTwoNullObjectsReturnsTrue()
        {
            const SimpleDomainObject obj1 = null;
            const SimpleDomainObject obj2 = null;

            var equality = Equals(obj1, obj2);

            Assert.Equal(true, equality);
        }
    }

    public class OtherSimpleDomainObject : BaseEntity<OtherSimpleDomainObject, int>
    {
        public void SetId(int ident)
        {
            Id = ident;
        }
    }

    public class SimpleDomainObject : BaseEntity<SimpleDomainObject, int>
    {
        public void SetId(int ident)
        {
            Id = ident;
        }
    }

    public class SubSimpleDomainObject : SimpleDomainObject
    {
    }
}