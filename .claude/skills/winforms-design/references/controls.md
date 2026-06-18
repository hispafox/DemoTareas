# Control Polish Recipes

Per-control recipes to take stock controls from default to modern. Apply the palette from theming.md and the spacing grid from layout-dpi.md.

## Table of contents
- Buttons (flat, primary, secondary)
- TextBox / ComboBox / inputs
- DataGridView (de-uglify)
- ListView
- ToolStrip / MenuStrip / StatusStrip
- Labels & section headers
- Owner-draw touches

## Buttons

Flatten them and create a clear primary/secondary hierarchy.

```csharp
// Secondary / default button
btn.FlatStyle = FlatStyle.Flat;
btn.FlatAppearance.BorderColor = Palette.Border;
btn.FlatAppearance.BorderSize = 1;
btn.BackColor = Palette.Surface;
btn.ForeColor = Palette.TextPrimary;
btn.FlatAppearance.MouseOverBackColor = Palette.SurfaceAlt;
btn.Height = 32;                       // consistent across the app
btn.Padding = new Padding(12, 0, 12, 0);
btn.UseVisualStyleBackColor = false;

// Primary button — accent, reserved for the main action only
primary.FlatStyle = FlatStyle.Flat;
primary.FlatAppearance.BorderSize = 0;
// NOTE: FlatAppearance.BorderColor does NOT accept Color.Transparent —
// throws NotSupportedException at runtime. When BorderSize = 0 the color
// is irrelevant; set it to the button's own BackColor as a safe no-op.
primary.FlatAppearance.BorderColor = Palette.Accent; // same as BackColor; never use Transparent
primary.BackColor = Palette.Accent;
primary.ForeColor = Palette.OnAccent;
primary.FlatAppearance.MouseOverBackColor = ControlPaint.Light(Palette.Accent, 0.1f);
primary.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(Palette.Accent, 0.05f);
primary.Height = 32;
```

Rules: one primary per screen; all buttons share a height; add mnemonics (`&Save`); set the form's `AcceptButton`/`CancelButton`; order dialog buttons bottom-right with the primary rightmost.

## TextBox / ComboBox / inputs

- `TextBox`: `BorderStyle = FixedSingle` (flatter than `Fixed3D`); set `BackColor`/`ForeColor` from the palette; use a placeholder via `PlaceholderText` (.NET Core+ / .NET Framework 4.7.2+ on newer builds, else owner-draw).
- For a fully custom flat/underlined input, host the `TextBox` borderless inside a bordered `Panel` and paint the border.
- `ComboBox`: `FlatStyle = FlatStyle.Flat`; for editable lists prefer `DropDownStyle = DropDownList` when free text isn't needed.
- Keep input height consistent with button height for aligned rows. Set consistent `Margin`.
- Use `ErrorProvider` for validation feedback (icon next to the field) instead of message boxes.

## DataGridView (de-uglify)

The default grid is the loudest "LOB 2003" element. This recipe modernizes it:

```csharp
grid.BorderStyle = BorderStyle.None;
grid.BackgroundColor = Palette.Surface;
grid.GridColor = Palette.Border;
grid.EnableHeadersVisualStyles = false;            // so header styling actually applies
grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
grid.ColumnHeadersDefaultCellStyle.BackColor = Palette.SurfaceAlt;
grid.ColumnHeadersDefaultCellStyle.ForeColor = Palette.TextPrimary;
grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
grid.ColumnHeadersHeight = 36;
grid.RowHeadersVisible = false;                    // drop the gray gutter
grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal; // rows, not a full grid
grid.AlternatingRowsDefaultCellStyle.BackColor = Palette.SurfaceAlt; // subtle zebra
grid.DefaultCellStyle.SelectionBackColor = ControlPaint.Light(Palette.Accent, 0.7f);
grid.DefaultCellStyle.SelectionForeColor = Palette.TextPrimary;
grid.DefaultCellStyle.Padding = new Padding(8, 6, 8, 6);
grid.RowTemplate.Height = 32;
grid.AllowUserToAddRows = false;
grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // or size key columns deliberately
grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
```

Also: right-align numeric/currency columns, format dates/numbers via `DefaultCellStyle.Format`, and give the grid an **empty state** (an overlaid label "No records yet" when row count is 0) instead of a blank gray rectangle.

## ListView

- `View = Details`, `FullRowSelect = true`, `GridLines = false` (cleaner), `HeaderStyle = ColumnHeaderStyle.Nonclickable` if sorting isn't wired.
- Set `BackColor`/`ForeColor` from palette; pair with an `ImageList` sized for current DPI.
- For tiles/cards, `View = LargeIcon` or `Tile` with a DPI-aware `ImageList`.

## ToolStrip / MenuStrip / StatusStrip

- Apply a custom `ToolStripRenderer` (subclass `ToolStripProfessionalRenderer` with a `ProfessionalColorTable`) to control gradients/borders — the default gradients look dated.
- `GripStyle = ToolStripGripStyle.Hidden`, `RenderMode = ToolStripRenderMode.Professional`.
- Set `ImageScalingSize` for DPI; use consistent flat icons.
- `StatusStrip`: keep it quiet — one or two items, palette colors, no heavy gradient.

## Labels & section headers

- Body labels: `TextSecondary` for captions, `TextPrimary` for values.
- Section header: a `Label` at 11–12pt SemiBold in `TextPrimary`, with whitespace above — this replaces most `GroupBox` usage.
- Align label columns; prefer right-aligned labels next to fields, or top-aligned labels above fields — pick one pattern per form.
- Use `&` mnemonics and set the label's `UseMnemonic = true` so Alt+key focuses the next control in tab order.

## Owner-draw touches

When the BCL can't express something (a rounded card, a custom toggle, a colored badge), owner-draw it rather than taking a dependency:
- Rounded panel: handle `Panel.Paint`, build a rounded `GraphicsPath`, fill + draw border; set `Region` for true rounded clipping if needed.
- Enable `SmoothingMode = AntiAlias` and `TextRenderingHint = ClearTypeGridFit` in custom paint.
- Custom toggle/switch: a `CheckBox` with `Appearance = Button` styled flat, or a small owner-drawn control.
- Keep custom painting DPI-aware: scale pen widths and radii by `DeviceDpi / 96f`.
