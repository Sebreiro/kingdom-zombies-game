using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KingdomZombies.Core;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Requires actual compiled state/DTO members to contain only explicit independent values.</summary>
internal static class SnapshotShapePolicy
{
    /// <summary>Rejects unknown reference shapes, missing selectors, or mutable backing fields.</summary>
    internal static bool Independent(Type type)
    {
        if (type == typeof(string) || type == typeof(long) || type == typeof(int) || type == typeof(ProofKind))
        {
            return true;
        }

        if (type.IsGenericType)
        {
            Type definition = type.GetGenericTypeDefinition();
            return (definition == typeof(IReadOnlyList<>) || definition == typeof(IReadOnlyDictionary<,>)
                    || definition == typeof(KeyValuePair<,>))
                    && type.GetGenericArguments().All(predicate: Independent);
        }

        if (type != typeof(ExampleState) && type != typeof(SnapshotDto) && type != typeof(StableIdentity))
        {
            return false;
        }

        PropertyInfo[] properties = type.GetProperties(bindingAttr: BindingFlags.Instance | BindingFlags.Public
                | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        return properties.Length > 0 && properties.All(predicate: property => property.SetMethod is null
                && Independent(type: property.PropertyType)) && type.GetFields(bindingAttr: BindingFlags.Instance
                        | BindingFlags.NonPublic | BindingFlags.Public).All(predicate: field => field.IsInitOnly);
    }
}
