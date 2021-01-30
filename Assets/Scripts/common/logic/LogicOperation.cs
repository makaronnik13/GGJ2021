using System;
using com.armatur.common.enums;

namespace com.armatur.common.logic
{
    public class LogicOperation : EnumedExample<LogicOperation>
    {
        public delegate bool OperationType(IComparable a, IComparable b);

        public static readonly LogicOperation IsNull = new LogicOperation("null", (a, b) => (a == null));
        public static readonly LogicOperation NotNull = new LogicOperation("notnull", (a, b) => (a != null));

        public static readonly LogicOperation Equal = new LogicOperation("eq", (a, b) => a.Equals(b));
        public static readonly LogicOperation NotEqual = new LogicOperation("ne", (a, b) => !a.Equals(b));

        public static readonly LogicOperation Greater = new LogicOperation("gt", (a, b) => a.CompareTo(b) > 0);

        public static readonly LogicOperation GreaterOrEqual =
            new LogicOperation("ge", (a, b) => a.CompareTo(b) > 0 || a.Equals(b));

        public static readonly LogicOperation Less = new LogicOperation("lt", (a, b) => a.CompareTo(b) < 0);

        public static readonly LogicOperation LessOrEqual =
            new LogicOperation("le", (a, b) => a.CompareTo(b) < 0 || a.Equals(b));

        public readonly OperationType Operation;


        private LogicOperation():base(null)
        {

        }

        private LogicOperation(string name, OperationType operation) : base(name)
        {
            Operation = operation;
            AddEnum(this);
        }
    }
}