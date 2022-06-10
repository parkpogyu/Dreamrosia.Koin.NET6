using System.Collections.Generic;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IIndicator<TSource, TContainer>
    {
        /// <summary>
        /// 지표이름
        /// </summary>
        string Name { get; }

        IEnumerable<TContainer> Generate(IEnumerable<TSource> source);
    }
}
