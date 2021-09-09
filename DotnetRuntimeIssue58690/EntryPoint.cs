using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace DotnetRuntimeIssue58690
{
    public static class EntryPoint
    {
        public static void Main()
        {
            object value = Assembly.GetExecutingAssembly().GetTypes()
                .SelectMany(type => type.GetCustomAttributes()
                    .Concat(type.GetMethods()
                        .SelectMany(method => method.GetCustomAttributes())))
                .First(attribute =>
                {
                    var type = attribute.GetType();

                    return (type.FullName is "System.Runtime.CompilerServices.NullableContextAttribute")
                        && (type.Assembly.FullName is not null)
                        && type.Assembly.FullName.Contains("DotnetRuntimeIssue58690");
                });

            var options = new JsonSerializerOptions();

            // Since System.Text.Json doesn't support serializing Reflection objects, and NullableContextAttribute contains one.
            options.Converters.Add(new ReflectionMemberInfoWriteOnlyJsonConverterFactory());

            using var stream = new MemoryStream();

            using (var writer = new Utf8JsonWriter(stream))
                JsonSerializer.Serialize(writer, value, options);

            var json = Encoding.UTF8.GetString(stream.ToArray());

            Console.WriteLine(json);
        }
    }
}
