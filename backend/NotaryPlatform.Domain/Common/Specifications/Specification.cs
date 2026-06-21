using System.Linq.Expressions;

namespace NotaryPlatform.Domain.Common.Specifications;

public abstract class Specification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; protected set; }
    public List<Expression<Func<T, object>>> Includes { get; } = [];
    public List<string> IncludeStrings { get; } = [];

    public bool IsSatisfiedBy(T entity)
    {
        if (Criteria is null)
        {
            return true;
        }

        return Criteria.Compile().Invoke(entity);
    }

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected void AddInclude(string includeString)
    {
        if (!string.IsNullOrWhiteSpace(includeString))
        {
            IncludeStrings.Add(includeString);
        }
    }
}
