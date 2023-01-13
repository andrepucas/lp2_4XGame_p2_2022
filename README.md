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
zooms in and out, independently.

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

![Units](Images/units_all.png "The 4 unit types")

There are **4 unit types**, each with a different combination of behaviours. All
units can move towards a map cell selected by the user, however not all move the
same way. As of now, there are 2 moving behaviours the units can adopt:

+ [Von Neumann]: ⇐ ⇑ ⇒ ⇓  
+ [Moore]: ⇐ ⇑ ⇒ ⇓ ⇖ ⇗ ⇘ ⇙

Furthermore, units can also **harvest resources** from the cell they are on and
even **generate new ones**. The resources each unit collects and generates can
be found in the image above.

The Miner is the only unit that generates a resource as of now (Pollution)
whenever it successfully collects Metals.

---

## Code Architecture

![UML](Images/uml.png "Classes UML Diagram")

### Controller

Manages the game, handling Player Input and [`GameStates`]. It starts
by setting the `CurrentState` to `Pre Start`, which in turn delegates the
[`PanelsUserInterface`] to update it's display, sending it the respective
[`UIStates`]. These represent specific UI cases for each [`GameStates`], managed
by boolean control variables, thus easing the transition between displayed info.

> The `CurrentState` and boolean control variables are updated
> following the **Observer Pattern**, through subscribed events, meaning other
> classes don't need a direct [`Controller`] reference.

### User Interface

The game's [`PanelsUserInterface`], which implements the generic [`IUserInterface`]
(**Interface Segregation Principle**), focuses on enabling and disabling single
responsibility panels, visually reflecting the current game section, following
the **Single Responsibility Principle**.

In turn, each panel handles their respective game behaviours and raises events
when the `CurrentState` needs to be updated. All panels extend an abstract
[`UIPanel`], which contains the universal opening and closing panel behaviours.
This follows the **Open/Closed Principle**, due to the ease of creating new
panels without having to modify any code.

### Scriptable Objects

Lastly, and improving from the previous phase, panels now rely on
[Unity ScriptableObjects] which hold pre-defined and ongoing game information,
thus allowing data to be shared without creating unneeded dependencies.

On top of that, with pre-defined data such as Terrains, Resources and Units, it's
now easier to simply compare data when validating/creating maps or units, instead
of the previously hardcoded string-compare solutions.

![SOs](Images/scriptable_objects.png "Preset Data Scriptable Object")

Now, to add or remove elements to the game, one simply needs to modify the
ScriptableObjects through the editor, without having to touch any code, which
adjusts itself and most* of the UI display automatically, complying with the
**Open/Closed Principle**

> \* In the case of units, new UI add buttons need to be manually added and the
> global resource counters might need to be pushed forward, so that everything fits
> on the screen. (editor modification only, no need to change any code).

---

## Game Sections

[**`• PRE-START •`**](#pre-start) [**`• MAP BROWSER •`**](#map-browser) [**`• MAP DISPLAY •`**](#map-display) [**`• INSPECTOR •`**](#inspector) [**`• UNITS CONTROL •`**](#units-control)

---

### Pre-start

![Pre-Start](Images/pre_start.png "Pre-start screen")

In the [`PreStartPanel`], an event is raised when the input prompt
"Press any key to start" is revealed. This event is subscribed by the
[`Controller`], which in turn starts a coroutine that will update the
`CurrentState` to `MapBrowser` after any key is pressed.

---

### Map Browser

![Map Browser](Images/map_browser.png "Map browser screen")

The [`MapBrowserPanel`] displays all existing map files inside the Desktop's
'map4xfiles' folder in a scrollable menu.

> The scrollable menu originally used the Unity's UI Element Scroll Rect
> component, however due to a mouse scroll wheel bug, it's now using a custom
> [`UpgradedScrollRect`] extension.

It starts by using the static [`MapFilesBrowser`] class to create a [`MapData`]
instance for each file, and return them. A [`MapData`] instance, at this initial
stage, contains a string array with all the file lines, a name (that matches
the file, without the extension), and the X and Y map dimensions, which are read
right away, from the first file line.

Improved since the first version, if the maps dimensions can't be converted at
this stage, even with the new way of reading files (ignoring blank and completely
commented lines), then the map's dimensions aren't saved at all, triggering an
invalid map reaction later on.

```c#
// If the conversion of both rows and cols value is successful.
if (m_firstLine.Length == 2 && Int32.TryParse(m_firstLine[0], out _rows) 
    && Int32.TryParse(m_firstLine[1], out _cols))
{
    // Sets the Rows and Cols properties.
    YRows = _rows;
    XCols = _cols;

    // Increments lines to ignore, so that future indexers start
    // after this dimensions line.
    _linesToIgnore++;
}

// If the conversion isn't possible, do nothing.
// It will be recognized as an invalid map later.
```

For each [`MapData`] returned, **if it has dimensions**, a [`MapFileWidget`]
is instantiated, serving as its visual representation and displaying the map's
name and dimensions, while referencing it. However, if it's dimensions are null,
the widget isn't created and a warning (as can be seen in the image above) is
revealed. Warnings are toggled by the static [`UIWarnings`] class, that
simply enables and disables the warning objects.

The displayed map name on the widgets can be edited by the player, which
internally updates the [`MapData`] and file's name. Because the file name
is editable, cautions have to be taken to not allow for illegal path characters,
verified by the static [`MapFileNameValidator`], which replaces illegal
characters for `_` using [Regex], or duplicate names, which is verified by the
[`MapFilesBrowser`] and adds a `_N` to the duplicate file.

![MapWidgetName](Images/widget_name.gif "Widget replacing illegal characters.")

```c#
private static readonly Regex ILLEGAL_CHARS = new Regex("[#%&{}\\<>*?/$!'\":@+`|= ]");
(...)
p_fileName = ILLEGAL_CHARS.Replace(p_fileName, "_");
```

After each [`MapFileWidget`] is instantiated, a [`MapFileGeneratorWidget`] is
instantiated at the end of the list, allowing direct map files creation, using
[Nuno Fachada's Map Generator][Generator].

> Regarding the Map Generator's code, we've made 2 small adjustments to the
> version we have implemented:
>
> + Increased the chance of generating resources from 0.3 to 0.5, to generate
> richer maps.
> + Fixed a small bug that prevented very small maps (with x * y > 10) from being
> generated and caused small maps to only have one or two terrains.
>
> ```c#
> int totalTiles = rows * cols;
>
> int numCenterPoints = (totalTiles > 50)
>    ? (int)(totalTiles * centerPointsDensity) 
>    : (int)(totalTiles * centerPointsDensity * (100/totalTiles));
> ```

When the Load Button is pressed, an event is raised containing the selected map
(yellow outline), which causes the [`Controller`] to change its `CurrentState` to
`LoadMap`.

#### Load Map

Before being ready to display, the selected [`MapData`] needs to convert its
array of lines into a [`GameTile`] list, a class that represents a tile's terrain,
containing a [`Resource`] list.

> While in the previous version [`GameTile`] and [`Resource`] were abstract
> classes with subclasses for each type. We've since then come to the conclusion
> that it was unnecessary and only complicated our code. This approach has been
> completely replaced with the current [Unity ScriptableObjects] system.

The conversion is done by iterating all lines, starting at the line following up
the dimensions, which have already been handled, when [`MapData`] was instantiated.
In each line, it starts by looking for a `#`, by trying to get its index.
If its greater than 0, then that line has a comment that needs to be ignored,
using a substring.

```c#
m_commentIndex = m_line.IndexOf("#");
if (m_commentIndex >= 0) m_line = m_line.Substring(0, m_commentIndex);
```

In our last version, this substring's size was not being accounted for, meaning
that a line that started with a `#` wouldn't be ignored (and neither would empty
lines). A major bug that has since then been fixed.

```c#
// If a comment occupies the full line or is empty, ignore it.
if (m_line.Length == 0)
{
    _linesToIgnore++;
    continue;
}
```

The line is then split into an array of strings, each representing a keyword.
The first should always be a Terrain, so it's compared with the Terrain names the
game has and instantiates a [`GameTile`] accordingly, adding it to this
[`MapData`]'s [`GameTile`] list. In our previous version, it looked something
this:

```c#
// Hardcoded switch case with string names.
case "desert":
    GameTiles.Insert(i - 1, new DesertTile());
    break;
```

A terrible solution that didn't respect the **Open/Closed Principle**, making it
necessary to change the code every time we wanted to update the game elements.

With ScriptableObjects, it now iterates every possible predefined game terrain
and looks for a raw name match - the name in lower case and no white spaces, the
most performant way we found of doing it was by [Jay Byford-Rew][Whitespace]:

```c#
public string RawName => string.Join("", _name.Split(default(string[]), 
    System.StringSplitOptions.RemoveEmptyEntries)).ToLower();
```

If the name is found, it then gets all relevant info from the scriptable object
and instantiates our game tile.

If there are any other words in the string array, each represents a [`Resource`]
to add to the [`GameTile`] we just instantiated. Again, each keyword is compared
with all possible Resource names in the game, now in the data ScriptableObject,
and added accordingly. In this version, if the supposed resource's name doesn't
match any of the possible resources, a control variable saves that not all
resources were added.

After iterating all the file data, the map's validity is checked once again:

```c#
// If the map's dimensions don't match the number of game tiles saved
// or if at least one resource couldn't be read, raise invalid data event.
if ((XCols * YRows) != GameTiles.Count || _failedResource)
    OnValidLoadedData?.Invoke(false);

// Otherwise, this map is valid to be load.
else OnValidLoadedData?.Invoke(true);
```

If the map isn't considered valid, another UI warning pops up and the map isn't
generated.

![InvalidMapLoaded](Images/invalid_map.gif "Invalid map warning.")

---

### Map Display

![Map Display](Images/map_display.png "Main map display screen")

(No longer using pivot and scale to pan and zoom. Back to using cam)

---

### Inspector

![Inspector](Images/inspector.png "Inspector panel")

(Visual improvements + less hardcoded).

---

### Units Control

![Units Control](Images/units_control.png "Units Control panel")

(New stuff. Unit spawn + selection, movement and harvesting).

---

## References

+ [4X Game (Phase 1) - Afonso Lage e André Santos][Phase 1]
+ [ScriptableObjects - Unity Documentation][Unity ScriptableObjects]
+ [4X Map Generator - Nuno Fachada][Generator]
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

[`GameStates`]:4XGameP2/Assets/Scripts/Enums/GameStates.cs
[`PanelsUserInterface`]:4XGameP2/Assets/Scripts/UserInterface/Panels/PanelsUserInterface.cs
[`UIStates`]:4XGameP2/Assets/Scripts/Enums/UIStates.cs
[`Controller`]:4XGameP2/Assets/Scripts/Controller.cs
[`IUserInterface`]:4XGameP2/Assets/Scripts/UserInterface/IUserInterface.cs
[`UIPanel`]:4XGameP2/Assets/Scripts/UserInterface/Panels/UIPanel.cs
[`PreStartPanel`]:4XGameP2/Assets/Scripts/UserInterface/Panels/UIPanelPreStart.cs
[`MapBrowserPanel`]:4XGameP2/Assets/Scripts/UserInterface/Panels/UIPanelMapBrowser.cs
[`MapFilesBrowser`]:4XGameP2/Assets/Scripts/Maps/MapFilesBrowser.cs
[`MapData`]:4XGameP2/Assets/Scripts/Maps/MapData.cs
[`MapFileWidget`]:4XGameP2/Assets/Scripts/UserInterface/Widgets/MapFileWidget.cs
[`UIWarnings`]:4XGameP2/Assets/Scripts/UserInterface/UIWarnings.cs
[`MapFileNameValidator`]:4XGameP2/Assets/Scripts/UserInterface/Widgets/MapFileNameValidator.cs
[`MapFileGeneratorWidget`]:4XGameP2/Assets/Scripts/UserInterface/Widgets/MapFileGeneratorWidget.cs
[`UpgradedScrollRect`]:4XGameP2/Assets/Scripts/Imported/UpgradedScrollRect.cs
[`GameTile`]:4XGameP2/Assets/Scripts/Maps/GameTile.cs
[`Resource`]:4XGameP2/Assets/Scripts/Maps/Resource.cs

[Phase 1]:https://github.com/andrepucas/lp2_4XGame_p1_2022
[Unity ScriptableObjects]:https://docs.unity3d.com/Manual/class-ScriptableObject.html
[Von Neumann]:https://en.wikipedia.org/wiki/Von_Neumann_neighborhood
[Moore]:https://en.wikipedia.org/wiki/Moore_neighborhood
[Generator]:https://github.com/VideojogosLusofona/lp2_2022_p1/tree/main/Generator
[Whitespace]:https://stackoverflow.com/questions/6219454/efficient-way-to-remove-all-whitespace-from-string/30732794#30732794
[Afonso Lage (a21901381)]:https://github.com/AfonsoLage-boop
[André Santos (a21901767)]:https://github.com/andrepucas
[Nuno Fachada]:https://github.com/nunofachada
[Bachelor in Videogames]:https://www.ulusofona.pt/en/undergraduate/videogames
[Lusófona University]:https://www.ulusofona.pt/en/
