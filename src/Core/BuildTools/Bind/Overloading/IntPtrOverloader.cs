// This file is part of Silk.NET.
// 
// You may modify and distribute Silk.NET under the terms
// of the MIT license. See the LICENSE file for details.

using System.Collections.Generic;
using System.Text;
using Silk.NET.BuildTools.Common;
using Silk.NET.BuildTools.Common.Builders;
using Silk.NET.BuildTools.Common.Functions;

namespace Silk.NET.BuildTools.Bind.Overloading
{
    public class IntPtrOverloader : IFunctionOverloader
    {
        public IEnumerable<ImplementedFunction> CreateOverloads(Function function)
        {
            var @params = new List<Parameter>(function.Parameters);
            var sb = new StringBuilder();
            sb.AppendLine("// IntPtrOverloader");
            if (function.ReturnType.ToString() != "void")
            {
                sb.Append("return ");
            }

            sb.Append($"{function.Name}(");
            var ret = false;
            for (var i = 0; i < function.Parameters.Count; i++)
            {
                var parameter = function.Parameters[i];
                var parameterName = Utilities.CSharpKeywords.Contains(parameter.Name)
                    ? $"@{parameter.Name}"
                    : parameter.Name;
                if (parameter.Type.IsIntPtr() && !(parameter.Type.IsOut))
                {
                    @params[i] = new ParameterSignatureBuilder(parameter)
                        .WithType(new TypeSignatureBuilder(parameter.Type).WithName("int").Build())
                        .Build();
                    sb.Append($"new IntPtr({parameterName})");
                    ret = true;
                }
                else if (parameter.Type.IsUIntPtr() && !(parameter.Type.IsOut))
                {
                    @params[i] = new ParameterSignatureBuilder(parameter)
                        .WithType(new TypeSignatureBuilder(parameter.Type).WithName("uint").Build())
                        .Build();
                    sb.Append($"new UIntPtr({parameterName})");
                    ret = true;
                }
                else
                {
                    var prefix = parameter.Type.IsOut ? "out " : parameter.Type.IsByRef ? "ref " : string.Empty;
                    sb.Append($"{prefix}{parameterName}");
                }

                if (i != function.Parameters.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.AppendLine(");");

            if (ret)
            {
                yield return new ImplementedFunction(new FunctionSignatureBuilder(function).WithParameters(@params).Build(), sb);
            }
        }
    }
}
