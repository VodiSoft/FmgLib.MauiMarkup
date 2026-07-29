// Portions of this file incorporate and extend code originally from Sharp.UI.
// Copyright (c) 2022 Pawel Krzywdzinski
// Licensed under the MIT License. See THIRD-PARTY-NOTICES for details.

namespace FmgLib.MauiMarkup;



public static partial class VisualStateExtension
{
    public static VisualState Name(this VisualState self,
        string name)
    {
        self.Name = name;
        return self;
    }
    
    public static VisualState Setters(this VisualState self,
        IList<Setter> setters)
    {
        foreach (var item in setters)
            self.Setters.Add(item);
        return self;
    }

    public static VisualState Setters(this VisualState self,
        params Setter[] setters)
    {
        foreach (var item in setters)
            self.Setters.Add(item);
        return self;
    }

    public static VisualState Setters(this VisualState self,
        Func<Setter[]> configure)
    {
        var setters = configure();
        foreach (var item in setters)
            self.Setters.Add(item);
        return self;
    }

    public static T TargetName<T>(this T self, string targetName)
            where T : IList<Setter>
    {
        foreach (var setter in self)
            setter.TargetName = targetName;
        return self;
    }

    public static VisualState StateTriggers(this VisualState self,
        IList<StateTriggerBase> stateTriggers)
    {
        foreach (var item in stateTriggers)
            self.StateTriggers.Add(item);
        return self;
    }

    public static VisualState StateTriggers(this VisualState self,
        params StateTriggerBase[] stateTriggers)
    {
        foreach (var item in stateTriggers)
            self.StateTriggers.Add(item);
        return self;
    }

    public static VisualState StateTriggers(this VisualState self,
        Func<StateTriggerBase[]> configure)
    {
        var stateTriggers = configure();
        foreach (var item in stateTriggers)
            self.StateTriggers.Add(item);
        return self;
    }
    
    public static VisualState TargetType(this VisualState self,
        Type targetType)
    {
        self.TargetType = targetType;
        return self;
    }
    
}
