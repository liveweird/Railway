using System;
using Xunit;

namespace Railway.Tests
{
    internal static class SampleService
    {
        internal static Try<string, Exception> Op1(bool param)
        {
            return param.ToString();
        }

        internal static Try<string, Exception> Op2(bool param)
        {
            return new Exception("No way, Jose.");
        }

        internal static string Op3(bool param)
        {
            return param.ToString();
        }

        internal static string Op4(bool param)
        {
            throw new Exception("No way, Jose.");
        }
    }

    internal class OuterService
    {
        internal Try<InnerService, Exception> GetInner1()
        {
            return new InnerService();
        }

        internal Try<InnerService, Exception> GetInner2()
        {
            return new Exception("No way, Jose.");
        }

        internal Try<InnerService, Exception> GetInner3()
        {
            throw new Exception("No way, Jose.");
        }
    }

    internal class InnerService
    {
        internal bool GetBool1(bool value)
        {
            return value;
        }

        internal Try<bool, Exception> GetBool2(bool value)
        {
            return value;
        }
    }

    public class TryTests
    {
        [Fact]
        public void OuterFailsInnerNothing()
        {
            // map
            var result1 = new OuterService()
                .GetInner2()
                .Map(inner => inner.GetBool1(true));

            Assert.IsType<Error<bool, Exception>>(result1);
            Assert.True(result1 is Error<bool, Exception> converted1 && converted1.Content.Message == "No way, Jose.");

            // flatmap
            var result2 = new OuterService()
                .GetInner2()
                .FlatMap(inner => inner.GetBool2(true));

            Assert.IsType<Error<bool, Exception>>(result2);
            Assert.True(result2 is Error<bool, Exception> converted2 && converted2.Content.Message == "No way, Jose.");

        }

        [Fact]
        public void OuterSucceedsInnerFails()
        {}

        [Fact]
        public void OuterSucceedsInnerSucceeds()
        {}

        [Fact]
        public void OperationSucceeds()
        {
            var result = SampleService.Op1(true);
            Assert.IsType<Success<string, Exception>>(result);
            Assert.True(result is Success<string, Exception> converted && converted.Content == true.ToString());
        }

        [Fact]
        public void OperationFails()
        {
            var result = SampleService.Op2(true);
            Assert.IsType<Error<string, Exception>>(result);
            Assert.True(result is Error<string, Exception> converted && converted.Content.Message == "No way, Jose.");
        }

        [Fact]
        public void WrappedOperationSucceeds()
        {
            var result = Try.Exec(() => SampleService.Op3(true));
            Assert.IsType<Success<string, Exception>>(result);
            Assert.True(result is Success<string, Exception> converted && converted.Content == true.ToString());
        }

        [Fact]
        public void WrappedOperationFails()
        {
            var result = Try.Exec(() => SampleService.Op4(true));
            Assert.IsType<Error<string, Exception>>(result);
            Assert.True(result is Error<string, Exception> converted && converted.Content.Message == "No way, Jose.");
        }
    }
}