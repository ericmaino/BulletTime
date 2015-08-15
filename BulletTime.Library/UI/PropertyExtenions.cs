using System;
using System.Linq.Expressions;

namespace BulletTime.UI
{
    public static class PropertyExtenions
    {
        public static Property<P> NewProperty<T, P>(this T host, Expression<Func<T, Property<P>>> action)
            where T : IPropertyHost
        {
            var expression = (MemberExpression) action.Body;
            return new Property<P>(expression.Member.Name, host);
        }
    }
}