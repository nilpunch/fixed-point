# Fixed Point

Deterministic fixed-point math library designed for online multiplayer games and other use cases.  
Can be used as a pure C# library or as a Unity package.

> [!WARNING]
> This project is in the early stages of development and lacks some core functionality. It is not yet suitable for production use.

## Overview

This library is based on [FixedMath.Net](https://github.com/asik/FixedMath.Net) but has been rewritten without hardcoded magic numbers that were tied to a specific scaling factor.

### Key Features

- The number of fractional bits can be controlled using a single constant.
- LUTs are generated deterministically at runtime with adjustable precision and size.
- Avoids static initializers, preventing IL2CPP from introducing unnecessary static guards.
