Lab 6 by Zachary Schroeder and Gerard Puhalla 

Our Lab focuses on inverse kinematics. In the unity scene, there is a gold crane arm at the center, a red boxed area to the side of the crane, and red spheres that generate around the crane.
The basic function of the scene is the crane arm moving to one of the red spheres, picking it up, and then depositing it in the box.

The crane uses the Cyclic Coordinate Descent algorithm, and interpolates between each sub-position in the algorithm to show how the algorithm solves the problem. The crane will try 10 times to get a sphere and then give up (marking it red) and moving to the next ball.

The crane uses linear interpolation to interpolate between the arm angles.

The crane has 2 different methods of sphere collection, an automatic method, and a user input method.

The crane defaults to the automatic method, where the crane moves and puts all the spheres in the box by itself.

However, should a user click on a specific sphere, the crane will move that sphere to the box. This serves as the user input method.

These two articles about Cyclic Coordinate Descent and Inverse Kinematics were used in the creation of this project:
http://www.darwin3d.com/gamedev/articles/col1198.pdf
https://sites.google.com/site/auraliusproject/ccd-algorithm

This has been a great semester thanks for grading all these projects. I hope you enjoy our animation!