using System;
using System.Linq.Expressions;
//https://daveaglick.com/posts/using-expression-trees-to-get-class-and-member-names
namespace csharp
{
    public class Foo
    {
        public int Bar { get; set; }
        public int Baz(int red, string green) => 1;
    }

    class Program
    {
        public static (string, string) PropertyName<T>(Expression<Func<T, object>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member == null)
            {
                member = (expression.Body as UnaryExpression)?.Operand as MemberExpression;
            }
            if (member == null)
            {
                throw new ArgumentException("Must be a member expression.");
            }
            return (typeof(T).Name, member.Member.Name);
        }

        public static (string, string) MemberName<T>(Expression<Func<T, string>> expression)
        {
            ConstantExpression constant = expression.Body as ConstantExpression;
            if (constant == null)
            {
                throw new ArgumentException("Expression must be a constant expression.");
            }
            return (typeof(T).Name, constant.Value.ToString());
        }

        static void Main(string[] args)
        {
            Console.WriteLine("PropertyName: {0}", PropertyName<Foo>(x => x.Bar));
            Console.WriteLine("MemberName: {0}", MemberName<Foo>(x => nameof(x.Baz)));

            //Evaluation
            Expression<Func<int>> add = () => 1 + 2;
            var func = add.Compile(); // Create Delegate
            var answer = func(); // Invoke Delegate
            Console.WriteLine("The answer is {0}", answer);
        }
    }
}
