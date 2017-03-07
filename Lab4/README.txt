Lab 4 by Gerard Puhalla and Zachary Schroeder

NOTE: This project was set up using tags to label things
Since tags are not included in the unitypackage export, I have submitted the TagManager.asset file along with the unity project
The TagManager.asset file must be placed in the ProjectSettings Folder for the project or the game will crash when a prey dies

Predators, represented by the red/magenta spheres, chase Prey, represented by the blue/cyan spheres, who will try to run away when spotted. Prey have slightly better vision and can run away faster.

Both predators and prey use field of vision to test if they see anything. We use ray cast lines which are show as lines on the board. Rays that hit an object turn green to show that the agent sees something.

Both avoid the fixed obstacles and walls in the arena they start in.

Prey that see a predator turn cyan colored and run away with more speed for a short sprint.

Predators that see a prey turn magenta colored and chase.

Agent motion
Predators chase prey when detected, prey try to avoid predators - 4/4 points
Agent vision - 4 points possible
Field of vision for predators, your choice of vision model for prey - 4/4 points

Additional features
Add fixed obstacles in the environment that the creatures cannot move through + 1
Add fixed obstacles in the environment that the creatures cannot see through + 1

Add a visualization that accurately shows the field-of-view of the agents. +1

