---
name: winforms-design
description: "Design and polish visually excellent, modern-looking Windows Forms (WinForms) desktop UIs on .NET or .NET Framework. Use this whenever building, laying out, restyling, or reviewing a WinForms Form or UserControl — layout and resizing behaviour, DPI scaling, spacing, typography, color and theming, dark mode, control styling, states and feedback, or accessibility. Make sure to use this skill even when the user only says things like 'make this form look good/modern/chulo', 'mejora la UI de esta ventana', 'diseña este formulario', 'que escale bien con DPI', 'pon dark mode', 'esta app parece de 2003', 'haz una app chula con WinForms', or pastes a .Designer.cs / form screenshot and wants it to look more professional — even if they don't say the word 'design'. This skill covers the VISUAL/UX layer only; it complements (does not replace) the dotnet-winforms architecture skill, which handles MVP, data binding, async and migration."
compatibility: "Requires a Windows Forms project on .NET (8/9/10+) or .NET Framework 4.x. Dark mode features require .NET 9+ on Windows 11."
metadata:
  version: "1.0.0"
---

# WinForms Design

Make Windows Forms apps look intentional, modern and professional instead of looking like a default 2003 LOB form. WinForms ships with ugly defaults (battleship gray, 3D borders, Tahoma-ish fonts, absolute pixel positioning). Almost everything that makes a form look "chulo" is about *overriding those defaults consistently* — and about layout that survives resizing and DPI. This skill is the visual/UX layer. For architecture (MVP, binding, async, migration) defer to the `dotnet-winforms` skill; the two are designed to run together.

## Trigger On

- building or restyling a `Form` / `UserControl` and wanting it to look modern and polished
- a form that "looks old", cramped, misaligned, or breaks when resized or on a 4K / scaled display
- requests like "make this look good", "más chulo", "más profesional", "design this screen"
- adding dark mode, theming, an accent color, or fixing contrast/accessibility
- de-uglifying a `DataGridView`, toolbar, or dialog
- a screenshot or `.Designer.cs` handed over for a visual review

## Design Principles

Apply these as defaults; deviate only with a reason.

1. **Layout is the foundation, not decoration.** A form that looks great at design size but collapses when resized or at 150% DPI is not well designed. Resilience comes first.
2. **Consistency beats cleverness.** One spacing scale, one type scale, one accent color, one button height. Repetition is what reads as "designed".
3. **Whitespace is a feature.** Cramped forms look amateur. Let the UI breathe with generous, consistent padding and grouping by space rather than boxes.
4. **Restraint with color.** Neutral surfaces, a single accent reserved for the primary action. Color used everywhere is color used nowhere.
5. **Respect the platform.** Use Segoe UI, native control behaviours, AcceptButton/CancelButton, mnemonics, tab order. Fight the ugly defaults, not the conventions users rely on.
6. **Every state is a design state.** Hover, pressed, focused, disabled, busy, empty, error — design them, don't let them be accidents.

## Workflow

When designing a new form or improving an existing one, work in this order. Earlier steps constrain later ones, so don't skip ahead.

1. **Fix the layout skeleton first.** Replace absolute positioning with `TableLayoutPanel` / `FlowLayoutPanel`. Set `Anchor` / `Dock` so the form survives resizing. Give the form a sensible `MinimumSize`. This is the single highest-impact change — see `references/layout-dpi.md`.
2. **Make it DPI-correct.** Use `AutoScaleMode = Dpi` (or `Font`) consistently across the form and every container, enable PerMonitorV2 awareness, and remove hardcoded pixel sizes. A crisp form at 100% that blurs at 150% fails. See `references/layout-dpi.md`.
3. **Apply the spacing grid.** Use an 8px spacing scale (4px for tight pairs): consistent `Margin` and `Padding`, form padding of 12–16px, generous gaps between logical groups. Align everything to invisible columns.
4. **Set the type system.** Set `Font` once on the `Form` (Segoe UI, 9pt base) and let controls inherit. Use a small, deliberate type scale; use weight and size for hierarchy, not random bolding. Never mix font families.
5. **Apply the color/theme.** Neutral surface palette, one accent for the primary action, WCAG 4.5:1 contrast minimum. Style the primary button distinctly. Add light/system/dark theming if wanted. See `references/theming.md`.
6. **Polish controls.** Flatten harsh 3D borders, style buttons, de-uglify the `DataGridView`, fix toolbars. See `references/controls.md` for per-control recipes.
7. **Design the states.** Hover/pressed via `FlatAppearance`, clear disabled styling, validation via `ErrorProvider` (never a `MessageBox` storm), busy state (disable + wait cursor + `Progress<T>`), and empty states for lists/grids.
8. **Accessibility and finishing pass.** Set `AccessibleName`/`AccessibleDescription`, verify keyboard navigation and tab order, add mnemonics (`&`), confirm focus is visible, set `AcceptButton`/`CancelButton`. Then run the **Pre-delivery checklist** below.

## Concrete defaults (cheat sheet)

Use these unless the project dictates otherwise. They are the difference between "default" and "chulo".

| Aspect | Default to |
|--------|-----------|
| Layout | `TableLayoutPanel` root; never absolute `Location` for structural layout |
| Resize | `Anchor`/`Dock` on every control; `Form.MinimumSize` set |
| DPI | `AutoScaleMode = Dpi`; PerMonitorV2 in app manifest / config |
| Font | Segoe UI 9pt set on the Form, inherited; Segoe UI Variable on Win11 |
| Type scale | Body 9pt · Section header 11–12pt SemiBold · Title 14–18pt |
| Spacing | 8px grid (4px tight); form `Padding = 16`; group gaps ≥ 16px |
| Surface | Near-white `Control`/`Window` surfaces, not the default gray slab |
| Accent | One color, reserved for the primary action only |
| Contrast | WCAG 4.5:1 for text; 3:1 for large text / UI affordances |
| Buttons | Min height 32px (~36 at scale), width ≥ 96px, consistent everywhere |
| Primary button | Accent `BackColor`, white text, `FlatStyle.Flat`, no border |
| Borders | Avoid `Fixed3D`; use `FlatStyle.Flat` + subtle 1px borders |
| Dialogs | `AcceptButton` + `CancelButton` set; buttons bottom-right, primary rightmost |
| Validation | `ErrorProvider` + `Validating`, inline — never serial `MessageBox` |
| Grids | No raised headers, soft/again no gridlines, alternating rows, row padding |

## Key Decisions

| Decision | Guidance |
|----------|----------|
| Absolute layout vs TableLayoutPanel | Always container-based layout. Absolute positioning is the #1 cause of forms that break on resize/DPI. |
| AutoScaleMode | `Dpi` for pixel-accurate scaling, or `Font` if the form is font-metric driven. Be consistent — mixing modes across containers misaligns everything. |
| Pure BCL vs UI library | Start BCL-pure — it's portable, free, and teaches the fundamentals. Reach for a library (see references/theming.md) only when you need rounded surfaces, animations, or a full theme engine the BCL can't express. |
| GroupBox vs whitespace | Prefer whitespace + a SemiBold section label to group. Use `GroupBox` sparingly; a form full of boxes looks busy and dated. |
| Dark mode | Worth it as a differentiator, but it's **experimental** on .NET 9+ and some controls misrender. Offer light/system/dark as an option, default to light, apply at startup. See references/theming.md. |
| Icons | Use one consistent modern icon set at one size. Don't mix clip-art and flat icons. |
| Owner-draw vs library | Owner-draw (`OnPaint`, `DrawItem`) for one or two custom touches; a library once you need it everywhere. |

## Pre-delivery checklist (the "chulo" gate)

A form is not done until every line passes. This is what separates polished from "technically works".

**Layout & resize**
- [ ] No absolute positioning for structure; root is a `TableLayoutPanel`/`FlowLayoutPanel`
- [ ] Resizing the form keeps everything aligned and usable; `MinimumSize` set
- [ ] No overlap, clipping, or stranded whitespace at min and large sizes

**DPI**
- [ ] Looks crisp at 100% / 125% / 150% / 200% — tested, not assumed
- [ ] `AutoScaleMode` consistent across form and containers; PerMonitorV2 enabled
- [ ] No hardcoded pixel sizes that break scaling; images scale cleanly

**Spacing & alignment**
- [ ] Consistent 8px-grid margins/padding; form has ≥12–16px padding
- [ ] Labels and fields align to shared columns; nothing is 1–2px off
- [ ] Logical groups separated by real whitespace

**Typography**
- [ ] Single font family (Segoe UI), set on the Form and inherited
- [ ] Deliberate type scale; hierarchy via size/weight, not random bold
- [ ] No text clipped or truncated at any supported scale

**Color & contrast**
- [ ] Neutral surfaces; one accent, used only for the primary action
- [ ] All text meets 4.5:1 contrast; verified, not eyeballed
- [ ] High-contrast mode doesn't break (custom colors yield to system)

**Controls & states**
- [ ] Buttons share size; primary is visually distinct; `AcceptButton`/`CancelButton` set
- [ ] Hover/pressed/focus/disabled states all look intentional
- [ ] Validation is inline via `ErrorProvider`; busy state disables + shows progress
- [ ] Lists/grids have an empty state; `DataGridView` is de-uglified

**Accessibility**
- [ ] Logical `TabIndex`; full keyboard operation; mnemonics on labels/buttons
- [ ] Focus is always visible; `AccessibleName`/`Description` set on key controls

## Anti-patterns (reject on sight)

- Absolute `Location`/`Size` for structural layout
- A `GroupBox` around every cluster of controls
- Mixed fonts, or bolding everything to create emphasis
- Default gray-on-gray with no surface contrast
- `Fixed3D` / `FixedSingle` borders everywhere for a heavy 1998 look
- Click targets under ~32px tall
- A wall of `MessageBox.Show` for validation
- Hardcoded pixel sizes that assume 96 DPI
- A raw `DataGridView` with raised headers and full gridlines

## Deliver

- forms with container-based layout that survive resize and DPI scaling
- a consistent spacing grid, type scale, and restrained accent palette
- polished, modern-looking controls with designed hover/focus/disabled/busy/empty/error states
- optional light/system/dark theming
- accessible, keyboard-navigable UI that passes the pre-delivery checklist

## Validate

- the form is crisp and aligned at 100–200% DPI on Windows 11
- resizing to `MinimumSize` and to large sizes never breaks the layout
- contrast meets WCAG AA; high-contrast mode still works
- every interactive state is visually distinct and intentional
- keyboard-only operation reaches every control in a sensible order

## References

- `references/layout-dpi.md` — container layout recipes (TableLayoutPanel/FlowLayoutPanel, Anchor/Dock patterns), the spacing grid in practice, full PerMonitorV2 DPI setup, and image scaling. Read when laying out or fixing resize/DPI.
- `references/theming.md` — light/dark neutral palettes with accent, contrast targets, the .NET 9+ experimental dark mode setup (`Application.SetColorMode`, WFO5001, per-control workarounds), high-contrast handling, and when a UI library earns its place. Read when applying color, theming, or dark mode.
- `references/controls.md` — per-control polish recipes: buttons (flat/primary), `DataGridView` de-uglification, `TextBox`/`ComboBox`, `ListView`, `ToolStrip`/menus, and owner-draw touches. Read when styling specific controls.
