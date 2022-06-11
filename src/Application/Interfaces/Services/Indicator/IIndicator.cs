using System.Collections.Generic;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IIndicator<TSource, TResult>
    {
        /// <summary>
        /// 지표이름
        /// </summary>
        string Name { get; }

        IEnumerable<TResult> Generate(IEnumerable<TSource> source);
    }
}
