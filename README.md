<h1 align="center"> Full Range Autoturrets </h1> <br>
<p align="center">
  <a href="https://gitpoint.co/">
    <img alt="FullRangeAutoturrets" title="FullRangeAutoturrets" src="https://hypernova.gg/game-data/rust/plugins/fullrangeautoturrets/header.png" width="1024">
  </a>
</p>

<p align="center">
  Making turrets even more of a bitch to deal with. Built with Harmony.
</p>

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Feedback](#feedback)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## Introduction

Allows server admins fine-grain control over the detection ranges and rotation ranges for AutoTurrets and FlameTurrets. You can make them even harder to mess with by giving them 360° detection range, or you can effectively make them harmless by setting their detection range to 0, making them just for show. It's all up to you. Hardcore server? We gotchu. Chill server? Ain't nobody got time for turrets, make 'em limp.

**Modifies both AutoTurrets and FlameTurrets.**

<p align="center">
  <img src="https://hypernova.gg/game-data/rust/plugins/fullrangeautoturrets/autoturretanim2.gif" width=350>
  <img src="https://hypernova.gg/game-data/rust/plugins/fullrangeautoturrets/flameturretanim2.gif" width=350>
</p>

## Features

A few of the things you can do with this mod:

* Modify detection range from 0° to 360° for AutoTurrets and FlameTurrets
* Modify rotation range from 0° to 360° for AutoTurrets and FlameTurrets
* Use its' fully documented open source code to learn Harmony with
* Take it out for a cup of coffee, idk. It's a mod.

## Configuration

This mod creates a configuration file in 

By default it'll look something like...
```json
{
  "Modification enabled (true/false)": true,
  "AutoTurret Options": {
    "Modify all AutoTurrets (true/false)": true,
    "Detection Range Degrees (0-360)": 360.0,
    "Rotation Range Degrees (0-360)": 360.0
  },
  "FlameTurret Options": {
    "Modify all FlameTurrets (true/false)": true,
    "Detection Range Degrees (0-360)": 360.0,
    "Rotation Range Degrees (0-360)": 360.0
  }
}
```
This should all be pretty self-explanatory, but in case it's not...
* **"Modification enabled (true/false)"** => Enables or disables the entire mod
* **"Modify all AutoTurrets (true/false)"** => Enables or disables the modification of AutoTurrets (**all of them!**)
* **"Modify all FlameTurrets (true/false)"** => Enables or disables the modification of FlameTurrets (**all of them!**)
* **"Detection Range Degrees (0-360)"** => Sets the *detection* range for a turret. This does **NOT** change the animation. If set to 0, disables the turret type.
* **"Rotation Range Degrees (0-360)"** => Sets the *rotation* range for a turret. This does **NOT** change the detection behavior. If set to 0, disables animation. If set to 360, will cause the AutoTurret to randomly choose a direction, and will cause the flame turret to rotate in circles infinitely.

## Feedback

Feel free to send me feedback on Discord by adding **Airathias#0001** or [file an issue](https://github.com/Hypernova-gg/FullRangeAutoturrets/issues/new). Feature requests are always welcome, thought due to limited time for sideprojects these few will be implemented, if any.
