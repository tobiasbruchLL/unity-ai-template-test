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
