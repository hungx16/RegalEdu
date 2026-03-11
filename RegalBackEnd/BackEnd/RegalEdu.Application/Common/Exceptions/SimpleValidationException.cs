namespace RegalEdu.Application.Common.Exceptions
{
    public class SimpleValidationException : Exception
    {
        public List<string> Errors { get; }

        public SimpleValidationException(IEnumerable<string> errors)
            : base("Validation failed")
        {
            Errors = errors.ToList();
        }
    }
}
