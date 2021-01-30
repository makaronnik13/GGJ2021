using System.Collections.Generic;
using com.armatur.common.flags;

namespace com.armatur.common.util
{
    public interface IFlaggedReadOnlyList<T> : IReadOnlyList<T>
    {
        IntFlag SizeFlag { get; }
    }
}