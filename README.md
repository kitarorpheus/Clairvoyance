# Clairvoyance
Display folder sizes in Explorer list view.

## ⚠️ Disclaimer
This program may cause Windows Explorer to become unresponsive.  
**Use at your own risk.**
The author is still learning, so improvements and optimizations by experts are welcome.

## Installation
1. Unzip the file and place it in any location.
2. Press `Win + R`, type `regedit`, and press Enter.
3. Navigate to: 
`HKEY_CLASSES_ROOT\Directory\Background\shell\`
4. Right-click → New → Key → (Any application name).
5. Right-click on the created key → New → Key → `command`.
6. Double-click `(Default)` and set it to:
`"C:(Path\to)\Clairvoyance.exe" "%V"`
7. Open Explorer, right-click, and you should see your registered application name in the context menu.

### Uninstallation
Delete the registry keys you created under  
`HKEY_CLASSES_ROOT\Directory\Background\shell\`.

## Motivation
Windows Explorer does not display folder sizes in the “Size” column, even in detailed view.  
This program attempts to fill that gap by overlaying folder size information.

## Mechanism
### Overlay
Uses UIAutomation to obtain the position of the “Size” column and overlays a transparent window there.  
### Calculation
The program launched via right-click recursively calculates folder sizes.  
### Display
The pairs of folder names and sizes are displayed in the transparent overlay window.
