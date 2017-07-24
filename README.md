# Alternative-Battery
An alternative battery indicator for Windows.

## Synopsis
A very simple battery indicator for Windows devices that use some kind of battery. This program uses WMI to get the battery information periodically and puts that information in the system tray.

## Motivation
My Windows 10 laptop battery indicator stopped updating the charge percentage, so I decided to write my own.

## Installation
### Compile:
```
csc.exe .\Battery.cs .\Run.cs
```

### Run:
```
.\Run.exe
```

## Commandline arguments
### Descriptions
* --color-outline color: Sets the battery's outline color
* --color-background color: Sets the battery's background color
* --color-normal-charge color: Sets the battery's normal charge color
* --color-powersave-charge color: Sets the battery's powersave charge color
* --color-low-charge color: Sets the battery's low charge color
* --color-critical-charge color: Sets the battery's critical charge color
* --interval seconds: The amount of seconds between the indicator's updates
* --show-console: Switch for displaying the console

### Example
```
.\Run.exe --color-outline 'Green' --interval 30 --show-console
 ```
Runs the indicator with a battery icon that has a green outline. The indicator is updated every 30 seconds. A console is shown if this is run through a shortcut.

## Warnings
* This is a VERY bare-bones solution
* There is no guarantee that this will work
* Do not rely on the existing code since it might change significantly at any point in the future
* This battery indicator does not include all the functionality that the native Windows (10) battery indicator supports

## Future improvements
* Charging icon when charging
* GUI for the settings
* More information in the system tray, like "time to full charge" (when charging) or "time to depletion" (when discharging)
