# Space Invaders
## [Coding Challenge](https://www.youtube.com/watch?v=biN3v3ef-Y0)

Space Invaders is a classic arcade game where the player defends themselves by an ever encroaching army of invading spaceships. The player can move and fire shots to destroy them, but must also be careful not to get hit themselves. Since the invaders are moving, and bullets are not instant, they will also have to lead their shots to make sure they land.

This challenge does not intend to be a faithful, accurate, or even fun re-creation of the original Space Invaders game. Instead, it intends to accomplish a few objectives:

1. Responds to player input
1. Invaders can move and go faster as time goes on
1. Players can launch bullets to destroy invaders
1. Players can be destroyed if they fail to eliminate the invaders

You'll note that there isn't anything related to levels, scoring, or even simple things like lives or restarts. There are many features missing to make this a fun, exciting game, but the nature of these challenges is to make things 'quick and dirty' so you can see how they are done in Godot. I highly recommend you use this as a base to add many arcade feaures, custom art, and make a cool game!

# Implementation
As far as games go, this is a very simple one. However, we take advantage of Godot in a number of ways to make things even simpler.

## Player
The player responds to input via the project's Input Map. This input map designates certian actions and what keypresses actuate those actions. By detecting things like left and right movement, as well as shooting a bullet, the player can move itself and produce bullet entities to launch at the invaders.

## Invaders
Invaders are quite simple - they move one direction until they hit a wall. When they hit the wall, they move towards the player, speed up, and switch directions. A great upgrade would be to have invaders launch their own bullets back at the player to destroy them!

## Collision and Detection
The main advantage we get in this challenge from Godot is the usage of the Physics engine to handle collisions. We don't need the physics engine to do any complicated kinematics (objects hitting, bouncing, rotating, etc.) Instead we use the physics engine to simply utilize it's [`Area2D`](https://docs.godotengine.org/en/latest/classes/class_area2d.html) nodes.

The `Area2D` includes a very basic [`CollisionShape2D`](https://docs.godotengine.org/en/latest/classes/class_collisionshape2d.html). Then, we connect to the `Area2D` `area_entered` signal to detect when one of these areas collide with another.

Each entity in this scene uses another helpful Godot feature: [`Groups!`](https://docs.godotengine.org/en/3.1/getting_started/step_by_step/scripting_continued.html#groups) These groups allow us to check what exactly the `Area2D` collided with - was it an enemy bullet? Did the invader reach the player? Did a shot land and the invader needs to be destroyed? These decisions are done with the `is_in_group()` function to determine who hit what.

## Game Scene
All the game scene does is define the boundaries of the game board (using the same `Area2D` technique described above) and also fills the board with a grid of Invaders. Eventually the game controller could do things like track score, lives, or even have multiple levels.

# Example
![Space Invaders!](./space_invaders.gif)
