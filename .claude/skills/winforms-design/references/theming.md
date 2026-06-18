# Theming, Color & Dark Mode

Color is where a form most easily looks cheap or polished. The rules: neutral surfaces, one accent, real contrast, and yield to the system in high-contrast mode.

## Table of contents
- Neutral palette + accent
- Contrast targets
- Styling surfaces (kill the gray slab)
- Dark mode (.NET 9+, experimental)
- High-contrast mode
- When a UI library earns its place

## Neutral palette + accent

Build from neutral surfaces and reserve a single accent for the primary action.

**Light palette (starting point):**
- App background / window: `#FFFFFF` to `#FAFAFA`
- Surface / card: `#FFFFFF` with a `#E5E7EB` 1px border
- Subtle fill (hover, zebra rows): `#F3F4F6`
- Primary text: `#1F2937` (near-black, not pure black)
- Secondary text: `#6B7280`
- Border / divider: `#E5E7EB`
- Accent (primary action): one brand color, e.g. `#2563EB`; accent text on it: `#FFFFFF`
- Danger: `#DC2626` · Success: `#16A34A` · Warning: `#D97706`

Use the accent only for the primary button and a few selected/active affordances. Everything else is neutral. A second or third accent dilutes the hierarchy.

Centralize colors in one static class so light/dark and rebranding are one-line changes:

```csharp
internal static class Palette
{
    public static Color Surface      => Theme.IsDark ? Color.FromArgb(0x1E,0x1E,0x1E) : Color.White;
    public static Color SurfaceAlt   => Theme.IsDark ? Color.FromArgb(0x2A,0x2A,0x2A) : Color.FromArgb(0xF3,0xF4,0xF6);
    public static Color Border       => Theme.IsDark ? Color.FromArgb(0x3A,0x3A,0x3A) : Color.FromArgb(0xE5,0xE7,0xEB);
    public static Color TextPrimary  => Theme.IsDark ? Color.FromArgb(0xEA,0xEA,0xEA) : Color.FromArgb(0x1F,0x29,0x37);
    public static Color TextSecondary=> Theme.IsDark ? Color.FromArgb(0x9A,0x9A,0x9A) : Color.FromArgb(0x6B,0x72,0x80);
    public static Color Accent       => Color.FromArgb(0x25,0x63,0xEB);
    public static Color OnAccent     => Color.White;
}
```

## Contrast targets

- Body text vs its background: **4.5:1** minimum (WCAG AA).
- Large text (≥18pt, or ≥14pt bold) and meaningful UI affordances/icons: **3:1** minimum.
- Don't rely on color alone to convey state (error, success) — pair with an icon, text, or border.
- Verify with a real contrast calculation, not by eye; near-misses (e.g. gray `#9CA3AF` on white ≈ 2.6:1) fail and look washed out.

## Styling surfaces (kill the gray slab)

The default `SystemColors.Control` gray is the single biggest "old" tell. Fix it:
- Set the `Form.BackColor` to a near-white surface; set content panels to `Surface`.
- Give cards/sections a `Surface` background with a 1px `Border` (custom paint or a bordered `Panel`).
- Replace `Fixed3D` borders with flat 1px borders (see controls.md).
- Use `SurfaceAlt` for zebra striping, hover fills, and inset regions — subtle, not heavy.

## Dark mode (.NET 9+, experimental)

Dark mode is a real differentiator but it is **experimental** in .NET 9 (goal to finalize in a later release) and some controls don't fully theme. Treat it as opt-in polish, not a guarantee.

**Enable it.** The API is guarded by the `WFO5001` diagnostic. Suppress it project-wide or locally:

```xml
<!-- .csproj -->
<PropertyGroup>
  <NoWarn>$(NoWarn);WFO5001</NoWarn>
</PropertyGroup>
```

```csharp
[STAThread]
static void Main()
{
    Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
    ApplicationConfiguration.Initialize();

#pragma warning disable WFO5001
    Application.SetColorMode(LoadUserPreference()); // SystemColorMode.System by default
#pragma warning restore WFO5001

    Application.Run(new MainForm());
}
```

`SystemColorMode` values:
- `Classic` — light mode, the legacy default.
- `System` — follow the Windows light/dark setting (good default).
- `Dark` — force dark.

`Application.ColorMode` reports the active mode; `Application.SystemColorMode` reports what Windows is set to (Windows 11+).

**Best practices:**
- Default to `System` (or light), and offer the user a Light / System / Dark choice, persisted (e.g. appsettings.json) and applied at startup.
- **Set the mode at startup, before `Application.Run`.** Switching at runtime is buggy (stale `SystemColors` brush caches mean some controls — toolstrips especially — don't recolor). If you offer a runtime toggle, prompt to restart.
- Requires **Windows 11** for full effect; on Windows 10 it degrades.

**Controls that misbehave in dark mode (known, plan around them):**
- `Button` with `FlatStyle.Standard` (the default) looks wrong — switch to `FlatStyle.System` or fully style a `FlatStyle.Flat` button yourself.
- `DateTimePicker`, `MonthCalendar`, `TabControl` — incomplete theming; consider owner-draw or a styled alternative.
- `DataGridView` headers don't follow automatically:
  ```csharp
  grid.EnableHeadersVisualStyles = false;
  grid.ColumnHeadersDefaultCellStyle.BackColor = Palette.SurfaceAlt;
  grid.ColumnHeadersDefaultCellStyle.ForeColor = Palette.TextPrimary;
  ```
- Title bar: dark mode tints it via the DWM; if you need manual control, `DwmSetWindowAttribute` with `DWMWA_USE_IMMERSIVE_DARK_MODE` is the underlying mechanism.

## High-contrast mode

When Windows high-contrast is on, your custom palette should **yield** to system colors so the OS accessibility settings win.

```csharp
if (SystemInformation.HighContrast)
{
    // use SystemColors.* and skip custom backgrounds/borders
}
```

Check `SystemInformation.HighContrast` before applying custom colors, and listen to `SystemEvents.UserPreferenceChanged` if you support live changes. Never hardcode colors in a way that defeats high-contrast.

## When a UI library earns its place

Stay BCL-pure by default — it's free, portable, and the right teaching baseline. A library is justified when you need, app-wide, things the BCL can't express cleanly: rounded surfaces and shadows, smooth animations, a full swappable theme engine, or a modern control set (cards, toggles, modern data grids). When that point comes, evaluate by: active maintenance, license, DPI correctness, and accessibility support — a library that breaks DPI or a11y is worse than styled BCL. Until then, owner-draw the one or two custom touches you need rather than taking a dependency.
