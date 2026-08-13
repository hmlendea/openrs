#!/usr/bin/env bash

set -euo pipefail

readonly SCRIPT_DIRECTORY="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
readonly WORKSPACE_DIRECTORY="$(cd "${SCRIPT_DIRECTORY}/../.." && pwd)"
readonly BASELINE_DIRECTORY="$(mktemp -d)"
readonly REPORT_DIRECTORY="$(mktemp -d)"
readonly API_MANIFEST_CODE="$(cat <<'CSHARP'
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

string GetTypeName(Type type)
{
    if (type is null)
    {
        return string.Empty;
    }

    if (type.FullName is not null)
    {
        return type.FullName;
    }

    return type.Name;
}

bool IsApiType(Type type) =>
    type.IsPublic ||
    type.IsNestedPublic ||
    type.IsNestedFamily ||
    type.IsNestedFamORAssem;

bool IsApiMethod(MethodBase method) =>
    method.IsPublic ||
    method.IsFamily ||
    method.IsFamilyOrAssembly;

bool IsApiField(FieldInfo field) =>
    field.IsPublic ||
    field.IsFamily ||
    field.IsFamilyOrAssembly;

string GetAttributes(MemberInfo member) =>
    string.Join(
        ",",
        member.CustomAttributes
            .Select(attribute => attribute.ToString())
            .OrderBy(attribute => attribute, StringComparer.Ordinal));

string GetParameterDefault(ParameterInfo parameter)
{
    if (!parameter.HasDefaultValue)
    {
        return "<none>";
    }

    object defaultValue = parameter.DefaultValue;

    if (defaultValue is null)
    {
        return "<null>";
    }

    return Convert.ToString(defaultValue, CultureInfo.InvariantCulture);
}

string GetParameters(ParameterInfo[] parameters) =>
    string.Join(
        ";",
        parameters.Select(parameter =>
            $"{GetTypeName(parameter.ParameterType)} {parameter.Name}" +
            $"|ATTR:{parameter.Attributes}" +
            $"|DEFAULT:{GetParameterDefault(parameter)}" +
            $"|CUSTOM:{string.Join(",", parameter.CustomAttributes.Select(attribute => attribute.ToString()).OrderBy(attribute => attribute, StringComparer.Ordinal))}"));

string GetGenericMetadata(Type[] genericArguments) =>
    string.Join(
        ";",
        genericArguments
            .Where(argument => argument.IsGenericParameter)
            .Select(argument =>
                $"{argument.Name}" +
                $"|ATTR:{argument.GenericParameterAttributes}" +
                $"|CONSTRAINTS:{string.Join(",", argument.GetGenericParameterConstraints().Select(GetTypeName).OrderBy(constraint => constraint, StringComparer.Ordinal))}"));

Assembly assembly = Assembly.LoadFrom(Environment.GetEnvironmentVariable("ASSEMBLY_PATH"));
BindingFlags flags =
    BindingFlags.Public |
    BindingFlags.NonPublic |
    BindingFlags.Instance |
    BindingFlags.Static |
    BindingFlags.DeclaredOnly;
List<string> manifestLines = [];

foreach (Type type in assembly.GetTypes().Where(IsApiType))
{
    manifestLines.Add(
        $"TYPE|{GetTypeName(type)}" +
        $"|ATTR:{type.Attributes}" +
        $"|BASE:{GetTypeName(type.BaseType)}" +
        $"|INTERFACES:{string.Join(",", type.GetInterfaces().Select(GetTypeName).OrderBy(interfaceName => interfaceName, StringComparer.Ordinal))}" +
        $"|GENERICS:{GetGenericMetadata(type.GetGenericArguments())}" +
        $"|CUSTOM:{GetAttributes(type)}");

    foreach (FieldInfo field in type.GetFields(flags).Where(IsApiField))
    {
        string constantValue = string.Empty;

        if (field.IsLiteral)
        {
            object rawConstantValue = field.GetRawConstantValue();

            if (rawConstantValue is not null)
            {
                constantValue = Convert.ToString(rawConstantValue, CultureInfo.InvariantCulture);
            }
        }

        manifestLines.Add(
            $"FIELD|{GetTypeName(type)}|{field.Name}" +
            $"|TYPE:{GetTypeName(field.FieldType)}" +
            $"|ATTR:{field.Attributes}" +
            $"|VALUE:{constantValue}" +
            $"|CUSTOM:{GetAttributes(field)}");
    }

    foreach (ConstructorInfo constructor in type.GetConstructors(flags).Where(IsApiMethod))
    {
        manifestLines.Add(
            $"CTOR|{GetTypeName(type)}" +
            $"|ATTR:{constructor.Attributes}" +
            $"|PARAMS:{GetParameters(constructor.GetParameters())}" +
            $"|CUSTOM:{GetAttributes(constructor)}");
    }

    foreach (MethodInfo method in type.GetMethods(flags).Where(IsApiMethod))
    {
        manifestLines.Add(
            $"METHOD|{GetTypeName(type)}|{method.Name}" +
            $"|ATTR:{method.Attributes}" +
            $"|RETURN:{GetTypeName(method.ReturnType)}" +
            $"|PARAMS:{GetParameters(method.GetParameters())}" +
            $"|GENERICS:{GetGenericMetadata(method.GetGenericArguments())}" +
            $"|CUSTOM:{GetAttributes(method)}");
    }

    foreach (PropertyInfo property in type.GetProperties(flags))
    {
        MethodInfo[] accessors = property.GetAccessors(true);

        if (!accessors.Any(accessor => IsApiMethod(accessor)))
        {
            continue;
        }

        manifestLines.Add(
            $"PROPERTY|{GetTypeName(type)}|{property.Name}" +
            $"|TYPE:{GetTypeName(property.PropertyType)}" +
            $"|ATTR:{property.Attributes}" +
            $"|ACCESSORS:{string.Join(",", accessors.Where(IsApiMethod).Select(accessor => $"{accessor.Name}:{accessor.Attributes}").OrderBy(accessor => accessor, StringComparer.Ordinal))}" +
            $"|PARAMS:{GetParameters(property.GetIndexParameters())}" +
            $"|CUSTOM:{GetAttributes(property)}");
    }

    foreach (EventInfo eventInfo in type.GetEvents(flags))
    {
        MethodInfo addMethod = eventInfo.GetAddMethod(true);
        MethodInfo removeMethod = eventInfo.GetRemoveMethod(true);

        if (!IsApiMethod(addMethod) && !IsApiMethod(removeMethod))
        {
            continue;
        }

        manifestLines.Add(
            $"EVENT|{GetTypeName(type)}|{eventInfo.Name}" +
            $"|TYPE:{GetTypeName(eventInfo.EventHandlerType)}" +
            $"|ATTR:{eventInfo.Attributes}" +
            $"|ADD:{addMethod?.Attributes}" +
            $"|REMOVE:{removeMethod?.Attributes}" +
            $"|CUSTOM:{GetAttributes(eventInfo)}");
    }
}

foreach (string manifestLine in manifestLines.OrderBy(line => line, StringComparer.Ordinal))
{
    Console.WriteLine(manifestLine);
}
CSHARP
)"

cleanup()
{
    rm -rf "${BASELINE_DIRECTORY}" "${REPORT_DIRECTORY}"
}

require_command()
{
    local COMMAND_NAME="${1}"

    if ! command -v "${COMMAND_NAME}" >/dev/null 2>&1
    then
        echo "[ERROR] Required command '${COMMAND_NAME}' is unavailable." >&2
        exit 1
    fi
}

generate_api_manifest()
{
    local ASSEMBLY_PATH_VALUE="${1}"
    local OUTPUT_PATH="${2}"

    ASSEMBLY_PATH="${ASSEMBLY_PATH_VALUE}" \
        dotnet script eval "${API_MANIFEST_CODE}" > "${OUTPUT_PATH}"
}

generate_data_content_manifest()
{
    local ROOT_DIRECTORY="${1}"
    local OUTPUT_PATH="${2}"

    {
        find "${ROOT_DIRECTORY}/OpenRS/Data" -type f
        find "${ROOT_DIRECTORY}/OpenRS/Content" \
            -type f \
            -not -path '*/bin/*' \
            -not -path '*/obj/*'
    } | LC_ALL=C sort | while IFS= read -r FILE_PATH
    do
        local RELATIVE_PATH="${FILE_PATH#"${ROOT_DIRECTORY}/"}"
        printf '%s  %s\n' "$(git hash-object "${FILE_PATH}")" "${RELATIVE_PATH}"
    done > "${OUTPUT_PATH}"
}

generate_output_manifest()
{
    local ROOT_DIRECTORY="${1}"
    local OUTPUT_PATH="${2}"

    for FILE_PATH in "${ROOT_DIRECTORY}/OpenRS/bin/Release/net10.0/"*
    do
        if [[ -f "${FILE_PATH}" ]]
        then
            basename "${FILE_PATH}"
        fi
    done | LC_ALL=C sort > "${OUTPUT_PATH}"
}

compare_manifests()
{
    local MANIFEST_NAME="${1}"
    local BASELINE_PATH="${2}"
    local CURRENT_PATH="${3}"

    if ! diff -u "${BASELINE_PATH}" "${CURRENT_PATH}"
    then
        echo "[ERROR] ${MANIFEST_NAME} differs from HEAD." >&2
        exit 1
    fi

    printf '%s: compatible (%s records, hash %s)\n' \
        "${MANIFEST_NAME}" \
        "$(wc -l < "${CURRENT_PATH}")" \
        "$(git hash-object "${CURRENT_PATH}")"
}

trap cleanup EXIT

require_command 'basename'
require_command 'diff'
require_command 'dotnet'
require_command 'find'
require_command 'git'
require_command 'sort'
require_command 'tar'

git -C "${WORKSPACE_DIRECTORY}" archive HEAD | tar -x -C "${BASELINE_DIRECTORY}"

dotnet build "${BASELINE_DIRECTORY}/OpenRS/OpenRS.csproj" -c Release >/dev/null
dotnet build "${WORKSPACE_DIRECTORY}/OpenRS/OpenRS.csproj" -c Release >/dev/null

generate_api_manifest \
    "${BASELINE_DIRECTORY}/OpenRS/bin/Release/net10.0/OpenRS.dll" \
    "${REPORT_DIRECTORY}/api-baseline.txt"
generate_api_manifest \
    "${WORKSPACE_DIRECTORY}/OpenRS/bin/Release/net10.0/OpenRS.dll" \
    "${REPORT_DIRECTORY}/api-current.txt"

generate_data_content_manifest \
    "${BASELINE_DIRECTORY}" \
    "${REPORT_DIRECTORY}/data-content-baseline.txt"
generate_data_content_manifest \
    "${WORKSPACE_DIRECTORY}" \
    "${REPORT_DIRECTORY}/data-content-current.txt"

generate_output_manifest \
    "${BASELINE_DIRECTORY}" \
    "${REPORT_DIRECTORY}/output-baseline.txt"
generate_output_manifest \
    "${WORKSPACE_DIRECTORY}" \
    "${REPORT_DIRECTORY}/output-current.txt"

compare_manifests \
    'Public API manifest' \
    "${REPORT_DIRECTORY}/api-baseline.txt" \
    "${REPORT_DIRECTORY}/api-current.txt"
compare_manifests \
    'Data/content manifest' \
    "${REPORT_DIRECTORY}/data-content-baseline.txt" \
    "${REPORT_DIRECTORY}/data-content-current.txt"
compare_manifests \
    'Release output manifest' \
    "${REPORT_DIRECTORY}/output-baseline.txt" \
    "${REPORT_DIRECTORY}/output-current.txt"