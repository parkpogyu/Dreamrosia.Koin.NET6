using System.Collections.Generic;

namespace Dreamrosia.Koin.Shared.Wrapper
{
    public interface IResult
    {
        List<string> Messages { get; set; }

        bool Succeeded { get; set; }

        string Code { get; set; }

        string FullMessage { get; }
    }

    public interface IResult<out T> : IResult
    {
        T Data { get; }
    }
}