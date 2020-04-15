using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GameLoaderXF
{
    //public static class Extensions
    //{
    //    public static MethodInfo GetPrivateMethod(this LoaderViewModel payLoad, string name)
    //    {
    //        Type type = payLoad.GetType();
    //        MethodInfo output = type.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);

    //        if (output != null)
    //        {
    //            return output;
    //        }
    //        output = type.GetMethod(name);
    //        if (output != null)
    //        {
    //            return output;
    //        }
    //        type = type.BaseType;
    //        output = type.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
    //        if (output == null)
    //        {
    //            throw new BasicBlankException($"Method with the name of {name} was not found  Type was {type.Name}");
    //        }
    //        return output;
    //    }
    //}
}
