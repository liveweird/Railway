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
            return new Exception("OpReturnExWrapped");
        }

        internal static string OpSuccess(bool param)
        {
            return param.ToString();
        }

        internal static string OpThrowEx()
        {
            throw new Exception("OpThrowEx");
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
            return new Exception("GetInnerReturnExWrapped");
        }

        internal InnerService GetInnerSuccess()
        {
            return new InnerService();
        }

        internal InnerService GetInnerThrowEx()
        {
            throw new Exception("GetInnerThrowEx");
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
            return new Exception("GetBoolReturnExWrapped");
        }

        internal bool GetBoolSuccess(bool value)
        {
            return value;
        }

        internal bool GetBoolThrowEx()
        {
            throw new Exception("GetBoolThrowEx");
        }                
    }

    public class TryTests
    {
        [Fact]
        public void OuterFailsInnerNothing()
        {
            // when outer doesn't support Try, so you have to wrap
            Func<InnerService> outerCall = () => new OuterService().GetInnerThrowEx();
            var result1 = outerCall.Try<InnerService, Exception>()
                .Map(inner => inner.GetBoolSuccess(true));

            Assert.IsType<Error<bool, Exception>>(result1);
            Assert.True(result1 is Error<bool, Exception> converted1 && converted1.Content.Message == "GetInnerThrowEx");

            // when outer does support Try, so you don't need to wrap
            var result2 = new OuterService()
                .GetInnerReturnExWrapped()
                .Map(inner => inner.GetBoolSuccess(true));

            Assert.IsType<Error<bool, Exception>>(result2);
            Assert.True(result2 is Error<bool, Exception>  converted2 && converted2.Content.Message == "GetInnerReturnExWrapped");
        }

        [Fact]
        public void OuterSucceedsInnerFails()
        {
            // when inner doesn't support Try
            Func<InnerService> outerCall1 = () => new OuterService().GetInnerSuccess();
            var result1 = outerCall1.Try<InnerService, Exception>()
                .FlatMap(inner =>
                {
                    Func<bool> innerCall1 = inner.GetBoolThrowEx;
                    return innerCall1.Try<bool, Exception>();
                });

            Assert.IsType<Error<bool, Exception>>(result1);
            Assert.True(result1 is Error<bool, Exception> converted1 && converted1.Content.Message == "GetBoolThrowEx");

            // when inner does support Try
            Func<InnerService> outerCall2 = () => new OuterService().GetInnerSuccess();
            var result2 = outerCall2.Try<InnerService, Exception>()
                .FlatMap(inner => inner.GetBoolReturnExWrapped());

            Assert.IsType<Error<bool, Exception>>(result2);
            Assert.True(result2 is Error<bool, Exception> converted2 && converted2.Content.Message == "GetBoolReturnExWrapped");
        }

        [Fact]
        public void OuterSucceedsInnerSucceeds()
        {
            // when inner doesnt support Try
            Func<InnerService> outerCall1 = () => new OuterService().GetInnerSuccess();
            var result1 = outerCall1.Try<InnerService, Exception>()
                .Map(inner => inner.GetBoolSuccess(true));

            Assert.IsType<Success<bool, Exception>>(result1);
            Assert.True(result1 is Success<bool, Exception> converted1 && converted1.Content);

            // when inner doesnt support Try
            Func<InnerService> outerCall2 = () => new OuterService().GetInnerSuccess();
            var result2 = outerCall2.Try<InnerService, Exception>()
                .FlatMap(inner => inner.GetBoolSuccessWrapped(true));

            Assert.IsType<Success<bool, Exception>>(result2);
            Assert.True(result2 is Success<bool, Exception> converted2 && converted2.Content);
        }

        [Fact]
        public void OperationSucceeds()
        {
            Func<string> caller = () => SampleService.OpSuccess(true);
            var result = caller.Try<string, Exception>();
            Assert.IsType<Success<string, Exception>>(result);
            Assert.True(result is Success<string, Exception> converted && converted.Content == true.ToString());
        }

        [Fact]
        public void OperationFails()
        {
            Func<string> caller = SampleService.OpThrowEx;
            var result = caller.Try<string, Exception>();
            Assert.IsType<Error<string, Exception>>(result);
            Assert.True(result is Error<string, Exception> converted && converted.Content.Message == "OpThrowEx");
        }

        [Fact]
        public void WrappedOperationSucceeds()
        {
            var result = SampleService.OpSuccessWrapped(true);
            Assert.IsType<Success<string, Exception>>(result);
            Assert.True(result is Success<string, Exception> converted && converted.Content == true.ToString());
        }

        [Fact]
        public void WrappedOperationFails()
        {
            var result = SampleService.OpReturnExWrapped();
            Assert.IsType<Error<string, Exception>>(result);
            Assert.True(result is Error<string, Exception> converted && converted.Content.Message == "OpReturnExWrapped");
        }
    }
}