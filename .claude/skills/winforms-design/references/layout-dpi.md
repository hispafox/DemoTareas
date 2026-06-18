# Layout & DPI

The two things that most separate a professional WinForms app from a default one: it stays aligned when resized, and it stays crisp when scaled. Get these right before touching color or controls.

## Table of contents
- Container-based layout
- Anchor & Dock
- The spacing grid in practice
- PerMonitorV2 DPI setup
- Image and icon scaling
- Common layout recipes

## Container-based layout

Never use absolute `Location`/`Size` for structural layout. Use containers that recompute positions automatically.

- **`TableLayoutPanel`** — the workhorse. A grid of rows/columns. Use percentage or `AutoSize` rows/columns for flexible regions and absolute for fixed ones (e.g. a fixed label column + a percentage field column).
- **`FlowLayoutPanel`** — for sequences that wrap or stack (toolbars, button rows, tag chips).
- **`SplitContainer`** — for user-resizable regions (master/detail, navigation + content).
- **`Panel`** with `Dock` — for banding a form into header / content / footer.

A clean form is usually a root `TableLayoutPanel` (or a docked set of panels), with nested layout panels inside cells. Set `Dock = Fill` on the root so it owns the client area.

```csharp
// Label column fixed, field column flexible — the classic form body
var table = new TableLayoutPanel
{
    Dock = DockStyle.Fill,
    ColumnCount = 2,
    Padding = new Padding(16),
    AutoSize = false,
};
table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));            // labels
table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));      // fields
// each field control: Dock = Fill, Margin = new Padding(0, 0, 0, 8)
```

Tips:
- Give input controls `Dock = DockStyle.Fill` (or `Anchor = Left|Right`) inside their cell so they grow with the column.
- Use `Margin` on children for inter-control spacing and `Padding` on the container for the inset — don't fake spacing with empty labels or `Location` math.
- Set `AutoSizeMode = GrowAndShrink` on panels that should hug their content.

## Anchor & Dock

- **`Dock`** attaches a control to an edge (or fills). Use for structural regions: a top toolbar (`Dock = Top`), a footer button bar (`Dock = Bottom`), the content area (`Dock = Fill`). **Add docked controls in reverse z-order** (Fill first in code, or last-added wins the remaining space) — get this wrong and the Fill control eats the others.
- **`Anchor`** pins a control's edges to its container's edges. `Anchor = Top|Left` (default) keeps a control fixed; `Top|Left|Right` stretches it horizontally; `Top|Bottom|Left|Right` stretches both ways.
- A button that should stay bottom-right on resize: `Anchor = Bottom|Right`. A text box that should widen with the form: `Anchor = Top|Left|Right`.

Always set `Form.MinimumSize` so the layout can't be crushed below usability.

## The spacing grid in practice

Use an 8px base unit (4px for tightly-related pairs like a label and its field).

- Form inset: `Padding = new Padding(16)`.
- Between stacked fields: `Margin = new Padding(0, 0, 0, 8)`.
- Between logical groups: a spacer row of 16–24px, or a group `Margin` top of 16.
- Button bar: `FlowLayoutPanel` docked bottom, `FlowDirection = RightToLeft`, `Padding = new Padding(0, 8, 0, 0)`, button `Margin = new Padding(8, 0, 0, 0)`.

Consistency is the point: every gap should be a multiple of the base unit. Mismatched 7/11/13px gaps are what make a form feel sloppy even when nothing is obviously wrong.

## PerMonitorV2 DPI setup

Goal: the app scales crisply across monitors with different scaling, with no blur. Two pieces — the awareness mode and consistent `AutoScaleMode`.

**.NET (8/9/10+) — preferred, via runtime config.** `ApplicationConfiguration.Initialize()` reads settings from the project / `app.config`. Set high-DPI mode there or explicitly:

```csharp
[STAThread]
static void Main()
{
    Application.SetHighDpiMode(HighDpiMode.PerMonitorV2); // before anything creates a window
    ApplicationConfiguration.Initialize();
    Application.Run(new MainForm());
}
```

Or declare it in the project (`.csproj`):

```xml
<PropertyGroup>
  <ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>
</PropertyGroup>
```

**.NET Framework 4.7+ — via app.manifest + app.config.** Add to the manifest:

```xml
<application xmlns="urn:schemas-microsoft-com:asm.v3">
  <windowsSettings>
    <dpiAwareness xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">PerMonitorV2</dpiAwareness>
    <dpiAware xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">true/PM</dpiAware>
  </windowsSettings>
</application>
```

**Per-form/control:** keep `AutoScaleMode` consistent everywhere.
- `AutoScaleMode = AutoScaleMode.Dpi` — pixel-accurate, recommended for layout-driven forms.
- `AutoScaleMode = AutoScaleMode.Font` — scales by font metrics; fine if used everywhere.
- Mixing `Dpi` on the form and `Font` (or `None`/`Inherit` unexpectedly) on a `UserControl` is the classic cause of controls that drift out of alignment on scaled displays. Pick one and apply it to every form and UserControl.

**Test matrix:** 100%, 125%, 150%, 200%, and moving the window between monitors at different scales. Don't assume — scaling bugs only show on a scaled monitor.

## Image and icon scaling

- Don't ship a single 16px bitmap and let Windows blur it. Provide multiple sizes (16/20/24/32/48) and select by DPI, or use vector-ish sources (high-res PNG scaled down, or an icon font / SVG-to-bitmap at runtime).
- For `ImageList`-based controls, set `ImageList.ImageSize` based on current DPI and load appropriately sized images.
- Prefer monochrome line icons that can be recolored for light/dark over baked-in colored bitmaps.

## Common layout recipes

**Header / content / footer:**
```
Form (Padding 0)
├─ Panel  Dock=Top    (header / title bar area, fixed height)
├─ Panel  Dock=Bottom (button bar, fixed height)
└─ Panel  Dock=Fill   (content; usually a TableLayoutPanel inside)
```

**Master / detail:** `SplitContainer` `Dock=Fill`; list on `Panel1`, detail form (a `UserControl`) on `Panel2`; set `Panel1MinSize`.

**Toolbar + grid:** `ToolStrip` `Dock=Top`; `DataGridView` `Dock=Fill`; status bar `StatusStrip` `Dock=Bottom`.

**Dialog:** content `TableLayoutPanel` `Dock=Fill` with form `Padding=16`; a bottom `FlowLayoutPanel` (`RightToLeft`) with Cancel then OK so OK sits rightmost; set `AcceptButton`/`CancelButton`.
