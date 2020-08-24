using System;
using System.Collections.Generic;

internal static class EcsTypesHelper
{
    static Dictionary<int, Type> TypeByHC = new Dictionary<int, Type>();

    public static void Register<T>()
    {
        TypeByHC[typeof(T).GetHashCode()] = typeof(T);
    } 

    public static Type GetTypeByHashCode(int hc)
    {
        Type type;
        if (TypeByHC.TryGetValue(hc, out type))
        {
            return type;
        }
        throw new Exception("Type does not registered");
    }
}
