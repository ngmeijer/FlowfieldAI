# PersonalPortfolio_Q4

When I was little, I used to play a browser-based game a lot called "Pixel Legions". It was a strategy game with very simple graphics, where you would play as a certain colour.
You'd have to conquer terrain and strategically eliminate all enemy forces on the map.

I was curious about how the AI pathfinding worked for that game, since it looked like every single pixel was able to find its own path. After research, I found it was done by using so-called "flowfields", which is a way to do pathfinding for a large amount of entities. Instead of calculating a path for every single enemy (which can go into the 1000's, if every "pixel" is an enemy), a path to the target would be calculated from every cell, which could be much cheaper depending on how large the level is. 

To increase performance even more, the level could be divided into an octree, depending on the location of enemies so that we don't waste resources on empty cells.

[Project demonstration](https://www.youtube.com/watch?v=6RQRLQWzzUk&t=5s)
