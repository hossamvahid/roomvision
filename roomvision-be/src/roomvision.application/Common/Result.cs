using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace roomvision.application.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string? Error { get; }
        public ErrorTypes? ErrorType { get; }

        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, string? error, ErrorTypes? errorType)
        {
            IsSuccess = isSuccess;
            Error = error;
            ErrorType = errorType;
        }

        public static Result Success()
        {
            return new Result(true, null, null);
        }

        public static Result Failure(string error, ErrorTypes errorType)
        {
            return new Result(false, error, errorType);
        }
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        protected Result(bool isSuccess, T? value, string? error, ErrorTypes? errorType)
            : base(isSuccess, error, errorType)
        {
            Value = value;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, null, null);
        }

        public static new Result<T> Failure(string error, ErrorTypes errorType)
        {
            return new Result<T>(false, default, error, errorType);
        }
    }
}