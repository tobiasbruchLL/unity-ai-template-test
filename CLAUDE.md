# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity 6 (6000.3.8f1) template project for 2D game development using clean architecture, dependency injection, and reactive programming patterns.

## Key Technologies

- **VContainer** (1.17.0) — IoC/DI container; all dependencies are registered via `IContainerBuilder` in installer classes
- **R3** — Reactive Extensions for Unity; use `Observable`, `Subject`, `ReactiveProperty` for event-driven code
- **PrimeTween** (1.3.8) — Animation/tweening library
- **Universal Render Pipeline** (URP 17.3.0, 2D renderer)
- **Unity Input System** (1.18.0) — use `InputSystem_Actions.inputactions` asset for input bindings

## Architecture

The project uses a three-layer clean architecture enforced via Assembly Definitions:

```
Presentation (LL.Template.Presentation)
    └── Core (LL.Template.Core)
            └── Common (LL.Common)
```

- **`Assets/_Common/`** — Shared utilities; `LL.Common.asmdef` is auto-referenced
- **`Assets/_Project/Scripts/Core/`** — Business logic, use-cases, domain models; no Unity UI dependencies
- **`Assets/_Project/Scripts/Presentation/`** — MonoBehaviours, Views, UI controllers
- **`Assets/_Project/Scenes/`** — `Start.unity` (startup/loading), `Main.unity` (main game)
- **`Assets/Settings/`** — URP assets, Input System actions, renderer config

Dependencies flow strictly downward: Presentation → Core → Common. Core must not reference Presentation.

## Build Scenes

Scene build order (EditorBuildSettings):
1. `Assets/_Project/Scenes/Start.unity`
2. `Assets/_Project/Scenes/Main.unity`

## Development Setup

Open the project in Unity Hub with Unity 6000.3.8f1. IDE options: VS Code (`.vscode/` config included with Unity debugger attach), Rider, or Visual Studio.

NuGet packages are managed via NuGetForUnity (`Assets/packages.config`). To add .NET packages, use the NuGetForUnity editor window rather than editing `packages.config` directly.

## Coding Conventions

- New feature areas get their own assembly definition (`.asmdef`) placed alongside the scripts folder
- VContainer installers (classes implementing `IInstaller`) wire up dependencies for a scene or feature scope
- Reactive state lives in Core; Presentation subscribes to Core observables and drives UI
- Avoid static singletons; prefer constructor/field injection via VContainer
