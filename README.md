# Surface of Revolution Visualizer and Simple Character Animation
A Unity project exploring mesh generation, hierarchical animation, and AR, built for the University of Washington's CSE 457 (Computer Graphics) course.

## Acknowledgments

Built for CSE 457 at the University of Washington. Starter code, scene templates, and the curve editor UI were provided by the course staff.

## Overview

I implemented the surface-of-revolution mesh generation from scratch, letting users control the number of radial subdivisions to trade off between a smoother mesh and lower vertex count. Vertex normals are computed analytically rather than approximated, so the mesh shades correctly from any viewing angle, and the UV coordinates are set up with a duplicated seam ring so the texture wraps around the mesh cleanly instead of warping or pinching at the seam.

Using this surface-of-revolution mesh as a building block, I modeled a low-poly duck out of primitive shapes and one revolved body piece, then rigged it into a transform hierarchy so its parts (head, wings, body) animate together correctly under rotation. From there I built out a walking animation and a couple of other custom animations, triggered through UI buttons.

Finally, I exported the duck as a prefab and brought it into an ARCore scene, so it can be placed and viewed in the real world through an Android device — tapping to spawn it on a detected surface, then dragging, scaling, and rotating it with touch gestures.

