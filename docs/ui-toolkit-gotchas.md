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

## UIDocument query timing

Query named elements in `Awake()` on the same GameObject as `UIDocument`. By the time `Awake()` runs, `UIDocument.OnEnable()` has already built the visual tree. If elements come back null, confirm the `name` attribute in UXML matches exactly (case-sensitive).
