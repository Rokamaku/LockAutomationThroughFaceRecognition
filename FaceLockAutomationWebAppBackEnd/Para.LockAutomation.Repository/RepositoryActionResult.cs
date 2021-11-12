using System;
using System.Collections.Generic;
using System.Text;

namespace Para.LockAutomation.Repository
{
    public class RepositoryActionResult<T>
    {
        public T Entity { get; private set; }
        public RepositoryActionStatus Status { get; private set; }

        public Exception Exception { get; private set; }
        public string ErrorMessage { get; set; }

        public RepositoryActionResult(T entity, RepositoryActionStatus status)
        {
            Entity = entity;
            Status = status;
        }

        public RepositoryActionResult(T entity, RepositoryActionStatus status, Exception exception) : this(entity, status)
        {
            Exception = exception;
        }

        public RepositoryActionResult(T entity, RepositoryActionStatus status, Exception exception, string errorMessage) : this(entity, status)
        {
            ErrorMessage = errorMessage;
        }
    }

    public enum RepositoryActionStatus
    {
        Ok = 1,
        Created = 2,
        Updated = 3,
        NotFound = 4,
        Deleted = 5,
        NothingModified = 6,
        Error = 7,
        Duplicated = 8,
        Warning = 9
    }
}
