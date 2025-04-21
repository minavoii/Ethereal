# Ethereal - An Aethermancer modding API

A modding API for [Aethermancer](https://store.steampowered.com/app/2288470/Aethermancer/) to help create mods with BepInEx.

Currently made for demo version 0.1.1.7.

## Installation

-   Download the BepInEx package from the [latest releases](https://github.com/minavoii/Ethereal/releases/latest/download/BepInEx_Package.zip).

-   Extract the files into the game's directory (`C:\Program Files (x86)\Steam\steamapps\common\Aethermancer Demo`).

-   You can download mods and extract them into the `BepInEx\plugins` directory.

-   A build of the monster randomizer example is available [here](https://github.com/minavoii/Ethereal/releases/latest/download/Randomizer.zip).

If you already installed BepInEx, you can get Ethereal separately from [here](https://github.com/minavoii/Ethereal/releases/latest/download/Ethereal.zip).

## Examples

You can find example mods into the [`examples` directory](https://github.com/minavoii/Ethereal/tree/main/examples).

## Adding new languages

You can add localisations at runtime as shown in the examples.

You can also add entire new languages or modify existing ones by creating `.json` files inside the `plugins\Ethereal\Locales` directory.

Please read [the wiki entry](https://github.com/minavoii/Ethereal/wiki/Languages) for more details.

## Replacing sprites

You can replace many sprites by placing them into the `plugins\Ethereal\Sprites` directory or anywhere else via the `Sprites` API.

Both individual images and Unity asset bundles are supported, but require a specific structure and naming.

Please read [the wiki entry](https://github.com/minavoii/Ethereal/wiki/Sprites) for more details.
