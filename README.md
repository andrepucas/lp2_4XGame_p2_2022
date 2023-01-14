# 4X GAME - PHASE 2

This **4X Game** phase begins where [Phase 1] ended, a Unity 2021.3 LTS game that
allows to generate, manipulate and pick a `.map4x` file from the Desktop to be
loaded and displayed as an interactive game map.

This repository implements different playable units that move around the map and
can harvest its resources, in turns.

Below, we'll revisit the main concepts from Phase 1, highlighting improvements
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
can move towards a map cell selected by the user, however not all move the
same way. As of now, there are 2 moving behaviours the units can adopt:

+ [Von Neumann]: ⇐ ⇑ ⇒ ⇓  
+ [Moore]: ⇐ ⇑ ⇒ ⇓ ⇖ ⇗ ⇘ ⇙

Furthermore, units can also **harvest resources** from the cell they are on and
even **generate new ones**. The resources each unit collects and generates can
be found in the image above. The Miner is the only unit that generates a resource
as of now (Pollution) whenever it successfully collects Metals.

---

## Code Architecture

![UML](Images/uml.png "Classes UML Diagram")

### Controller

Manages the game, handling Player Input and [`GameStates`]. It starts
by setting the `CurrentState` to `Pre Start`, which in turn delegates the
[`PanelsUserInterface`] to update its display, sending it the respective
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
ScriptableObjects through the editor, without having to touch any code,
complying with the **Open/Closed Principle**.

> For units, new UI add buttons need to be manually added and the global
> resource counters might need to be pushed forward, so that everything fits
> on the screen. (editor modification only, no need to change code).

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
right away.

Improved since the first version, if the maps dimensions can't be converted at
this stage, even with the new way of reading files (ignoring blank and completely
commented lines before trying to convert), then the map's dimensions aren't saved
at all, triggering an invalid map reaction later on.

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
name and dimensions, while referencing it. However, if its dimensions are null,
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

Before being ready to generate and display, the selected [`MapData`] needs to
convert its array of lines into a collection of [`GameTile`], a class that
represents a tile's terrain and contains a [`Resource`] collection.

> While in the previous version [`GameTile`] and [`Resource`] were abstract
> classes with subclasses for each type. We've since then come to the conclusion
> that it was unnecessary and only complicated our code. This approach has been
> completely replaced with the current [Unity ScriptableObjects] system.

The conversion is done by iterating all lines, starting at the line following up
the dimensions, which have already been handled, when [`MapData`] was instantiated.
In each line, it starts by looking for a `#`, by trying to get its index.
If it's greater than 0, then that line has a comment that needs to be ignored,
using a substring.

```c#
m_commentIndex = m_line.IndexOf("#");
if (m_commentIndex >= 0) m_line = m_line.Substring(0, m_commentIndex);
```

In our previous version, this substring's size was not being accounted for, meaning
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
[`MapData`]'s [`GameTile`] collection. In our previous version, it looked something
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
and looks for a raw name match (the name in lower case and no white spaces):

```c#
// Iterates collection of all valid game terrains.
for (int t = 0; t < p_gameData.Terrains.Count; t++)
{
    // If the first word matches a valid terrain name.
    if (m_lineStrings[0] == p_gameData.Terrains[t].RawName)
    {
        // Adds new game tile (initialized with preset data) to the collection.
        break;
    }

    // Increments control number of terrains checked.
    m_checkCount++;
}

// If the terrain wasn't found, increments lines to ignore.
if (m_checkCount == p_gameData.Terrains.Count)
    _linesToIgnore++;
```

If there are any other words in the string array, each represents a [`Resource`]
to add to the [`GameTile`] we just instantiated. Again, each keyword is compared
with all possible Resource names in the game and added accordingly.

In this version, if the supposed resource's name doesn't match any of the possible
resources, a control variable saves that not all resources were added and after
all lines are read, the map's validity is verified one last time:

```c#
// If the map's dimensions don't match the number of game tiles saved
// or if at least one resource couldn't be read, raise invalid data event.
if ((XCols * YRows) != GameTiles.Count || _failedResource)
    OnValidLoadedData?.Invoke(false);

// Otherwise, this map is valid to load.
else OnValidLoadedData?.Invoke(true);
```

If the map isn't considered valid, another UI warning pops up and the map isn't
generated.

![InvalidMapLoaded](Images/invalid_map.gif "Invalid map warning.")

---

### Map Display

![Map Display](Images/map_display.png "Main map display screen")

Once [`MapData`] has a valid [`GameTile`] collection and is successfully loaded,
the [`Controller`] sends it to [`MapDisplay`], responsible for generating the map.

The map is generated using the Unity's [Grid Layout Group] and [Content Size Fitter]
components. The only adjustments needed are setting the Grid Layout's cell size
and the column count constraint, both calculated with the map's size.

```c#
m_newCellSize.y = MAX_Y_SIZE / p_map.Dimensions_Y;
m_newCellSize.x = MAX_X_SIZE / p_map.Dimensions_X;

// Sets both the X and Y to the lowest value out of the 2, making a square.
if (m_newCellSize.y < m_newCellSize.x) m_newCellSize.x = m_newCellSize.y;
else m_newCellSize.y = m_newCellSize.x;

_cellSize = m_newCellSize.x;

// Updates the grid layout group.
_gridLayout.constraintCount = p_map.Dimensions_X;
_gridLayout.cellSize = m_newCellSize;
```

With the grid prepared, [`MapDisplay`] then iterates every [`GameTile`] in the
[`MapData`]'s list and instantiates a [`MapCell`] prefab for each, as a child of
the grid object. A [`MapCell`] represents an interactable game tile, holding its
terrain and resources sprites.

Once all are instantiated, [`MapDisplay`] then raises an event that makes the
[`Controller`] tell the [`PanelsUserInterface`] that it can now enable the
[`MapDisplayPanel`], rendering the map visible, and disabling the Grid Layout
and Content Size Fitter components, boosting performance by reducing automatic
component calls.

New in this phase, it's now displayed the total number of map resources at the top
of the screen. Each of these counters is instantiated when the map is generated,
one for each possible preset resource. This means that even if resources are
added or removed from the game, the counters above will adjust themselves
automatically.

```c#
// IN SETUP METHOD

// Iterates all possible resources' preset values.
foreach (PresetResourcesData f_rData in _presetData.Resources)
{
    // Instantiates a visual resource count object and updates its sprite..
    Instantiate(_mapResourceCount, _resourceCountFolder).
        GetComponentInChildren<Image>().sprite = f_rData.DefaultResourceSprite;
}

// IN RESOURCE COUNTERS UPDATE METHOD

// Variable that dictates which name to access.
int m_nameIndex = 0;

// Goes through each counter.
foreach (Transform f_counter in _resourceCountFolder)
{
    // Stores the TMP component.
    TMP_Text f_textComponent = f_counter.GetComponentInChildren<TMP_Text>();

    // Updates text to display number of said resources on the map.
    f_textComponent.text = _mapData.GameTiles
    .SelectMany(t => t.Resources)
    .Where(r => (r.Name.ToLower().Replace(" ", ""))
    .Equals(_presetData.ResourceNames.ToList()[m_nameIndex]))
    .Count().ToString();

    // Increases the variable so the next name is accessed.
    m_nameIndex++;
}
```

In this state, the map can be moved and zoomed in/out, using the key binds shown
on the bottom of the screen. The player's input is handled by the [`Controller`],
who then passes the directional info to the [`MapDisplay`] that tries to pan and
zoom using the camera's transform and orthographic size.

> In our previous version, we used the map's Rect Transform pivot to move and its
> scale to zoom. We thought this would be a good way to keep the zoom centered,
> while allowing for the map's edges to be masked. However, we were aware of how
> performant heavy this was, so for this phase we've decided to optimize it.

To successfully mask the map's edges with this camera approach however, we had
to:

+ Attach the background image to the Camera, so it would always follow it.
  + On a Screen Space Canvas, so that zooming in and out didn't affect the
background.
  + In a layer order lower than everything else, so it's always rendered first.
+ Attach a foreground image to the camera - the background, but with a hole in
the center.
  + In a layer order higher than the map, so it overlays it, but not higher
than corner UI elements such as buttons.
  + With empty raycast target game objects covering the edges, so that it also
masked mouse clicks.

![background and Foreground](Images/bg_fg.png "Background and Foreground side by side")

All [`MapCell`]s are hoverable and clickable by the mouse through Unity's
[IPointerHandlers], updating its base sprite to look hovered and raising
two events when clicked. One triggers the [`Controller`] to display the
[`InspectorPanel`], while the other sends out the data needed to inspect.

---

### Inspector

![Inspector](Images/inspector.png "Inspector panel")

The [`InspectorPanel`] is responsible for displaying the clicked [`MapCell`]'s
properties. It does so by syncing its name, coin and food values, plus the
terrain and resources sprites with the clicked cell. It also displays
text components accordingly to the shown resources, to add extra visual info.
This written info is equal for every tile, since the Coin and Food values of
resources and terrains don't change. The only values that vary are the
[`GameTile`]'s totals.

Merely a "show" type of panel, when the user presses `escape` or clicks away,
the [`Controller`] updates its `CurrentState` to `Gameplay` and closes this panel.

---

### Units Control

![Units Control](Images/units_control.png "Units Control panel")

Units are the main thing we've got to show for this phase. A [`Unit`] itself
is a mix between a [`GameTile`] and a [`MapCell`], since it contains
data, like its type and resources but can also be interacted with through
[IPointerHandlers].

#### Spawn

Added to the map through the 4 small button at the top of the screen,
in the [`GameplayPanel`]. Units are placed in a separate Canvas, overlaying the
map, on top of a map cell. However, units and map cells have no dependencies
between each other. A unit doesn't have a reference to the cell it's standing on
and a cell doesn't have a reference to the unit that is standing on it. Instead,
all of this info is stored in the [`OngoingGameData`] ScriptableObject, which
contains two dictionaries relative to the map cells and units and methods that
allow to manipulate them.

```c#
// Readonly dictionary that stores all cells and respective map positions.
public IReadOnlyDictionary<Vector2, MapCell> MapCells => _mapCells;

// Readonly dictionary that stores all units and respective map positions.
public IReadOnlyDictionary<Vector2, Unit> MapUnits => _mapUnits;
```

This way, when a unit is being spawned in on a random map position, for example,
it can access the MapUnits dictionary to checks if that position is free.

```c#
// Finds a random map position that doesn't have a unit in it.
do
{   m_randomMapPos = new Vector2(
        UnityEngine.Random.Range(0, _mapData.XCols),
        UnityEngine.Random.Range(0, _mapData.YRows));
}
while (_ongoingData.MapUnits[m_randomMapPos] != null);
```

If the position is free, the unit is instantiated the same way terrains and
resources were, through the preset game data, and is added to the MapUnits
dictionary.

#### Selection

A single unit can be selected by clicking it or multiple units can be selected at
once by either using `CTRL` + Click to select more or by holding down the mouse
and drag box selecting. Right clicking deselects all units. The input for this is
managed by the [`Controller`], naturally, however the behaviours for each input
are managed by the [`UnitSelection`] class. Responsible for drawing the selection
box and managing which units are selected or not.

It does so by updating the size of the box, checking for units within its
bounds and managing 3 collections:

```c#
// Private collection containing all spawned units.
private IList<Unit> _unitsInGame;

// Private collection containing all hovered units.
private ISet<Unit> _unitsHovered;

// Private collection containing all selected units.
private ISet<Unit> _unitsSelected;
```

When the mouse is released, hovered units become selected and an event is raised
containing the collection of selected units. If there are any, the [`Controller`]
reveals this [`UnitsControlPanel`], which displays all relevant information
about the selected units and contains action buttons.

The displayed information includes the type or number of selected units, its
icons and collected resources. The display itself is flexible with the attention
to the number of selected units:

+ 1 unit is selected: Displays type + singular unit selected expression.
+ 2+ units selected: Displays count + plural units selected expression.
+ 15+ units selected: Adjusts icons display to hide the oldest ones, showing how
many are hidden.

![UnitSelection](Images/unit_selection.gif "Selecting units")

At the bottom of the panel there are 3 action buttons that allow the user to
control the selected units. With the exception of the Remove action, which simply
removes them from the map, each unit action advances 1 game turn.

#### Movement

Once the movement button is pressed, the game changes its cursor and enters a
selection mode, toggling off all other buttons and disabling normal inspection
and selection input, allowing only for the user to click on a map cell to choose
as the units target. This mode is toggled off if a cell is selected or
if the user presses the right mouse button.

If a cell is selected, an animated target is instantiated at that position and
the units start moving towards it, one cell at a time, incrementing one turn
every move. As was mentioned earlier, units have 2 different move types: Von
Neumann and Moore, and will stop moving if they come across any obstacle, like,
other units in their way.

![UnitMovement](Images/unit_movement.gif "Moving units")

```c#
// While there are moving units.
while (m_movingUnits.Count > 0)
{
    // Iterates every moving unit.
    foreach (Unit f_unit in m_movingUnits)
    {
        // Saves unit's next move towards destination.
        m_nextMove = f_unit.GetNextMoveTowards(p_targetCell.MapPosition);

        // If move is valid, move units.

        // If the unit didn't move, add it to blocked units collection.
        m_blockedUnits.Add(f_unit);
    }

    // Removes blocked units from moving units collection.
    m_movingUnits.ExceptWith(m_blockedUnits);

    // Clears blocked units.
    m_blockedUnits.Clear();

    // Waits for units to move and ends turn.
    if (m_movingUnits.Count > 0)
    {
        yield return m_waitForUnitsToMove;
        OnNewTurn?.Invoke();
    }
}
```

This works because each unit has a movement behaviour that implements
[`IUnitMoveBehaviour`], an interface with one method signature, that each
specific movement type then executes as they see fit (**Strategy Pattern**).
Movement behaviours are attributed to each unit type directly in the
[`PresetGameData`] ScriptableObject, using empty prefabs and then getting the
attached script reference, like so:

```c#
// PRESET UNITS DATA STRUCT

// Serialized movement type prefab with script.
[SerializeField] private GameObject _moveBehaviour;

// Move Behaviour property.
public IUnitMoveBehaviour MoveBehaviour => _moveBehaviour.GetComponent<IUnitMoveBehaviour>();
```

#### Harvesting

The harvest button is only toggled on when the at least one of the selected units
is standing on a cell with resources it can collect. Collectable resources for
each cell are pre-set as name strings, which are compared with the names
of the resources in the cell they're on, every time the buttons are updated.

```c#
// Enables harvest button if not moving and there are resources to harvest.
_harvestButton.interactable = !(_isSelectingMove || _isMoving) && SelectedUnitsCanHarvest();
```

When the button is pressed, all selected units attempt to harvest, comparing its
strings of collectable resources with the tile's. If the resource is found, it's
removed from the tile and added to the unit's collection, to be displayed in the
panel. Furthermore, if the unit successfully harvests a resource, it will play
a feedback animation and, if it can produce any resource that doesn't already
exist in the cell, it will simply add it to the cell.

![UnitHarvest](Images/unit_harvest.gif "Unit harvesting")

---

## References

+ [4X Game (Phase 1) - Afonso Lage e André Santos][Phase 1]
+ [4X Map Generator - Nuno Fachada][Generator]
+ [ScriptableObjects - Unity Documentation][Unity ScriptableObjects]
+ [Regex Class - Microsoft Docs][Regex]

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
[`MapDisplay`]:4XGameP2/Assets/Scripts/Maps/MapDisplay.cs
[`MapDisplayPanel`]:4XGameP2/Assets/Scripts/UserInterface/Panels/UIPanelMapDisplay.cs
[`MapCell`]:4XGameP2/Assets/Scripts/Maps/MapCell.cs
[`InspectorPanel`]:4XGameP2/Assets/Scripts/UserInterface/Panels/UIPanelInspector.cs
[`Unit`]:4XGameP2/Assets/Scripts/Units/Unit.cs
[`GameplayPanel`]:4XGameP2/Assets/Scripts/UserInterface/Panels/UIPanelGameplay.cs
[`OngoingGameData`]:4XGameP2/Assets/Scripts/ScriptableObjectsData/OngoingGameDataSO.cs
[`UnitSelection`]:4XGameP2/Assets/Scripts/Units/UnitSelection.cs
[`UnitsControlPanel`]:4XGameP2/Assets/Scripts/UserInterface/Panels/UIPanelUnitsControl.cs
[`IUnitMoveBehaviour`]:4XGameP2/Assets/Scripts/Units/MoveBehaviors/IUnitMoveBehaviour.cs
[`PresetGameData`]:4XGameP2/Assets/Scripts/ScriptableObjectsData/PresetGameDataSO.cs

[Phase 1]:https://github.com/andrepucas/lp2_4XGame_p1_2022
[Regex]:https://learn.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex?view=net-7.0
[Unity ScriptableObjects]:https://docs.unity3d.com/Manual/class-ScriptableObject.html
[Von Neumann]:https://en.wikipedia.org/wiki/Von_Neumann_neighborhood
[Moore]:https://en.wikipedia.org/wiki/Moore_neighborhood
[Generator]:https://github.com/VideojogosLusofona/lp2_2022_p1/tree/main/Generator
[Grid Layout Group]:https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-GridLayoutGroup.html
[Content Size Fitter]:https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-ContentSizeFitter.html
[IPointerHandlers]:https://docs.unity3d.com/2018.2/Documentation/ScriptReference/EventSystems.IPointerClickHandler.html
[Afonso Lage (a21901381)]:https://github.com/AfonsoLage-boop
[André Santos (a21901767)]:https://github.com/andrepucas
[Nuno Fachada]:https://github.com/nunofachada
[Bachelor in Videogames]:https://www.ulusofona.pt/en/undergraduate/videogames
[Lusófona University]:https://www.ulusofona.pt/en/
