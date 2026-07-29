// Portions of this file incorporate and extend code originally from Sharp.UI.
// Copyright (c) 2022 Pawel Krzywdzinski
// Licensed under the MIT License. See THIRD-PARTY-NOTICES for details.

namespace FmgLib.MauiMarkup;

public static class StyleExtension
{
    public static VisualStateGroupList GetVisualStateGroupList(this Style self)
    {
        VisualStateGroupList visualStateGroupList = null;
        Setter setter = self.Setters.FirstOrDefault((Setter e) => e.Property == VisualStateManager.VisualStateGroupsProperty);
        if (setter != null)
        {
            visualStateGroupList = setter.Value as VisualStateGroupList;
        }

        if (visualStateGroupList == null)
        {
            visualStateGroupList = new VisualStateGroupList();
            Setter item = new Setter
            {
                Property = VisualStateManager.VisualStateGroupsProperty,
                Value = visualStateGroupList
            };
            self.Setters.Add(item);
        }

        return visualStateGroupList;
    }
}
