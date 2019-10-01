# Clickami

This Clickbot was built by Christopher Quadflieg aka Shinigami.  
You can use it to click on a given desktop position 1,000 times per second if your CPU can handle it ;)

## Contents

- Clickami.exe - The program
- Xceed.Wpf.Toolkit.dll - This is a library needed by the exe. If this is not in the same directory as the ClickBot, it will not work.
  Source: https://github.com/xceedsoftware/wpftoolkit

## TODO

- [x] Persistent settings [#1][issue-1]
- [x] Add tooltips explanations for each field [#2][issue-2]
- [ ] Make hotkeys configurable [#3][issue-3]

## Installation

Extract the zip file to any location and double-click the exe.

## Instruction for the fields

- X - The x-coordiante for the mouse where to click
- Y - The y-coordiante for the mouse where to click
- Delay - The delay between clicks
- Loops - How often to click
- Infinitely - Check this if you want to click infinitely (can stopped via ESC-key)
- always on top - Check this if you want to always have this window
  in front of each other window.

You can find the actual mouse-coordinates in the right bottom corner of the Clickbot.  
You can use F1-key to start and ESC-key to stop clicking.  
The Clickbot will register all ESC- and F1-key presses, so that these keys won't work in other
applications while Clickbot is running! Please close the Clickbot to unregister the keys. (I don't
know if I will fix this.)

## Changelog

[Learn about the latest improvements][changelog].

Thanks for using my program  
Shinigami

[issue-1]: https://github.com/Shinigami92/Clickami/issues/1
[issue-2]: https://github.com/Shinigami92/Clickami/issues/2
[issue-3]: https://github.com/Shinigami92/Clickami/issues/3
[changelog]: CHANGELOG.md
