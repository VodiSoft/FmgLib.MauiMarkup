using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace FmgLib.MauiMarkup.Generator.Extensions;

public partial class ExtensionGenerator
{
    void GenerateEventMethod(ISymbol @event)
    {
        var eventSymbol = (IEventSymbol)@event;

        var existInBases = false;
        if (mainSymbol.BaseType is null)
        {
            return;
        }

        Helpers.LoopDownToObject(mainSymbol.BaseType, type =>
        {
            existInBases = (type
            .GetMembers()
            .FirstOrDefault(e =>
            e.Kind == SymbolKind.Event &&
            e.DeclaredAccessibility == Accessibility.Public &&
            e.Name.Equals(eventSymbol.Name, StringComparison.Ordinal)) != null);

            return existInBases;
        });

        if (!existInBases && !Helpers.NotGenerateList.Contains(eventSymbol.Name))
        {
            if (mainSymbol.IsSealed)
            {
                GenerateEventMethodHandler_Sealed(eventSymbol);
                GenerateEventMethodNoArgs_Sealed(eventSymbol);
            }
            else
            {
                GenerateEventMethodHandler_Normal(eventSymbol);
                GenerateEventMethodNoArgs_Normal(eventSymbol);
            }
            isGeneratedExtension = true;
        }
    }


    void GenerateEventMethodHandler_Sealed(IEventSymbol eventSymbol)
    {
        builder.Append($@"
    public static {mainSymbol.ToQualifiedName()} On{eventSymbol.Name}(this {mainSymbol.ToQualifiedName()} self, {eventSymbol.Type.ToQualifiedName()} handler)
    {{
        self.{eventSymbol.Name} += handler;
        return self;
    }}
    ");
    }

    void GenerateEventMethodHandler_Normal(IEventSymbol eventSymbol)
    {
        builder.Append($@"
    public static T On{eventSymbol.Name}<T>(this T self, {eventSymbol.Type.ToQualifiedName()} handler)
        where T : {mainSymbol.ToQualifiedName()}
    {{
        self.{eventSymbol.Name} += handler;
        return self;
    }}
    ");
    }

    void GenerateEventMethodNoArgs_Sealed(IEventSymbol eventSymbol)
    {
        var invokeMethod = ((INamedTypeSymbol)eventSymbol.Type).DelegateInvokeMethod;
        var parameterCount = invokeMethod?.Parameters.Length ?? 0;
        if (parameterCount <= 2)
            builder.Append($@"
    public static {mainSymbol.ToQualifiedName()} On{eventSymbol.Name}(this {mainSymbol.ToQualifiedName()} self, global::System.Action<{mainSymbol.ToQualifiedName()}> action)
    {{
        {(parameterCount == 2 ? $"self.{eventSymbol.Name} += (o, arg) => action(self);" : parameterCount == 1 ? $"self.{eventSymbol.Name} += (o) => action(self);" : parameterCount == 0 ? $"self.{eventSymbol.Name} += () => action(self);" : string.Empty)}
        return self;
    }}
        ");
    }

    void GenerateEventMethodNoArgs_Normal(IEventSymbol eventSymbol)
    {
        var invokeMethod = ((INamedTypeSymbol)eventSymbol.Type).DelegateInvokeMethod;
        var parameterCount = invokeMethod?.Parameters.Length ?? 0;
        if (parameterCount <= 2)
            builder.Append($@"
    public static T On{eventSymbol.Name}<T>(this T self, global::System.Action<T> action)
        where T : {mainSymbol.ToQualifiedName()}
    {{
        {(parameterCount == 2 ? $"self.{eventSymbol.Name} += (o, arg) => action(self);" : parameterCount == 1 ? $"self.{eventSymbol.Name} += (o) => action(self);" : parameterCount == 0 ? $"self.{eventSymbol.Name} += () => action(self);" : string.Empty)}
        return self;
    }}
        ");
    }
}
