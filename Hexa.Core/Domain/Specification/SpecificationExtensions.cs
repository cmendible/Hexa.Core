
namespace Hexa.Core.Domain.Specification
{
    public static class SpecificationExtensions
    {

        /// <summary>
        ///  AndAlso operator
        /// </summary>
        /// <param name="leftSideSpecification">left operand in this AND operation</param>
        /// <param name="rightSideSpecification">right operand in this AND operation</param>
        /// <returns>New specification</returns>
        public static ISpecification<TEntity> AndAlso<TEntity>(this ISpecification<TEntity> leftSideSpecification, ISpecification<TEntity> rightSideSpecification)
            where TEntity : class
        {
            return new AndAlsoSpecification<TEntity>(leftSideSpecification, rightSideSpecification);
        }

        /// <summary>
        /// OrElse operator
        /// </summary>
        /// <param name="leftSideSpecification">left operand in this OR operation</param>
        /// <param name="rightSideSpecification">left operand in this OR operation</param>
        /// <returns>New specification</returns>
        public static ISpecification<TEntity> OrElse<TEntity>(this ISpecification<TEntity> leftSideSpecification, ISpecification<TEntity> rightSideSpecification)
             where TEntity : class
        {
            return new OrElseSpecification<TEntity>(leftSideSpecification, rightSideSpecification);
        }
    }
}
