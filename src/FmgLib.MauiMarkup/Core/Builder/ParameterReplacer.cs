using System.Linq.Expressions;

namespace FmgLib.MauiMarkup;

public class ParameterReplacer : ExpressionVisitor
{
    private readonly ParameterExpression _oldParameter;
    private readonly Expression _newParameter;

    /// <summary>
    /// Initializes a new instance of the <c>ParameterReplacer</c> class.
    /// </summary>
    /// <param name="oldParameter">The value used for <paramref name="oldParameter"/>.</param>
    /// <param name="newParameter">The value used for <paramref name="newParameter"/>.</param>
    public ParameterReplacer(ParameterExpression oldParameter, Expression newParameter)
    {
        _oldParameter = oldParameter;
        _newParameter = newParameter;
    }

    /// <summary>
    /// Executes the <c>VisitParameter</c> operation.
    /// </summary>
    /// <param name="node">The value used for <paramref name="node"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node == _oldParameter ? _newParameter : base.VisitParameter(node);
    }
}