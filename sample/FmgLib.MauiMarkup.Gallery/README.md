# FmgLib.MauiMarkup Gallery

The reference sample: **24 pages, one per feature**, built entirely with FmgLib.MauiMarkup. There is no
XAML anywhere in this project — not even in `App`, the shell, or the styles.

```bash
dotnet build sample/FmgLib.MauiMarkup.Gallery/FmgLib.MauiMarkup.Gallery.csproj -f net10.0-ios
```

Targets `net10.0-android`, `net10.0-ios`, `net10.0-maccatalyst` and `net10.0-windows`, and references the
library by project reference, so an edit in `src/` shows up here on the next build.

## What is in it

| Category | Pages |
|---|---|
| **Fundamentals** | Fluent Properties · Events · Assign & References · Custom Extensions |
| **Layout** | Layout & Alignment · Grid · Responsive & Adaptive |
| **Data** | Data Binding · MultiBinding · Compiled Bindings · Collections & Templates |
| **Interaction** | Gestures · Behaviors · Triggers · SwipeView |
| **Appearance** | Styling · Visual States · Animations · Shapes & Paths · Gradients & Brushes · Formatted Text |
| **Platform** | Theming · Localization · Hot Reload |

Each page shows the running feature **and the markup that produced it**, side by side, because the code is
the point.

## How it is organised

```
Theme/          AppColors, AppStyles, Ui — the design system
Controls/       DemoPage (chrome + hot reload), Demo (sections, stages, code blocks), behaviors
Models/         DemoCatalog (the single source of truth), DemoViewModel
Demos/          One file per page
HomePage.cs     Searchable, filterable, reflowing card grid
```

**Adding a demo is one entry in `DemoCatalog.All`** — the home grid renders it and the shell registers its
route from the same list.

## The parts worth stealing

- **`Theme/Ui.cs`** — the whole look of the app in ~120 lines of composition shorthands
  (`.Card()`, `.Stage()`, `.Heading()`, `.Muted()`). This is the Custom Extension Methods pattern applied
  to a real app, and it is why the demo pages read as intent rather than as colours and paddings.
- **`Controls/DemoPage.cs`** — responsiveness solved once: the body is capped at a readable width and
  centred, and padding tightens on phones through the idiom builder.
- **`HomePage.SpanForWidth`** — column count from the actual window width rather than the device idiom, so
  a desktop window dragged narrow reflows exactly like a phone.
- **Every colour is an `OnLight/OnDark` builder** — the theme toggle in the hero repaints the running app
  without rebuilding a single page.

## Responsiveness

Two complementary approaches, both demonstrated:

- **Measured** — `HomePage` recalculates `GridItemsLayout.Span` on `SizeChanged` (1 → 4 columns), which
  also keeps the list virtualized.
- **Declarative** — the Responsive page uses a wrapping `FlexLayout` (no breakpoints at all) and an
  `AdaptiveTrigger` visual state, alongside the idiom and platform value builders.
