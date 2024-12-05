namespace Reflector;

using System.Reflection;
using System.Text;

public static class Reflector
{
    public static void PrintStructure(Type type, string path)
    {
        var typeInfo = type.GetTypeInfo();
        var filePath = Path.Combine(path, $"{typeInfo.Name}.cs");
        using var stream = File.Create(filePath);
        using var streamWriter = new StreamWriter(stream);
        if (typeInfo.IsInterface)
        {
            throw new ArgumentException("Do now work with interfaces");
        }

        if (typeInfo.IsPublic)
        {
            streamWriter.Write("public ");
        }

        if (typeInfo.IsAbstract)
        {
            if (typeInfo.IsSealed)
            {
                streamWriter.Write("static ");
            }
            else
            {
                streamWriter.Write("abstract ");
            }
        }
        else if (typeInfo.IsSealed)
        {
            streamWriter.Write("sealed");
        }

        streamWriter.Write($"class {typeInfo.Name} ");
        var ints = typeInfo.ImplementedInterfaces.ToArray();
        if (ints.Length > 0)
        {
            streamWriter.Write(":");
            for (var i = 0; i < ints.Length; ++i)
            {
                streamWriter.Write($"{ints[i].GetTypeInfo().Name}");
                if (i != ints.Length - 1)
                {
                    streamWriter.Write(", ");
                }
                else
                {
                    streamWriter.Write(" ");
                }
            }
        }

        streamWriter.Write("{\n");
        foreach (var fi in typeInfo.DeclaredFields)
        {
            PrintField(streamWriter, fi);
        }

        foreach (var mi in typeInfo.DeclaredMethods)
        {
            PrintMethod(streamWriter, mi);
        }

        streamWriter.Write("}\n");
    }

    public static string DiffClasses(Type a, Type b)
    {
        var diff = new StringBuilder();
        var memA = a.GetTypeInfo().DeclaredMembers;
        var memB = b.GetTypeInfo().DeclaredMembers;

        foreach (var mem in memA)
        {
            if (!memB.Any(m => m.Name == mem.Name))
            {
                diff.Append($"{mem.Name}\n");
            }
        }

        foreach (var mem in memB)
        {
            if (!memA.Any(m => m.Name == mem.Name))
            {
                diff.Append($"{mem.Name}\n");
            }
        }

        return diff.ToString();
    }

    private static void PrintMethod(StreamWriter streamWriter, MethodInfo methodInfo)
    {
        var offset = "    ";
        if (methodInfo.IsPublic)
        {
            streamWriter.Write(offset + "public ");
        }
        else
        {
            streamWriter.Write(offset + "private ");
        }

        if (methodInfo.IsStatic)
        {
            streamWriter.Write("static ");
        }

        if (!methodInfo.IsConstructor)
        {
            streamWriter.Write($"{methodInfo.ReturnType} ");
        }

        streamWriter.Write($"{methodInfo.Name} ");
        var pars = methodInfo.GetParameters();
        streamWriter.Write("(");
        if (pars.Length > 0)
        {
            for (int i = 0; i < pars.Length; ++i)
            {
                if (i != pars.Length - 1)
                {
                    streamWriter.Write($"{pars[i].GetType().Name}, ");
                }
                else
                {
                    streamWriter.Write($"{pars[i].GetType().Name}");
                }
            }
        }

        streamWriter.Write(")");

        streamWriter.Write("{}\n");
    }

    private static void PrintField(StreamWriter streamWriter, FieldInfo fieldInfo)
    {
        var offset = "    ";
        if (fieldInfo.IsPublic)
        {
            streamWriter.Write(offset + "public ");
        }
        else
        {
            streamWriter.Write(offset + "private ");
        }

        if (fieldInfo.IsStatic)
        {
            streamWriter.Write("static ");
        }

        streamWriter.Write($"{fieldInfo.FieldType.GetTypeInfo().Name} {fieldInfo.Name};\n");
    }
}
