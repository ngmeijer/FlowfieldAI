# PersonalPortfolio_Q4

When I was little, I used to play a browser-based game a lot called "Pixel Legions". It was a strategy game with very simple graphics, where you would play as a certain colour.
You'd have to conquer terrain and strategically eliminate all enemy forces on the map.

I was curious about how the AI pathfinding worked for that game, since it looked like every single pixel was able to find its own path. After research, I found it was done by using so-called "flowfields", which is a way to do pathfinding for a large amount of entities. Instead of calculating a path for every single enemy (which can go into the 1000's, if every "pixel" is an enemy), a path to the target would be calculated from every cell, which could be much cheaper depending on how large the level is. 

To increase performance even more, the level could be divided into an octree, depending on the location of enemies so that we don't waste resources on empty cells.

[Project demonstration](https://www.youtube.com/watch?v=6RQRLQWzzUk&t=5s)

Here, you can see a flowfield without any calculations applied to it and AI agents placed on random cells.
<img src="https://github.com/ngmeijer/FlowfieldAI/assets/58357808/e788b1c3-1c36-4448-aa34-3fcb3621ab69" width="300" height="300">

Now you can see the direction of each cell. When an agent collides with a cell, it requests that direction and applies that vector to its own velocity. This means an agent does not need to calculate its own path, but rather just "follows the arrows on the ground".
<br>
<img src="https://github.com/ngmeijer/FlowfieldAI/assets/58357808/7df3a520-6f18-48df-b320-e47be9bc51f1" width="300" height="300">
