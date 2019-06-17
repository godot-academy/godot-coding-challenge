# Menger Sponge Fractal
## [Coding Challenge](https://www.youtube.com/watch?v=LG8ZK-rRkXo&list=PLRqwX-V7Uu6ZiZxtDDRCi6uhfTH4FilpH&index=2)

A Menger Sponge Fractal is a pretty straightforward fractal - you take a cube (or a square if you live in two dimensions) and then you split it into 9 segments. You then take the center segment(s) and remove them. This leaves you with a 'sponge' structure that has holes throughout it.
When you split the fractal agian, you take every 'full' segment, and repeat the process. Since this is recursive, and creates quite a large number of shapes, you can quickly run into performance issues.

However, we combat this in Godot by utilizing a MultiMeshInstance/MultiMesh node combo. We can instantiate a large number of 'identical' meshes, in this case the cube, and have them all rendered in 'one go' on the GPU. This gives us less control over each mesh (you really only get to edit their transform and a few other things) but this is plenty for making the fractal work. We add an exponential number of cubes to the MultiMesh, and get very good performance. Eventually it's the process of splitting the segments, not rendering them, that eats up processing time.

Overall it has a very cool effect for how simple it is. I also added the ability to make the 'inverse' sponge so you can see how the holes are all connected.

# Example
![Making a Menger Sponge Fractal](./menger_sponge.gif)
