

using FluentValidation;
using FluentValidation.Results;
using MediatR;
using RegalEdu.Application.Common.Exceptions;

namespace RegalEdu.Application.Common.Behaviors
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;

        }

        //public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        //{
        //    if (_validators.Any ( ))
        //    {
        //        var context = new ValidationContext<TRequest> (request);
        //        var validationResults = await Task.WhenAll (_validators.Select (v => v.ValidateAsync (context, cancellationToken)));
        //        var failures = validationResults.SelectMany (r => r.Errors).Where (f => f != null).ToList ( );

        //        if (failures.Count != 0)
        //        {
        //            var errors = failures.Select (f => f.ErrorMessage).ToList ( );
        //            throw new SimpleValidationException (errors);
        //        }
        //    }
        //    return await next ( );
        //}

        public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
        {
            if (_validators.Any ( ))
            {
                var context = new ValidationContext<TRequest> (request);
                var failures = new List<ValidationFailure> ( );

                // CHẠY TUẦN TỰ để tránh DbContext concurrency
                foreach (var validator in _validators)
                {
                    var result = await validator.ValidateAsync (context, cancellationToken).ConfigureAwait (false);
                    if (!result.IsValid && result.Errors is not null)
                    {
                        failures.AddRange ((IEnumerable<ValidationFailure>)result.Errors.Where (f => f is not null));
                    }
                }

                if (failures.Count != 0)
                {
                    // Có thể distinct để tránh trùng thông điệp
                    var errors = failures
                        .Where (f => !string.IsNullOrWhiteSpace (f.ErrorMessage))
                        .Select (f => f.ErrorMessage)
                        .Distinct ( )
                        .ToList ( );

                    throw new SimpleValidationException ((IEnumerable<string>)errors);
                    //throw new FluentValidation.ValidationException(failures);

                }
            }

            return await next ( ).ConfigureAwait (false);
        }

    }
}
