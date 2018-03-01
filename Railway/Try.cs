using System;

namespace Railway
{
    public static class TryExtensions
    {
        public static Try<TSuccess2, TError> Map<TSuccess, TSuccess2, TError>(this Try<TSuccess, TError> @try,
            Func<TSuccess, TSuccess2> mapper)
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
            Func<TSuccess, Try<TSuccess2, TError>> mapper)
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
            TSuccess defaultValue)
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

    public static class Try
    {
        public static Try<TSuccess, Exception> Exec<TSuccess>(Func<TSuccess> called)
        {
            try
            {
                return called.Invoke();
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }

    public abstract class Try<TSuccess, TError>
    {
        public static implicit operator Try<TSuccess, TError>(TSuccess success) =>
            new Success<TSuccess, TError>(success);

        public static implicit operator Try<TSuccess, TError>(TError error) =>
            new Error<TSuccess, TError>(error);
    }

    public class Success<TSuccess, TError> : Try<TSuccess, TError>
    {
        public TSuccess Content { get; }

        internal Success(TSuccess content)
        {
            Content = content;
        }

        public static implicit operator TSuccess(Success<TSuccess, TError> success) => success.Content;
    }

    public class Error<TSuccess, TError> : Try<TSuccess, TError>
    {
        public TError Content { get; }

        internal Error(TError content)
        {
            Content = content;
        }

        public static implicit operator TError(Error<TSuccess, TError> error) => error.Content;
    }
}
