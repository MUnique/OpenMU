# GameMap

I want to give a quick description about how the [game map](../src/GameLogic/GameMap.cs),
especially observing between players/npc works.

## AreaOfInterestManager

Each game map has an area of interest manager. It takes care of the event
subscriptions between objects, in case they were added, moved or removed at the
map.

The default implementation ([BucketAreaOfInterestManager](../src/GameLogic/BucketAreaOfInterestManager.cs))
creates a BucketMap, which is described below.
Other (simpler) implementations may be possible for the future for maps which
are more likely to be "empty" or require that all objects of a map need to know
all other objects (e.g. Duel map).

## BucketMap

The [bucket map](../src/GameLogic/BucketMap{T}.cs) is basically a two-dimensional
data structure.
We split the map of 256 x 256 possible coordinates into "[buckets](../src/GameLogic/Bucket{T}.cs)".
For example, each bucket covers 8 x 8 coordinates.

A player (or other "observers") can observe buckets, so that it gets informed
when objects (other players, npc, dropped items) enter or leave this bucket.
When a player moves and comes into the range of a bucket (defined by an
"info range"), it subscribes this enter/leave events and adds all existing
objects of this bucket to his view.

You may ask, why I introduced such a structure and not simply use a list of
objects and compare the distance to them when moving to decide if a object
should be sent (add/remove) to the game client.
As far as I know, the original server works like that and has a lot of lag when
a lot of players (more than thousand at castle siege) are on the same map, all
moving around.
This is a O(n²) problem where n is the number of objects on the same map - this
can get pretty complex very fast.

Now with my concept, this search is only happening when a player enters a new
bucket - and then it searches only for new buckets in range, not for other
individual objects.
So this is not as complex when you have many objects on your map. Of course,
this needs a bit more memory and is slower on empty maps, but IMHO that's
really worth it - usually, game maps are packed with monsters, players and a
lot of dropped items.

## Other ideas

Instead of partitioning a map into buckets (where a lot of them are empty), we
could try to use some other 2d index structures, like
[R-trees](https://en.wikipedia.org/wiki/R-tree) or [B-Trees](https://en.wikipedia.org/wiki/B-tree)
with [Z-Ordering](https://en.wikipedia.org/wiki/Z-order_curve).
