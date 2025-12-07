namespace Mos3ef.Api.Exceptions
{
    /// <summary>
    /// Exception thrown when a user is authenticated but not authorized to access a resource (403 Forbidden).
    /// </summary>
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message) { }
    }
}
