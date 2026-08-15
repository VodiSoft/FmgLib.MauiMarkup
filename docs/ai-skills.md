---
title: AI Skills
description: Ten installable skill bundles that teach AI coding agents to write correct FmgLib.MauiMarkup code.
badge: New
---

# AI Skills

Ask any AI agent to "write a MAUI page in C# markup" and you get plausible-looking code that doesn't
compile: invented method names, XAML habits transcribed literally, view models constructed inside
`Build()`, `ContentTemplate(page)` instead of `ContentTemplate(() => page)`.

The problem isn't the model — it's that the library's rules are simple but not guessable. Property
`Foo` becomes `.Foo(...)`. Event `Bar` becomes `.OnBar(...)`. `Grid.Row` drops its prefix but
`Shell.TitleColor` doesn't. `Build()` re-runs on every hot reload, so state must live in fields. None
of that can be inferred from the type system alone.

**AI Skills are ten Markdown bundles that state those rules directly.** Install them once and your
agent stops guessing.

```csharp
// what an agent writes without the skills
new Label().SetText("Hello").SetFontSize(30).HorizontalAlign("Center")   // none of this exists

// what it writes with them
new Label().Text("Hello").FontSize(30).CenterHorizontal()
```

They use the [Agent Skills](https://code.claude.com/docs/en/skills) format — plain Markdown with a YAML
header — so they work with Claude Code, the Claude apps, the Agent SDK, and any agent that can read a
`SKILL.md` file. Everything is MIT licensed and versioned alongside the library, so an API change and
its skill update ship in the same commit.

## Install

### Automatic

Tell your agent:

> Fetch https://mauimarkup.fmglib.dev/llms.txt and install the FmgLib.MauiMarkup AI skills.

It will find this page, read the catalog below and download each skill into place.

### Manual

Each skill is a folder containing a `SKILL.md`. Drop it in one of two places:

| Scope | Location |
|---|---|
| Personal — available in every project | `~/.claude/skills/<skill-name>/SKILL.md` |
| Project — committed with the repository, shared with the team | `<repo>/.claude/skills/<skill-name>/SKILL.md` |

The core skill also ships a `references/` folder; keep the relative paths so its internal links resolve.

```bash
# core skill, straight from GitHub
mkdir -p ~/.claude/skills/mauimarkup/references
BASE=https://raw.githubusercontent.com/VodiSoft/FmgLib.MauiMarkup/master/skills/mauimarkup
curl -fsSL $BASE/SKILL.md -o ~/.claude/skills/mauimarkup/SKILL.md
for f in cheatsheet bindings layout styling-theming pitfalls; do
  curl -fsSL $BASE/references/$f.md -o ~/.claude/skills/mauimarkup/references/$f.md
done
```

Every other skill is a single file:

```bash
NAME=mauimarkup-mvvm     # any name from the catalog
mkdir -p ~/.claude/skills/$NAME
curl -fsSL https://raw.githubusercontent.com/VodiSoft/FmgLib.MauiMarkup/master/skills/$NAME/SKILL.md \
     -o ~/.claude/skills/$NAME/SKILL.md
```

Committing the folder to `<repo>/.claude/skills/` is the recommended option for teams: every developer
and every CI agent then works from the same instructions.

## The ten skills

Source for all of them:
[`skills/`](https://github.com/VodiSoft/FmgLib.MauiMarkup/tree/master/skills) ·
raw base URL `https://raw.githubusercontent.com/VodiSoft/FmgLib.MauiMarkup/master/skills/<name>/SKILL.md`

| Skill | Teaches |
|---|---|
| **`mauimarkup`** *(required)* | The fluent model, page skeleton, the four property overloads, layout, events, `Assign`/`InvokeOnElement`, name derivation, `Build()` discipline. Ships a five-file `references/` bundle: API cheatsheet, bindings, layout tables, styling & theming, pitfalls |
| `mauimarkup-xaml-migration` | Per-page migration procedure, a 30-row XAML→C# mapping table, and the constructs that need judgement rather than translation — `x:Reference` ordering, `StaticResource`, converters, `RelativeSource` |
| `mauimarkup-mvvm` | `FmgLibContentPage<TViewModel>`, typed `BindingContext`, compiled `Getter`/`Setter` bindings, commands, dependency injection, CommunityToolkit.Mvvm |
| `mauimarkup-shell` | Shell, `FlyoutItem`, `Tab`, `TabBar`, `ContentTemplate` lambdas, flyout templates, per-page Shell attached properties, routes, windows, menu bars |
| `mauimarkup-collections` | `ItemsSource`/`ItemTemplate`, template selectors, item layouts, `EmptyView`, infinite scroll, pull-to-refresh, `BindableLayout` and when *not* to use it |
| `mauimarkup-styling` | `Style<T>`, resource organization, `AppThemeBinding` dark mode, visual states, triggers, gradients, shadows, `Animate…To` |
| `mauimarkup-localization` | JSON and RESX setup, `Translate`/`TranslateFormat`, live culture switching, fallback chains, missing-key policies, RTL |
| `mauimarkup-thirdparty` | `[MauiMarkup]`, `[MauiMarkupAttachedProp]`, automatic generator mode, base-class generation, the `New` suffix rule, deliberately skipped members |
| `mauimarkup-hotreload` | `IFmgLibHotReload`, handler options, `dotnet watch` vs. IDE channels, reload-safe `Build()`, the full troubleshooting matrix |
| `mauimarkup-review` | Nine audit passes with ready-to-run ripgrep queries, a severity model and a reporting format |

Installing all ten is fine — an agent reads a skill's body only when the task matches its description,
so unused skills cost nothing.

### Recommended sets

| You are | Install |
|---|---|
| Starting a new app | `mauimarkup` + `mauimarkup-shell` + `mauimarkup-mvvm` + `mauimarkup-hotreload` |
| Migrating an existing XAML app | `mauimarkup` + `mauimarkup-xaml-migration` + `mauimarkup-styling` |
| Building a data-heavy app | `mauimarkup` + `mauimarkup-mvvm` + `mauimarkup-collections` |
| Shipping to several markets | add `mauimarkup-localization` |
| Using Syncfusion / UraniumUI / SkiaSharp / ZXing | add `mauimarkup-thirdparty` |
| Cleaning up an inherited codebase | `mauimarkup` + `mauimarkup-review` |

## What changes in practice

A few of the corrections the skills encode — each one a mistake agents make reliably without them:

| Without skills | With skills |
|---|---|
| Invents `.SetText()`, `.HorizontalAlign()` | Derives `.Text()`, `.CenterHorizontal()` from the property name |
| Emits a `.xaml` + `.xaml.cs` pair | One `.cs` file, no `InitializeComponent()` |
| `new MyViewModel()` inside `Build()` | View model in a constructor field — state survives hot reload |
| `.ContentTemplate(new HomePage())` | `.ContentTemplate(() => new HomePage())` |
| `.TextColor(isDark ? white : black)` | `.TextColor(e => e.OnLight(black).OnDark(white))` — a live theme binding |
| `e.Path("UserName")` everywhere | `e.Getter(static (VM vm) => vm.UserName)` — compile-checked |
| `BindableLayout` for a 5000-item list | `CollectionView`, because only it virtualizes |
| Adds `builder.UseFmgLibMauiMarkup()` | Knows no registration call exists |
| Hand-writes extensions for a Syncfusion control | `[MauiMarkup(typeof(SfButton))]` and lets the generator do it |

## Verifying the install

Ask for something the skills make unambiguous:

> Write a MauiMarkup login page with an email entry, a password entry, and a submit button that stays
> disabled until both are filled.

A correct answer is a single `.cs` file implementing `IFmgLibHotReload`, building the tree in `Build()`,
capturing the entries with `.Assign(out var …)`, and containing no XAML at all.

## Keeping them honest

The skills live in the library repository, so they are reviewed with the code they describe. If one
teaches something the library no longer does, please
[open an issue](https://github.com/VodiSoft/FmgLib.MauiMarkup/issues) — an agent confidently repeating
stale guidance is worse than an agent with no skill at all.

## Related Topics

- [Getting Started](getting-started.md) — install the library the skills describe
- [From XAML to C#](xaml-to-csharp.md) — the human version of the migration skill
- [Tips & Troubleshooting](tips-and-troubleshooting.md) — the human version of the pitfalls reference
