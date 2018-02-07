using Xunit;

namespace Railway.Tests
{
    public class OptionTests
    {
        internal class Something
        {
            public Something(bool logic)
            {
                Logic = logic;
            }

            public bool Logic { get; }
        }

        [Fact]
        public void ApplyNoneOnNone()
        {
            IOption<Something> none = Option<Something>.None;
            Assert.IsType<None<Something>>(none.Apply(a => Option<Something>.None));
        }

        [Fact]
        public void ApplySomeOnNone()
        {
            IOption<Something> none = Option<Something>.None;
            Assert.IsType<None<Something>>(none.Apply(a => Option<Something>.Some(new Something(true))));
        }

        [Fact]
        public void ApplyNoneOnSome()
        {
            IOption<Something> none = Option<Something>.Some(new Something(true));
            Assert.IsType<None<Something>>(none.Apply(a => Option<Something>.None));
        }

        [Fact]
        public void ApplySomeOnSome()
        {
            IOption<Something> none = Option<Something>.Some(new Something(true));
            var applied = none.Apply(a => Option<Something>.Some(new Something(!(a.Logic))));
            Assert.IsType<Some<Something>>(applied);
            Assert.False(applied is Some<Something> converted && converted.Value.Logic);
        }

    }
}
