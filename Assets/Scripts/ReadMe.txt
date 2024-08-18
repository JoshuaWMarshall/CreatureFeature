GitHub Repository -

https://github.com/JoshuaWMarshall/CreatureFeature


README: Game/Simulation Setup and Usage

Overview

Welcome to our game/simulation! This document provides instructions for setting up and using the tools available in our demo scene. Follow the steps below to get 	started and customize your environment with terrain, trees, and dinosaurs.

Setup

Opening the Demo Scene
- Start by opening the demo scene in Unity.

Accessing Tools
-Navigate to the Tools tab in the Unity editor.
-Open the following tools: Terrain Generation, Tree Placement, Dinosaur Placement

Terrain Generation Window
-In the Terrain Generation window, click the Find References button.
-Click the Generate Noise button to create a noise map.
-Drag the prefab for the water mesh from the Prefabs folder into the designated field.

Modify Terrain Parameters
- Randomise Seed: Select whether to use a random seed (true/false) or input a custom seed.
- Noise Scale, Lacunarity, Octaves: Adjust these values to modify the noise map, which affects the terrain heightmap.
- Gradient: Modify the gradient to color the mesh.

Set values for:
- xSize and zSize
- Mesh Scale
- Height Multiplier
- Water Level: Set the height of the water level.

Click the Generate Terrain button to create the terrain mesh based on the parameters you've set.

Tree Placement Window
- In the Tree Placement window, click the Find References button.
- Click the Generate Noise Texture button to create a noise map for tree placement. This noise map is different from the terrain noise map.
- Drag the tree prefab from the Prefabs folder into the designated field.

Modify Tree Parameters
- Noise Scale, Lacunarity, Octaves: Adjust these values to modify the noise map for tree placement.
- Intensity: the threshold to determine if a tree will spawn based on the noise map colour value.
- Randomness: Adjust the amount of random offset applied to tree placement.
- Max Steepness: Define the maximum slope where trees can be placed.


After configuring the values, click the Place Trees button.

Dinosaur Placement Window
- In the Dinosaur Placement window, click the Find References button.
- Drag the prefabs for the Stegosaurus and Velociraptor from the Prefabs folder into the designated fields.
- Specify the number of each type of dinosaur you want to place.

Click the Place Dinosaurs button to spawn the dinosaurs in the scene.

Simulation
- Hit Play in Unity to start the simulation.
- A camera will focus on a dinosaur, and you will see a UI displaying the dinosaur’s needs (hunger, thirst, energy).
- Switch Between Dinosaurs
        Use the dropdown menu to switch between different dinosaur types.
        Navigate between dinosaurs using the Next and Back buttons.
        The camera will follow the selected dinosaur.

Camera Controls
- Rotate Camera: Hold the right mouse button to rotate the camera.
- Zoom: Use the mouse wheel to zoom in and out.