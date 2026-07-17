# Motion Cues Prototype v0.9 GUI

A private, self-contained Windows passenger-comfort experiment with a full-screen stochastic particle field.

## Run it

Double-click `MotionCuesPrototype-v0.9-gui.exe`. The transparent overlay does not block clicks or typing. Closing the controls hides them; double-click the notification-area icon to reopen them.

## Features

- Completely offline procedural motion
- No phone, accelerometer, network connection, account, or background service
- Full-screen stochastic particles with independent wandering
- Adjustable interval, direction, dot size, particle count, flow speed, and opacity
- Working color chooser with live swatch, hex display, and eight presets
- Roomier controls with protected foreground layering over the motion overlay
- Readable high-contrast controls that remain above the overlay
- Manual overlay toggle and notification-area controls
- Settings saved under `%LOCALAPPDATA%\MotionCuesPrototype`

## Build from source

Run `build.ps1` with PowerShell. It uses the C# compiler included with Windows and creates `MotionCuesPrototype-v0.9-gui.exe` in this folder.

This is an experimental comfort aid, not a medical device. It is intended for passengers only.

