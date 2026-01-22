# Unity Essentials

This module is part of the Unity Essentials ecosystem and follows the same lightweight, editor-first approach.
Unity Essentials is a lightweight, modular set of editor utilities and helpers that streamline Unity development. It focuses on clean, dependency-free tools that work well together.

All utilities are under the `UnityEssentials` namespace.

```csharp
using UnityEssentials;
```

## Installation

Install the Unity Essentials entry package via Unity's Package Manager, then install modules from the Tools menu.

- Add the entry package (via Git URL)
    - Window → Package Manager
    - "+" → "Add package from git URL…"
    - Paste: `https://github.com/CanTalat-Yakan/UnityEssentials.git`

- Install or update Unity Essentials packages
    - Tools → Install & Update UnityEssentials
    - Install all or select individual modules; run again anytime to update

---

# Predefined Assembly Utility

> Quick overview: Helpers for mapping Unity’s common assemblies and discovering loaded types that implement a given interface or base type across the game assemblies.

Runtime type discovery is provided over the standard Unity assemblies (Assembly-CSharp and Assembly-CSharp-firstpass). The utilities map well-known assembly names and enumerate loaded types assignable to a target interface or base type, enabling lightweight reflection scenarios like event type gathering or auto-registration.

![screenshot](Documentation/Screenshot.png)

## Features
- Known assembly mapping
  - Map assembly names (e.g., `"Assembly-CSharp"`, `"Assembly-CSharp-firstpass"`) to a friendly enum via `GetAssemblyType`
- Type discovery across game assemblies
  - `GetTypes(Type interfaceOrBaseType)` returns all loaded types assignable to the provided type
  - Searches Assembly-CSharp and Assembly-CSharp-firstpass; excludes editor-only assemblies by default
- Minimal and dependency-free
  - Uses `AppDomain.CurrentDomain.GetAssemblies()` and standard reflection only

## Requirements
- Unity 6000.0+
- Types to discover must be in loaded assemblies (typically Assembly-CSharp or Assembly-CSharp-firstpass)
- Works at runtime (play mode) when assemblies are loaded into the current `AppDomain`

## Usage
- Map an assembly name to a predefined type
```csharp
using System.Reflection;
using UnityEssentials;

string name = Assembly.GetExecutingAssembly().GetName().Name;
var asmType = PredefinedAssemblyUtility.GetAssemblyType(name);
// asmType is AssemblyCSharp, AssemblyCSharpFirstPass, etc., or null if unknown
```

- Discover all types implementing an interface (or assignable to a base type)
```csharp
using System;
using System.Collections.Generic;
using UnityEssentials;

// Example: collect all event types implementing a marker interface
IReadOnlyList<Type> eventTypes = PredefinedAssemblyUtility.GetTypes(typeof(IMyMarkerInterface));
foreach (var t in eventTypes)
{
    UnityEngine.Debug.Log($"Discovered: {t.FullName}");
}
```

Typical integration
- Event systems: gather all event types implementing `IEvent` to pre-initialize buses
- Auto-registration: find implementations of a service interface and register them in a custom container

## How It Works
- Assembly filtering
  - All loaded assemblies are enumerated; each is mapped using `GetAssemblyType(assembly.GetName().Name)`
  - Recognized assemblies are stored with their `Type[]` lists
- Type matching
  - For Assembly-CSharp and Assembly-CSharp-firstpass, every type is checked with `interfaceType.IsAssignableFrom(type)` and added if it matches and is not the interface itself
- Editor assemblies
  - Editor-only assemblies are intentionally excluded from the search set

## Notes and Limitations
- Loaded assemblies only
  - Types are discovered only from assemblies already loaded into the `AppDomain`
- Assembly scope
  - By design, only Assembly-CSharp and Assembly-CSharp-firstpass are searched; editor assemblies are not included
- Reflection caveats
  - `Assembly.GetTypes()` can throw in edge cases (e.g., load errors); callers should be aware this method does not catch such exceptions
- Type parameter
  - The parameter is named `interfaceType`, but any base type is supported; discovery returns all types assignable to it

## Files in This Package
- `Runtime/PredefinedAssemblyUtility.cs` – Assembly name mapping and type discovery across predefined game assemblies
- `Runtime/UnityEssentials.PredefinedAssemblyUtility.asmdef` – Runtime assembly definition

## Tags
unity, reflection, assemblies, type-discovery, appdomain, interface, base-type, runtime, utilities
