using Application.Exceptions;

namespace Persistence.Repositories
{
    [Serializable]
    internal class EntityDoesNotExistException : TtlogixApiException
    {
        public EntityDoesNotExistException()
        {
        }

        public EntityDoesNotExistException(string? message) : base(message)
        {
        }
    }
}