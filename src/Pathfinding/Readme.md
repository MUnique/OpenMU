# Pathfinding

This projects includes an a-star pathfinding algorithm, specifically for maps
which have a maximum size of 256 x 256. This limitation arises by using byte
fields for the coordinates, which saves some memory.
There are a few heuristics available but by default the PathFinder uses NoHeuristic
which is basically equal to using the Dijkstra algorithm.

## Priority Queue

There are two implementations of a priority queue of the Open-List: BinaryMinHeap
and IndexedLinkedList.
I suggest using the BinaryMinHeap because it's commonly used and it's hard to get
IndexedLinkedList working faster under real circumstances.
The reason is, that it's pretty hard to get the index fast under all conditions,
because the expected open list lengths and estimated costs are always different.

## Scoped

The implementation can be used in a scoped way, which means that the pathfinder
is only used for a scoped area of the map. This is useful if you want to calculate
paths very quickly and you know that the path is only needed in a small area.
You can read about that on my blog post: [Optimized Pathfinding](https://munique.net/optimizing-pathfinding/).

## Safezones

Safezones are areas on the map where usually no path should be calculated, except
for special NPCs like guards. By default, the pathfinder will not calculate paths
on the safezone tiles. You can change this behavior by passing the parameter
`includeSafezone`. The safezones are encoded into the grid cost values as the
highest bit.

## Pre-Calculation

In the sub-folder PreCalculation includes a pathfinder which makes use of
pre-calculated paths. It's not used yet by OpenMU and needs some further testing.

For more information, visit <http://www.gamedev.net/page/resources/_/technical/artificial-intelligence/precalculated-pathfinding-revisited-r1939>.

## Further optimizations

If we find out that the current implementation is too slow, we could implement
[Jump Point Search](http://www.gdcvault.com/play/1022094/JPS-Over-100x-Faster-than).
