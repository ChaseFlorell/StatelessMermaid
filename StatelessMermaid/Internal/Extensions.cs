using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Stateless;

namespace StatelessMermaid.Internal;

internal static partial class Extensions
{
    internal static string ToMermaidId(this string value) =>
        string.IsNullOrEmpty(value) ? "_" : InvalidIdCharRegex().Replace(value, "_");

    internal static string ToHumanReadable(this string identifier) =>
        WordBoundaryRegex().Replace(identifier, " ");

    internal static string ToMermaidSyntax(this DiagramDirection direction) => direction switch
    {
        DiagramDirection.TopToBottom => "TB",
        DiagramDirection.LeftToRight => "LR",
        DiagramDirection.RightToLeft => "RL",
        DiagramDirection.BottomToTop => "BT",
        _ => "TB"
    };

    internal static string ToMermaidSyntax(this DiagramVersion version) => version switch
    {
        DiagramVersion.V2 => "-v2",
        DiagramVersion.V1 => string.Empty,
        _ => "-v2"
    };

    internal static string ToNotePosition(this DiagramDirection direction) =>
        direction == DiagramDirection.RightToLeft ? "left of" : "right of";

    internal static string ToFriendlyName(this Type type)
    {
        if (type.IsGenericType)
        {
            var baseName = type.Name[..type.Name.IndexOf('`')];
            var args = string.Join(", ", type.GetGenericArguments().Select(ToFriendlyName));
            return $"{baseName}<{args}>";
        }

        return type.Name switch
        {
            "Boolean" => "bool",
            "Byte" => "byte",
            "SByte" => "sbyte",
            "Int16" => "short",
            "UInt16" => "ushort",
            "Int32" => "int",
            "UInt32" => "uint",
            "Int64" => "long",
            "UInt64" => "ulong",
            "Single" => "float",
            "Double" => "double",
            "Decimal" => "decimal",
            "String" => "string",
            "Char" => "char",
            "Object" => "object",
            var name => name
        };
    }

    internal static Dictionary<string, Type[]> ExtractTriggerParameters<TState, TTrigger>(this StateMachine<TState, TTrigger> machine)
    {
        try
        {
            var field = typeof(StateMachine<TState, TTrigger>)
                .GetField("_triggerConfiguration", BindingFlags.NonPublic | BindingFlags.Instance);

            if (field?.GetValue(machine) is not IDictionary dict)
            {
                return new Dictionary<string, Type[]>();
            }

            var result = new Dictionary<string, Type[]>();
            foreach (DictionaryEntry entry in dict)
            {
                entry.ExtractGenerics(ref result);
            }

            return result;
        }
        catch
        {
            return new Dictionary<string, Type[]>();
        }
    }

    private static void ExtractGenerics(this DictionaryEntry entry, ref Dictionary<string, Type[]> result)
    {
        if (entry.Value is null)
        {
            return;
        }

        var valueType = entry.Value.GetType();
        if (!valueType.IsGenericType)
        {
            return;
        }

        // TriggerWithParameters<TArg> is a nested class inside StateMachine<TState, TTrigger>.
        // GetGenericArguments() returns all type args including those from the outer class,
        // so we skip the outer class's args to isolate the trigger parameter types.
        var outerArgCount = valueType.GetGenericTypeDefinition()
            .DeclaringType?.GetGenericArguments().Length ?? 0;
        var paramTypes = valueType.GetGenericArguments().Skip(outerArgCount).ToArray();

        if (paramTypes.Length > 0)
        {
            result[entry.Key.ToString()!] = paramTypes;
        }
    }


    [GeneratedRegex("[^a-zA-Z0-9_]")]
    private static partial Regex InvalidIdCharRegex();

    [GeneratedRegex(@"(?<=[a-z\d])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])")]
    private static partial Regex WordBoundaryRegex();
}