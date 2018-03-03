using System;

namespace Railway
{
    public static class TryExtensions
    {
        public static Try<TSuccess, TError> Try<TSuccess, TError>(this Func<TSuccess> called) where TError : Exception
        {
            try
            {
                return called.Invoke();
            }
            catch (TError ex)
            {
                return new Error<TSuccess, TError>(ex);
            }
        }

        public static Try<TSuccess2, TError> Map<TSuccess, TSuccess2, TError>(this Try<TSuccess, TError> @try,
            Func<TSuccess, TSuccess2> mapper) where TError : Exception
        {
            switch (@try)
            {
                case Success<TSuccess, TError> success:
                    return mapper(success);
                case Error<TSuccess, TError> error:
                    return (TError)error;
                default:
                    throw new InvalidOperationException();
            }
        }

        public static Try<TSuccess2, TError> FlatMap<TSuccess, TSuccess2, TError>(this Try<TSuccess, TError> @try,
            Func<TSuccess, Try<TSuccess2, TError>> mapper) where TError : Exception
        {
            switch (@try)
            {
                case Success<TSuccess, TError> success:
                    return mapper(success);
                case Error<TSuccess, TError> error:
                    return (TError)error;
                default:
                    throw new InvalidOperationException();
            }
        }

        public static TSuccess Reduce<TSuccess, TError>(this Try<TSuccess, TError> @try,
            TSuccess defaultValue) where TError : Exception
        {
            switch (@try)
            {
                case Success<TSuccess, TError> success:
                    return success;
                case Error<TSuccess, TError> _:
                    return defaultValue;
                default:
                    throw new InvalidOperationException();
            }
        }
    }

    public abstract class Try<TSuccess, TError> where TError : Exception
    {
        public static implicit operator Try<TSuccess, TError>(TSuccess success) =>
            new Success<TSuccess, TError>(success);

        public static implicit operator Try<TSuccess, TError>(TError error) =>
            new Error<TSuccess, TError>(error);
    }

    public class Success<TSuccess, TError> : Try<TSuccess, TError> where TError : Exception
    {
        public TSuccess Content { get; }

        internal Success(TSuccess content)
        {
            Content = content;
        }

        public static implicit operator TSuccess(Success<TSuccess, TError> success) => success.Content;
    }

    public class Error<TSuccess, TError> : Try<TSuccess, TError> where TError : Exception
    {
        public TError Content { get; }

        internal Error(TError content)
        {
            Content = content;
        }

        public static implicit operator TError(Error<TSuccess, TError> error) => error.Content;
    }
}
