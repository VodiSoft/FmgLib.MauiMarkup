// Portions of this file incorporate and extend code originally from Sharp.UI.
// Copyright (c) 2022 Pawel Krzywdzinski
// Licensed under the MIT License. See THIRD-PARTY-NOTICES for details.

namespace FmgLib.MauiMarkup;

public interface IPropertySettersBuilder<T>
{
    PropertySettersContext<T> Context { get; set; }

    bool Build()
    {
        return false;
    }
}
