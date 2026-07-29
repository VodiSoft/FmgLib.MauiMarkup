// Portions of this file incorporate and extend code originally from Sharp.UI.
// Copyright (c) 2022 Pawel Krzywdzinski
// Licensed under the MIT License. See THIRD-PARTY-NOTICES for details.

namespace FmgLib.MauiMarkup;

public static class PropertyBindingBuilderExtension
{
    public static PropertyBindingBuilder<bool> Negate(this PropertyBindingBuilder<bool> self)
    {
        return self.Convert<bool>(e => !e).ConvertBack<bool>(e => !e);
    }
}
