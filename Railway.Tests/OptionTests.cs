using Xunit;

namespace Railway.Tests
{
    public class OptionTests
    {
        private static class SampleService
        {
            internal static Option<SampleEntity> Op1(bool value)
            {
                return new SampleEntity(value);
            }

            internal static Option<SampleEntity> Op2()
            {
                return Option.None;
            }
        }

        private class SampleEntity
        {
            public SampleEntity(bool logic)
            {
                Logic = logic;
            }

            public bool Logic { get; }
        }

        [Fact]
        public void MapNone()
        {
            var result = SampleService.Op2();
            var mapped = result.Map(a => a.Logic);
            Assert.IsType<None<bool>>(mapped);
        }

        [Fact]
        public void MapSome()
        {
            var result = SampleService.Op1(true);
            var mapped = result.Map(a => a.Logic);
            Assert.IsType<Some<bool>>(mapped);
            Assert.True(mapped is Some<bool> converted && converted.Value);
        }

        [Fact]
        public void WhenNegative()
        {
            var entity = new SampleEntity(false);
            var filtered = entity.When(subj => subj.Logic);
            Assert.IsType<None<SampleEntity>>(filtered);
        }

        [Fact]
        public void WhenPositive()
        {
            var entity = new SampleEntity(true);
            var filtered = entity.When(subj => subj.Logic);
            Assert.IsType<Some<SampleEntity>>(filtered);
        }

        [Fact]
        public void ReduceNone()
        {
            var none = SampleService.Op2();
            var reduced = none.Reduce(new SampleEntity(true));
            Assert.True(reduced.Logic);
        }

        [Fact]
        public void ReduceSome()
        {
            var some = SampleService.Op1(false);
            var reduced = some.Reduce(new SampleEntity(true));
            Assert.False(reduced.Logic);
        }
    }
}
