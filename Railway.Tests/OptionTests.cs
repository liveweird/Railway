using Xunit;

namespace Railway.Tests
{
    public class OptionTests
    {
        internal class SampleEntity
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
            Option<SampleEntity> none = Option.None;
            Assert.IsType<None<int>>(none.Map(a => 1));
        }

        [Fact]
        public void MapSome()
        {
            Option<SampleEntity> some = new SampleEntity(true);
            var mapped = some.Map(a => 1);
            Assert.IsType<Some<int>>(mapped);
            Assert.True(mapped is Some<int> converted && converted.Value == 1);
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
            Option<SampleEntity> none = Option.None;
            var reduced = none.Reduce(new SampleEntity(true));
            Assert.True(reduced.Logic);
        }

        [Fact]
        public void ReduceSome()
        {
            Option<SampleEntity> none = new SampleEntity(false);
            var reduced = none.Reduce(new SampleEntity(true));
            Assert.False(reduced.Logic);
        }
    }
}
