# GameMap / Mapa del Juego

*Read this document in [English](#english) or [Español](#espanol).* 

<a id="english"></a>
## English

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

<a id="espanol"></a>
## Español

Quiero dar una rápida descripción de cómo funciona el [game map](../src/GameLogic/GameMap.cs),
especialmente la observación entre players y npc.

## AreaOfInterestManager

Cada game map tiene un area of interest manager. Se encarga de las
suscripciones a eventos entre objetos, en caso de que se agreguen, muevan o
eliminen en el map.

La implementación por defecto ([BucketAreaOfInterestManager](../src/GameLogic/BucketAreaOfInterestManager.cs))
crea un BucketMap, que se describe abajo. Otras implementaciones más simples
podrían ser posibles en el futuro para maps que probablemente estén vacíos o
requieran que todos los objetos de un map conozcan a todos los demás objetos
(por ejemplo, el Duel map).

## BucketMap

El [bucket map](../src/GameLogic/BucketMap{T}.cs) es básicamente una estructura
de datos bidimensional. Dividimos el map de 256 x 256 coordenadas posibles en
"[buckets](../src/GameLogic/Bucket{T}.cs)". Por ejemplo, cada bucket cubre
8 x 8 coordenadas.

Un player (u otros "observers") puede observar buckets para ser informado
cuando objetos (otros players, npc, items dropped) entren o salgan de ese
bucket. Cuando un player se mueve y entra en el rango de un bucket (definido
por un "info range"), se suscribe a estos eventos de entrada/salida y agrega
todos los objetos existentes de ese bucket a su view.

Podrías preguntarte por qué introduje esta estructura y no simplemente usar una
lista de objetos y comparar la distancia al moverse para decidir si un objeto
debería enviarse (add/remove) al game client. Según sé, el server original
funciona así y tiene mucho lag cuando hay muchos players (más de mil en castle
siege) en el mismo map, todos moviéndose. Es un problema O(n²) donde n es el
número de objetos en el mismo map, lo cual se vuelve complejo rápidamente.

Con este concepto, la búsqueda solo ocurre cuando un player entra en un bucket
nuevo, y entonces busca solo nuevos buckets en rango, no otros objetos
individuales. Así no es tan complejo cuando tienes muchos objetos en tu map. Por
supuesto, esto requiere un poco más de memoria y es más lento en maps vacíos,
pero en mi opinión vale la pena; usualmente los game maps están llenos de
monsters, players y muchos items dropped.

## Otras ideas

En lugar de dividir un map en buckets (donde muchos están vacíos), podríamos
usar otras estructuras de índice 2D, como
[R-trees](https://en.wikipedia.org/wiki/R-tree) o [B-Trees](https://en.wikipedia.org/wiki/B-tree)
con [Z-Ordering](https://en.wikipedia.org/wiki/Z-order_curve).
