using System;
using Xunit;

namespace Railway.Tests
{
    internal static class SampleService
    {
        internal static Try<string, Exception> OpSuccessWrapped(bool param)
        {
            return param.ToString();
        }

        internal static Try<string, Exception> OpReturnExWrapped()
        {
            return new Exception("No way, Jose.");
        }

        internal static string OpSuccess(bool param)
        {
            return param.ToString();
        }

        internal static string OpThrowEx()
        {
            throw new Exception("No way, Jose.");
        }
    }

    internal class OuterService
    {
        internal Try<InnerService, Exception> GetInnerSuccessWrapped()
        {
            return new InnerService();
        }

        internal Try<InnerService, Exception> GetInnerReturnExWrapped()
        {
            return new Exception("No way, Jose.");
        }

        internal InnerService GetInnerSuccess()
        {
            return new InnerService();
        }

        internal InnerService GetInnerThrowEx()
        {
            throw new Exception("No way, Jose.");
        }
    }

    internal class InnerService
    {
        internal Try<bool, Exception> GetBoolSuccessWrapped(bool value)
        {
            return value;
        }

        internal Try<bool, Exception> GetBoolReturnExWrapped()
        {
            return new Exception("No way, Jose.");
        }

        internal bool GetBoolSuccess(bool value)
        {
            return value;
        }

        internal bool GetBoolThrowEx()
        {
            throw new Exception("No way, Jose.");
        }                
    }

    public class TryTests
    {
        [Fact]
        public void OuterFailsInnerNothing()
        {
            // map (when inner test doesn't support Try)
            var outerCall = new Func<InnerService>(() => new OuterService().GetInnerSuccess());
            var result1 = outerCall.Try<InnerService, Exception>()
                .Map(inner => inner.GetBoolSuccess(true));

            Assert.IsType<Error<bool, Exception>>(result1);
            Assert.True(result1 is Error<bool, Exception> converted1 && converted1.Content.Message == "No way, Jose.");

            // flatmap (when both support Try & use the same Exception wrapper class)
            var result2 = new OuterService()
                .GetInnerReturnExWrapped()
                .FlatMap(inner => inner.GetBoolSuccessWrapped(true));

            Assert.IsType<Error<bool, Exception>>(result2);
            Assert.True(result2 is Error<bool, Exception>  converted2 && converted2.Content.Message == "No way, Jose.");
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
            var result = SampleService.OpSuccessWrapped(true);
            Assert.IsType<Success<string, Exception>>(result);
            Assert.True(result is Success<string, Exception> converted && converted.Content == true.ToString());
        }

        [Fact]
        public void OperationFails()
        {
            var result = SampleService.OpThrowExWrapped();
            Assert.IsType<Error<string, Exception>>(result);
            Assert.True(result is Error<string, Exception> converted && converted.Content.Message == "No way, Jose.");
        }

        [Fact]
        public void WrappedOperationSucceeds()
        {
            var result = Try.Exec(() => SampleService.OpSuccess(true));
            Assert.IsType<Success<string, Exception>>(result);
            Assert.True(result is Success<string, Exception> converted && converted.Content == true.ToString());
        }

        [Fact]
        public void WrappedOperationFails()
        {
            var result = Try.Exec(() => SampleService.OpThrowEx());
            Assert.IsType<Error<string, Exception>>(result);
            Assert.True(result is Error<string, Exception> converted && converted.Content.Message == "No way, Jose.");
        }
    }
}