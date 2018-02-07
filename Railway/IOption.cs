using System;
using System.Net;

namespace Railway
{
    public interface IOption<TContent>
    {
        IOption<TContent> Apply(Func<TContent, IOption<TContent>> mutator); 
    }

    public static class Option<TContent>
    {
        public static None<TContent> None => new None<TContent>();
        public static Some<TContent> Some(TContent content) => new Some<TContent>(content);
    }

    public class Some<TContent> : IOption<TContent>
    {
        public Some(TContent value)
        {
            Value = value;
        }

        public TContent Value { get; }

        public IOption<TContent> Apply(Func<TContent, IOption<TContent>> mutator)
        {
            return mutator.Invoke(Value);
        }
    }

    public class None<TContent> : IOption<TContent>
    {
        public IOption<TContent> Apply(Func<TContent, IOption<TContent>> mutator)
        {
            return new None<TContent>();
        }
    }
}
