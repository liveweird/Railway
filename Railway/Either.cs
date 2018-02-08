using System;

namespace Railway
{
    public static class EitherExtensions
    {
        public static Either<TLeft2, TRight> Map<TLeft, TLeft2, TRight>(this Either<TLeft, TRight> either,
            Func<TLeft, TLeft2> mapper)
        {
            switch (either)
            {
                case Left<TLeft, TRight> left:
                    return mapper(left);
                case Right<TLeft, TRight> right:
                    return (TRight)right;
                default:
                    throw new InvalidOperationException();
            }
        }

        public static TLeft Reduce<TLeft, TRight>(this Either<TLeft, TRight> either,
            TLeft defaultValue)
        {
            switch (either)
            {
                case Left<TLeft, TRight> left:
                    return left;
                case Right<TLeft, TRight> _:
                    return defaultValue;
                default:
                    throw new InvalidOperationException();
            }
        }
    }

    public abstract class Either<TLeft, TRight>
    {
        public static implicit operator Either<TLeft, TRight>(TLeft left) =>
            new Left<TLeft, TRight>(left);

        public static implicit operator Either<TLeft, TRight>(TRight right) =>
            new Right<TLeft, TRight>(right);
    }

    public class Left<TLeft, TRight> : Either<TLeft, TRight>
    {
        private TLeft Content { get; }

        public Left(TLeft content)
        {
            Content = content;
        }

        public static implicit operator TLeft(Left<TLeft, TRight> left) => left.Content;
    }

    public class Right<TLeft, TRight> : Either<TLeft, TRight>
    {
        private TRight Content { get; }

        public Right(TRight content)
        {
            Content = content;
        }

        public static implicit operator TRight(Right<TLeft, TRight> right) => right.Content;
    }
}
