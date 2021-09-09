using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotnetRuntimeIssue58690
{
    public sealed class ReflectionMemberInfoWriteOnlyJsonConverter<TMemberInfo>
            : JsonConverter<TMemberInfo>
        where TMemberInfo : MemberInfo
    {
        public override TMemberInfo? Read(
                ref Utf8JsonReader      reader,
                Type                    typeToConvert,
                JsonSerializerOptions   options)
            => throw new NotSupportedException();

        public override void Write(
                Utf8JsonWriter          writer,
                TMemberInfo             value,
                JsonSerializerOptions   options)
            => writer.WriteStringValue(value switch
            {
                Type type                   => type.FullName,
                { DeclaringType: not null } => $"{value.DeclaringType.FullName}.{value.Name}",
                _                           => value.Name
            });
    }
}
