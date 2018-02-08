using System;

namespace Railway
{
    public static class OptionExtensions
    {
        public static Option<TResult> Map<TContent, TResult>(this Option<TContent> option,
            Func<TContent, TResult> mapper)
        {
            switch (option)
            {
                case Some<TContent> some:
                    return mapper(some);
                default:
                    return Option.None;
            }
        }

        public static Option<TContent> When<TContent>(this TContent content,
            Func<TContent, bool> predicate)
        {
            if (predicate(content))
            {
                return content;
            }

            return Option.None;
        }

        public static TContent Reduce<TContent>(this Option<TContent> option,
            TContent defaultValue)
        {
            switch (option)
            {
                case Some<TContent> some:
                    return some.Value;
                default:
                    return defaultValue;
            }
        }
    }

    public abstract class Option
    {
        public static None None = new None();
    }

    public abstract class Option<TContent>
    {
        public static implicit operator Option<TContent>(TContent content) => new Some<TContent>(content);
        public static implicit operator Option<TContent>(None none) => new None<TContent>();
    }

    public class Some<TContent> : Option<TContent>
    {
        public Some(TContent value)
        {
            Value = value;
        }

        public TContent Value { get; }

        public static implicit operator TContent(Some<TContent> wrapped) => wrapped.Value;
    }

    public class None
    {
        internal None() { }
    }

    public class None<TContent> : Option<TContent>
    {
    }
}
