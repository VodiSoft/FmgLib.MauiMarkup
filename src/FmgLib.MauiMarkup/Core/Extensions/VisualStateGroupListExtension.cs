// Portions of this file incorporate and extend code originally from Sharp.UI.
// Copyright (c) 2022 Pawel Krzywdzinski
// Licensed under the MIT License. See THIRD-PARTY-NOTICES for details.

namespace FmgLib.MauiMarkup;

public static class VisualStateGroupListExtension
{
    public static VisualStateGroup GetCommonStatesVisualStateGroup(this VisualStateGroupList self)
    {
        VisualStateGroup visualStateGroup = self.FirstOrDefault((VisualStateGroup e) => e.Name.Equals("CommonStates"));
        if (visualStateGroup == null)
        {
            visualStateGroup = new VisualStateGroup
            {
                Name = "CommonStates"
            };
            self.Add(visualStateGroup);
        }

        return visualStateGroup;
    }

    public static void Add(this VisualStateGroupList self, VisualState visualState)
    {
        self.GetCommonStatesVisualStateGroup().States.Add(visualState);
    }
}
