# Mitosis
## [Coding Challenge](https://www.youtube.com/watch?v=jxGS3fKPKJA)

Although not really a Mitosis simulation, this does 'simulate' having cells taht can be split into two smaller cells, and the process can repeat indefinitely. This is actally a very simple simulation of very simple objects - cells (which are just a colored cirlce) can react to input (mouse motion in this case) and split into two smaller, similarly colored cells. By putting a large number of cells on the board, with many vibrant colors, you can get a cool little toy to watch.

# Implementation
## Cells
Cells are a simple `Area2D` node with a basic collision shape attached. The cell knows how to draw itself (simply a `draw_circle()` call) and can 'jiggle' by randomly adjusting it's position. If not provided, the cell will generate a random color. When the user mouses over the `Area2D`, it emits an `input_event` signal that can be used later.

## Mitosis Simulation
The simulation itself is really just a large collection of cells positioned randomly on the screen. The simulation listens for every cell's `input_event` signal, and if mouse movement is detected then the cell is split into two. Splitting is simply removing the old cell, and creating two smaller cells with slightly adjusted colors in it's place.

# Example
![Colorful Mitosis!](./mitosis.gif)
