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
    }

    public class EitherTests
    {
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
    }
}