# 4X GAME - PHASE 2

This **4X Game** phase begins where [Phase 1] ended, a Unity 2021.3 LTS game that
allows to generate, manipulate and pick a `.map4x` file from the Desktop to be
loaded and displayed as an interactive game map.

This repository implements different playable units that move around the map and
can harvest its resources, in turns.

Below, we'll revisits the main concepts from Phase 1, highlighting improvements
and all new features.

[**`• CODE ARCHITECTURE •`**](#code-architecture) [**`• METADATA •`**](#metadata)

---

## Game Elements

![Game Elements](Images/game_elements.png "4X Game Gameplay")

The game map is represented by a grid of squared tiles, each visually composed
by a terrain with zero to six different resource types. All tiles can be
inspected when clicked, displaying its properties: **Terrain, Resources, Coin**,
and **Food**.

The total number of map resources is displayed on top of the screen
at all times and zooming/panning is also made available, through the typical
keyboard binds.

**Units** can be placed on the map and ordered to move and interact with it,
harvesting and generating resources, by turns. To make it so they are always
visible and more easily selectable, units remain the same size while the map
zooms in and out.

### Terrains

![Terrains](Images/terrains_all.png "The 5 terrain types")

There are **5 terrain types**, all visibly distinct, represented by two color
tones only. As the foundation of each game tile, terrains are responsible for
establishing its base **Coin** and **Food** values.

### Resources

![Resources](Images/resources_all.png "The 6 resource types, across all terrains")

There are **6 resource types**, also visibly distinct, but not only from
each other. On some cases they will vary depending on the terrain they're on.
A terrain can have up to 6 resources, each having their own **Coin** and **Food**
values, which stack.

![Water Resources](Images/water_resources_all.png "All 6 resources values")

Above are the pre-defined **Coin** and **Food** values for each resource.

### Units

![Units](Images/units_all.png "Desert tile with 4 resources")

There are **4 unit types**, each with a different combination of behaviours. All
units can move towards a map cell selected by the user, however, not all move the
same way. As of now, there are 2 moving behaviours the units can adopt:

+ [Von Neumann]: ⇐ ⇑ ⇒ ⇓  
+ [Moore]: ⇐ ⇑ ⇒ ⇓ ⇖ ⇗ ⇘ ⇙

Furthermore, units can also **harvest resources** from the cell they are on and
even **generate new ones**. The resources each unit collects and generates can
be found in the image above.

**Note:** The Miner is the only unit that generates a resource as of now
(Pollution) whenever it successfully collects Metals.

---

## Code Architecture

![UML](Images/uml.png "Classes UML Diagram")

The game is structured around 5 Main Sections, centrally managed by the
[`Controller`]. Represented as one or more interface panels, visually toggled by
the [`PanelsUserInterface`], each responsible for handling its respective game
behaviours independently from each other and delegating responsibilities to
secondary classes when needed.

Lastly, and improving from the previous phase, panels now rely on
[Unity ScriptableObjects], holders of pre-defined and ongoing game information,
thus allowing data to be shared without creating dependencies.

**Game sections:** [**`• PRE-START •`**](#pre-start) [**`• MAP BROWSER •`**](#map-browser) [**`• MAP DISPLAY •`**](#map-display) [**`• INSPECTOR •`**](#inspector) [**`• UNITS CONTROL •`**](#units-control)

---

### Controller

(Manages game)

### User Interface

(Talk about panels)

### Scriptable Objects

(Store game data.)

---

### Pre-start

[Imagem]

(Same)

---

### Map Browser

[Imagem]

(Add file verification system + Warnings)

#### Load Map

(No longer checks for hardcoded names, uses scriptable object data instead)

---

### Map Display

[Imagem]

(No longer using pivot and scale to pan and zoom. Back to using cam)

---

### Inspector

[Imagem]

(Visual improvements + less hardcoded).

---

### Units Control

[Imagem]

(New stuff. Unit spawn + selection, movement and harvesting).

---

### UML Diagram

[Imagem] (Fix last one w feedback + new things. Mby move up?)

---

## References

+ [4X Game (Phase 1) - Afonso Lage e André Santos][Phase 1]
+ [ScriptableObjects - Unity Documentation][Unity ScriptableObjects]
+ [Removing whitespace from string - StackOverflow (Jay Byford-Rew)][Whitespace]

## Metadata

|       [Afonso Lage (a21901381)]      |       [André Santos (a21901767)]      |
|:------------------------------------:|:-------------------------------------:|
|                                      |                                       |
|       Units Control Panel setup      |    Invalid maps check + UI warnings   |
|    Map & Unit's Resource Counters    |          Scriptable Objects           |
|           Units Harvesting           |     New & Updated User Interface      |
|                - - -                 |       Changes to Map Pan & Zoom       |
|                - - -                 |   Units spawn, selection & movement   |
|       XML Documentation (1/2)        |        XML Documentation (1/2)        |
|             README (1/2)             |              README (1/2)             |

> Game created as final project for Programming Languages II, 2022/23.  
> Professor: [Nuno Fachada].  
> [Bachelor in Videogames] at [Lusófona University].

[**`• BACK TO TOP •`**](#4x-game---phase-2)

[`Controller`]:4XGameP2/Assets/Scripts/Controller.cs
[`PanelsUserInterface`]:4XGameP2/Assets/Scripts/UserInterface/Panels/PanelsUserInterface.cs

[Phase 1]:https://github.com/andrepucas/lp2_4XGame_p1_2022
[Unity ScriptableObjects]:https://docs.unity3d.com/Manual/class-ScriptableObject.html
[Von Neumann]:https://en.wikipedia.org/wiki/Von_Neumann_neighborhood
[Moore]:https://en.wikipedia.org/wiki/Moore_neighborhood
[Whitespace]:https://stackoverflow.com/questions/6219454/efficient-way-to-remove-all-whitespace-from-string/30732794#30732794
[Afonso Lage (a21901381)]:https://github.com/AfonsoLage-boop
[André Santos (a21901767)]:https://github.com/andrepucas
[Nuno Fachada]:https://github.com/nunofachada
[Bachelor in Videogames]:https://www.ulusofona.pt/en/undergraduate/videogames
[Lusófona University]:https://www.ulusofona.pt/en/
