# FmgLib.MauiMarkup — AI Skills

Ten focused instruction bundles that teach an AI coding agent how to use FmgLib.MauiMarkup properly:
the fluent API model, the naming rules it can derive instead of guess, the `Build()` lifecycle, the
binding builder, the source generator, and the mistakes that produce silent runtime failures.

Without them, agents write plausible-looking MAUI markup that doesn't compile — invented method names,
XAML habits transcribed literally, view models constructed inside `Build()`. With them, agents derive
the right method name from the property name and get the lifecycle right the first time.

Skills are plain Markdown with YAML frontmatter (the [Agent Skills](https://code.claude.com/docs/en/skills)
format), so they work with Claude Code, the Claude apps, the Agent SDK, and any agent that can read a
`SKILL.md`.

## Install

### Automatic (recommended)

Tell your agent:

> Fetch https://mauimarkup.fmglib.dev/llms.txt and install the FmgLib.MauiMarkup AI skills.

It will discover the catalog and download each skill into place.

### Manual

Copy the skill folders you want into your skills directory — keep the folder name and the internal
`references/` paths so relative links keep working.

| Scope | Location |
|---|---|
| Personal (all projects) | `~/.claude/skills/<skill-name>/SKILL.md` |
| One project (committed with the repo) | `<repo>/.claude/skills/<skill-name>/SKILL.md` |

```bash
# personal install of the core skill, from a clone of this repository
mkdir -p ~/.claude/skills
cp -R skills/mauimarkup ~/.claude/skills/
```

```bash
# core skill straight from GitHub, no clone
mkdir -p ~/.claude/skills/mauimarkup/references
BASE=https://raw.githubusercontent.com/VodiSoft/FmgLib.MauiMarkup/master/skills/mauimarkup
curl -fsSL $BASE/SKILL.md -o ~/.claude/skills/mauimarkup/SKILL.md
for f in cheatsheet bindings layout styling-theming pitfalls; do
  curl -fsSL $BASE/references/$f.md -o ~/.claude/skills/mauimarkup/references/$f.md
done
```

Every other skill is a single file:

```bash
NAME=mauimarkup-mvvm     # any name from the catalog below
mkdir -p ~/.claude/skills/$NAME
curl -fsSL https://raw.githubusercontent.com/VodiSoft/FmgLib.MauiMarkup/master/skills/$NAME/SKILL.md \
     -o ~/.claude/skills/$NAME/SKILL.md
```

## Catalog

| Skill | Install when | Contents |
|---|---|---|
| **`mauimarkup`** *(required)* | Always — every other skill builds on it | The fluent model, page skeleton, the four property overloads, layout, events, `Assign`/`InvokeOnElement`, name derivation, plus a five-file `references/` bundle (cheatsheet, bindings, layout, styling & theming, pitfalls) |
| `mauimarkup-xaml-migration` | Porting an existing XAML app | Per-page procedure, a 30-row XAML→C# mapping table, and the constructs that need judgement (`x:Reference` ordering, `StaticResource`, converters, `RelativeSource`) |
| `mauimarkup-mvvm` | Any app with view models | `FmgLibContentPage<TViewModel>`, typed `BindingContext`, compiled `Getter`/`Setter` bindings, commands, DI, CommunityToolkit.Mvvm |
| `mauimarkup-shell` | Building the app skeleton | Shell/FlyoutItem/Tab/TabBar, `ContentTemplate` lambdas, flyout templates, per-page Shell properties, routes, windows, menus |
| `mauimarkup-collections` | Lists, feeds, carousels, card grids | `ItemsSource`/`ItemTemplate`, template selectors, item layouts, `EmptyView`, infinite scroll, pull-to-refresh, `BindableLayout`, virtualization rules |
| `mauimarkup-styling` | Design systems, dark mode, animation | `Style<T>`, resource organization, `AppThemeBinding`, visual states, triggers, gradients, shadows, `Animate…To` |
| `mauimarkup-localization` | Multi-language apps | JSON and RESX setup, `Translate`/`TranslateFormat`, live culture switching, fallback, missing-key policies, RTL |
| `mauimarkup-thirdparty` | Syncfusion, UraniumUI, SkiaSharp, ZXing, InputKit, DevExpress, custom controls | `[MauiMarkup]`, `[MauiMarkupAttachedProp]`, automatic mode, base-class generation, the `New` suffix rule, skipped members |
| `mauimarkup-hotreload` | Setting up or debugging the dev loop | `IFmgLibHotReload`, handler options, `dotnet watch` vs IDE channels, reload-safe `Build()`, full troubleshooting matrix |
| `mauimarkup-review` | Auditing an existing codebase | Nine review passes with ripgrep queries, severity model and a reporting format |

## Recommended sets

| You are | Install |
|---|---|
| Starting a new app | `mauimarkup` + `mauimarkup-shell` + `mauimarkup-mvvm` + `mauimarkup-hotreload` |
| Migrating from XAML | `mauimarkup` + `mauimarkup-xaml-migration` + `mauimarkup-styling` |
| Building a data-heavy app | `mauimarkup` + `mauimarkup-mvvm` + `mauimarkup-collections` |
| Shipping to several markets | add `mauimarkup-localization` |
| Using a third-party control kit | add `mauimarkup-thirdparty` |
| Cleaning up an inherited codebase | `mauimarkup` + `mauimarkup-review` |

Installing all ten is fine — agents load a skill's body only when the task matches its description.

## Verifying the install

Ask your agent something the skills make unambiguous, for example:

> Write a MauiMarkup login page with an email entry, a password entry and a submit button that is
> disabled until both are filled.

A correctly loaded set produces a single `.cs` file with `IFmgLibHotReload`, `Build()`, `.Assign(out …)`
and no XAML anywhere.

## Contributing

The skills are versioned with the library, so an API change and its skill update land in the same
commit. If a skill teaches something that no longer matches the library, please
[open an issue](https://github.com/VodiSoft/FmgLib.MauiMarkup/issues) — an agent repeating stale
guidance is worse than one with no skill at all.

Full documentation: <https://mauimarkup.fmglib.dev> · Licensed MIT, like the library.
