# UI Toolkit Gotchas

## `UnityEngine.UIElements` type name collisions

`UnityEngine.UIElements` exports types with common names that can silently collide with project types:

| UIElements type | Likely collision |
|---|---|
| `Tab` | Any custom tab/navigation enum named `Tab` |
| `Toggle` | Custom toggle classes |
| `Button` | Rarely an issue but exists |

**Fix:** When a project type shares a name with a UIElements type, use the fully qualified name for the project type throughout any file that also imports `UnityEngine.UIElements`:

```csharp
using UnityEngine.UIElements; // needed for VisualElement, Button, etc.

// BAD — ambiguous between LL.Core.Navigation.Tab and UnityEngine.UIElements.Tab
_state.CurrentTab.Value = Tab.Home;

// GOOD — unambiguous
_state.CurrentTab.Value = LL.Core.Navigation.Tab.Home;
```

Keeping both `using` directives and qualifying only the ambiguous type is cleaner than removing one import and breaking other references.

## UXML stylesheet reference path

The `<Style>` element path is relative to the UXML file's location:

```xml
<!-- MainLayout.uxml lives at Assets/_Project/UI/UXML/MainLayout.uxml -->
<!-- MainLayout.uss lives at Assets/_Project/UI/USS/MainLayout.uss   -->
<Style src="../USS/MainLayout.uss" />
```

## Tab screen visibility pattern

Use absolute-positioned panels with `display: none` toggled via USS class, not `visibility: hidden` (which still takes up layout space):

```css
.tab-screen          { position: absolute; display: none; }
.tab-screen.visible  { display: flex; }
```

Toggle in code via `AddToClassList` / `RemoveFromClassList` — never set `style.display` directly, so USS stays the single source of truth for layout.

## Scaling the UI — two approaches

There are two ways to scale all UI Toolkit elements up or down:

### Option A — Panel Settings reference resolution (preferred for global scaling)

`MainPanelSettings.asset` uses `ScaleWithScreenSize`. Reducing the reference resolution makes everything appear larger:

- 1080 × 1920 → 1:1 on a 1080p screen
- 720 × 1280 → ~1.5× larger on the same screen

**Caveat:** The `.asset` file is serialized Unity YAML. Editing it as plain text is fragile — prefer doing this via the Unity Inspector (`Scale Mode → Scale With Screen Size → Reference Resolution`). Editing it raw risks corrupting the asset.

### Option B — USS pixel values (surgical, text-safe)

Edit font sizes, heights, padding, etc. directly in the `.uss` file. Safe to do from any text editor. Only scales the values you touch, so new elements added later won't inherit the change automatically.

**Rule of thumb:** Use Panel Settings for a one-time global rescale; use USS for per-element adjustments.

## UIDocument query timing

Query named elements in `Awake()` on the same GameObject as `UIDocument`. By the time `Awake()` runs, `UIDocument.OnEnable()` has already built the visual tree. If elements come back null, confirm the `name` attribute in UXML matches exactly (case-sensitive).

## UXML template embedding (`<ui:Template>` + `<ui:Instance>`)

To embed one UXML file inside another, declare the template before `<Style>` and instantiate it with `<ui:Instance>`:

```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements" editor-extension-mode="False">
    <ui:Template name="ShopScreen" src="ShopScreen.uxml" />  <!-- must be before <Style> -->
    <Style src="../USS/MainLayout.uss" />
    ...
    <ui:VisualElement name="tab-shop">
        <ui:Instance template="ShopScreen" />
    </ui:VisualElement>
```

The `src` path is relative to the containing UXML file's location. Both files in `UXML/` means `src="ShopScreen.uxml"` (filename only).

**Critical:** UIToolkit wraps `<ui:Instance>` content in a generated `TemplateContainer` element. This container does **not** auto-fill its parent — you must size it explicitly in USS:

```css
.parent-class > TemplateContainer {
    flex-grow: 1;
    flex-direction: column;
}
```

Without this, the embedded screen will either collapse to zero height or not fill its container.

## Embedded screens conflict with centered tab panels

The default `.tab-screen` class uses `align-items: center; justify-content: center`, which is fine for a simple placeholder label but breaks a complex embedded screen that needs to fill all available space. Use a modifier class on tabs that host real screens:

```xml
<!-- simple placeholder — centering is fine -->
<ui:VisualElement name="tab-home" class="tab-screen">
    <ui:Label text="Home" class="tab-screen-label" />
</ui:VisualElement>

<!-- complex embedded screen — needs fill override -->
<ui:VisualElement name="tab-shop" class="tab-screen tab-screen--fill">
    <ui:Instance template="ShopScreen" />
</ui:VisualElement>
```

```css
.tab-screen--fill {
    align-items: stretch;
    justify-content: flex-start;
}
.tab-screen--fill > TemplateContainer {
    flex-grow: 1;
    flex-direction: column;
}
```

## ScrollView sizing requirements

**Vertical ScrollView** (scrollable content list): The ScrollView must have a constrained height — unconstrained, UIToolkit renders it at its full content height with no scroll. Absolute positioning with `top: 0; bottom: 0` inside a `position: relative; overflow: hidden` parent provides this constraint:

```css
.shop-body          { flex-grow: 1; position: relative; overflow: hidden; }
.content-panel      { position: absolute; left: 0; right: 0; top: 0; bottom: 0; }
```

**Horizontal ScrollView** (e.g. category tab bar): Needs an explicit fixed height and `flex-shrink: 0`, otherwise it collapses or stretches unpredictably:

```css
.category-scroll { height: 64px; flex-shrink: 0; }
```

To hide the scrollbar chrome use UXML attributes (not CSS):

```xml
<ui:ScrollView mode="Horizontal"
               horizontal-scroller-visibility="Hidden"
               vertical-scroller-visibility="Hidden">
```

## 2-column grid with `flex-wrap`

`flex-wrap: wrap` works in UIToolkit. For a 2-column card grid, put `width: 48%` on each card and `justify-content: space-between` on the container. The container must be width-constrained (a ScrollView's content area provides this automatically):

```css
.gem-grid { flex-direction: row; flex-wrap: wrap; justify-content: space-between; }
.gem-card { width: 48%; }
```

## Emoji characters in UXML

Raw emoji in `text=""` attributes are unreliable across platforms. Use XML unicode escapes instead:

```xml
<!-- BAD -->
<ui:Label text="💎" />

<!-- GOOD -->
<ui:Label text="&#x1F48E;" />
```

## Multiple view MonoBehaviours sharing one UIDocument

Multiple `MonoBehaviour` views can each call `GetComponent<UIDocument>().rootVisualElement` on the same GameObject and then query with `Q<>()` independently — they all receive the same root element reference. This is safe and correct.

Avoid name collisions between elements queried by different views. Use prefixes as namespacing: `btn-cat-*` for shop category buttons, `panel-*` for shop panels, `btn-*` for nav buttons, etc.
