# Ethereal - An Aethermancer modding API

A modding API for [Aethermancer](https://store.steampowered.com/app/2288470/Aethermancer/) to help create mods with BepInEx.

## Downloads

You can get Ethereal from the [latest releases](https://github.com/minavoii/Ethereal/releases/latest/download/Ethereal.zip).

A build of the monster randomizer example is available [here](https://github.com/minavoii/Ethereal/releases/latest/download/Randomizer.zip).

## Installing mods

If you don't have BepInEx:

-   Download [BepInEx separately](https://github.com/BepInEx/BepInEx) or the [Ethereal + BepInEx package](https://github.com/minavoii/Ethereal/releases/latest/download/Ethereal+BepInEx.zip).

-   Extract the files into the game's directory (`C:\Program Files (x86)\Steam\steamapps\common\Aethermancer`).

You can now download mods and extract them into the `BepInEx\plugins` directory.

## Examples

You can find example mods into the [`examples` directory](https://github.com/minavoii/Ethereal/tree/main/examples).

## Adding localisation

You can add localisations at runtime as shown in the examples.

You can also add entire new languages or modify existing ones by creating `.json` files inside the `plugins\Ethereal\Locales` directory.

Please read [the wiki entry](https://github.com/minavoii/Ethereal/wiki/Localisation) for more details.

## Replacing sprites

You can replace many sprites by placing them into the `plugins\Ethereal\Sprites` directory or anywhere else via the `Sprites` API.

Both individual images and Unity asset bundles are supported, but require a specific structure and naming.

Please read [the wiki entry](https://github.com/minavoii/Ethereal/wiki/Sprites) for more details.
