using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RegalEdu.Application.Tests.TestHelpers
{
    public abstract class BaseValidatorTests<TValidator, TCommand>
        where TValidator : AbstractValidator<TCommand>
        where TCommand : class
    {
        protected TValidator Validator;

        protected BaseValidatorTests()
        {
            // Cho phép class con tự khởi tạo Validator
        }

        protected async Task AssertHasValidationErrorAsync<TProperty>(
            TCommand command,
            Expression<Func<TCommand, TProperty>> propertyExpression,
            string expectedErrorContains)
        {
            var result = await Validator.TestValidateAsync(command);

            // Debug print (giúp dễ check)
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"[DEBUG] Property: {error.PropertyName} | Message: {error.ErrorMessage}");
            }

            result.Errors
                .Should().ContainSingle(e => e.PropertyName == propertyExpression.GetFullPropertyPath()
                                          && e.ErrorMessage.Contains(expectedErrorContains));
        }

        protected async Task AssertNoValidationErrorsAsync(TCommand command)
        {
            var result = await Validator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    // Helper extension để lấy full property path
    internal static class ExpressionExtensions
    {
        public static string GetFullPropertyPath<T, TProperty>(this Expression<Func<T, TProperty>> expression)
        {
            var path = GetMemberPath(expression.Body);
            return path ?? throw new InvalidOperationException("Expression must be a MemberExpression");
        }

        private static string? GetMemberPath(Expression expression)
        {
            if (expression is MemberExpression memberExpr)
            {
                var parentPath = GetMemberPath(memberExpr.Expression);
                return parentPath == null ? memberExpr.Member.Name : $"{parentPath}.{memberExpr.Member.Name}";
            }

            if (expression is UnaryExpression unaryExpr && unaryExpr.Operand is MemberExpression innerMember)
            {
                var parentPath = GetMemberPath(innerMember.Expression);
                return parentPath == null ? innerMember.Member.Name : $"{parentPath}.{innerMember.Member.Name}";
            }

            return null;
        }
    }
}
