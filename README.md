## CellularAutomata

Cette classe implémente une méthode de génération procédurale basée sur un automate cellulaire, dans l'esprit du jeu de la vie de Connway. Son rôle est de remplir une grille avec deux type de cellules : terre et eau, et d'obtenir progressivement une carte cohérente à partir d'un bruit initial.

### Fonctionnement général

L'algorithme repose sur un principe simple : pour chaque cellule, on observe ses 8 voisines.
Si au moins **X** d'entre elles sont de type *TERRE*, alors la cellule observé devient elle aussi *TERRE*.
En répétant cette opération plusieurs fois, la grille s'uniformise et forme des masses de terrain naturelles.

---

1. Initialisation

La classe initialise deux buffers, `currentBuffer` et `nextBuffer`, tous les deux sont de la taille de la grille.
Ils permettent de calculer l'état futur des cellules sans modifier immédiatement l'état courant.

---

2. Bruit initial

La méthode `CreateNoise` remplit `currentBuffer` avec un mélange de cellules eau/terre de manière aléatoire.
Le ratio de terre est déterminé par `GroundRatio` qui contrôle la proportion de terre initiale.

---

3. Affinage par itération

Pour chaque étape du processus:

- On calcule `nextBuffer[x, y] = SurroundedByGround(x, y)`.
- `SurroundedByGround` compte le nombre de cellules voisines de type *TERRE*.
  Si ce nombre est supérieur ou égal à `GroundSisterNeeded`, la cellule devient *TERRE*.
- Si une cellule change d'état entre les deux buffers, son apparence est mise à jour.
- A chaque itération, les buffers sont swap, on attend `GridGenerator.StepDelay` (délai pour mieux visualiser chaque itération), puis on recommence.

Le tout est répété `_maxSteps` fois.

---

4. Résultat

Au fil des itération, le bruit initial se stabilise et donne naissance à des zones de terre cohérentes entourées d'eau.
L'algorithme permet ainsi de générer des formes naturelles à partir d'un simple bruit.

![cellularAutomataGIF](https://i.imgur.com/ibB0hHK.gif)

---

## NoiseGeneration

Cette classe implémente une méthode de génération procédurale basée sur du bruit **OpenSimplex2**. Son rôle est de transformer une grille vide en une carte composée de plusieurs types de terrains (herbe, eau, sable, et pierre) en fonction d’une valeur de bruit allant de -1 à 1.

---

### Fonctionnement général

Pour chaque cellule de la grille, on appelle la méthode `noise.GetNoise(x, y)` afin d'obtenir la valeur de hauteur (comprise entre -1 et 1) à une coordonnée précise. Cette valeur est ensuite comparée à différents seuils (`WaterHeight`, `SandHeight`, `GroundHeight`, `HillHeight`) qui permettent de déterminer quel type de cellule doit être posée. Le tout donne une carte naturelle avec des zones et des transitions cohérentes entre les "biomes".

---

1. Fonctionnement général

La méthode commence par créer une instance de **FastNoiseLite** en utilisant la seed fournie par `RandomService`. Le bruit utilisé est du **OpenSimplex2**, un bruit rapide et fluide, idéal pour de la génération de terrain.

Comme dit précédemment, chaque appel à `noise.GetNoise(x, y)` renvoie une valeur entre -1 et 1 qui représente la hauteur sur le bruit (1 = blanc / top, -1 = noir / bottom).

---

2. Parcours de la grille

L’algorithme parcourt chaque coordonnée X et Y de la grille, et pour chaque cellule :

- On récupère la cellule correspondante.
- On récupère la valeur de bruit.
- On choisit le type de cellule à déposer en fonction des seuils.

Ces seuils définissent la répartition des biomes. Plus la valeur de bruit est basse, plus la zone tend vers l’eau ; plus elle est haute, plus on s’approche des zones rocheuses (cela s'applique uniquement à ma configuration, mais l'inverse et tout à fait possible).

---

3. Attribution des cellules

La valeur `noiseValue` est comparée dans l’ordre aux seuils :

- Si elle est inférieure à **WaterHeight**, la cellule devient *eau*.
- Entre **WaterHeight** et **SandHeight**, c’est du *sable*.
- Entre **SandHeight** et **GroundHeight**, c’est de l’herbe.
- Au-dessus de **GroundHeight**, on place de la roche.

Le type de la cellule est ensuite utilisé dans la méthode `AddTileToCell`, qui instancie la cellule sur la grille.

---

4. Résultat

Une fois la grille parcourue, la carte affiche une répartition naturelle des terrains : des zones d’eau entouré par du sable, des plaines d’herbe, et quelques rochers. Grâce au bruit **OpenSimplex2**, les formes sont organiques et variées, sans motifs répétitifs.

![openSimplex2GIF](https://i.imgur.com/2BbgxZj.gif)

---

## SimpleRoomPlacement

Cette classe implémente une méthode de génération simple basée sur le placement aléatoire de salle et la création de couloirs qui les relient. L'objectif est d'obtenir une structure de type "donjon" (plusieurs pièces séparés, connectées par des couloirs).

---

### Fonctionnement général

L'algorithme suit deux grandes étapes :

1. Placer progressivement des salles rectangulaire de taille fixe sur la grille, en évitant qu'elles se chevauchent.
2. Relier les salles entre elles à l'aide de couloirs en forme de L, en connectant les centres de chaque salle dans l'ordre de création.

Une fois ces étapes terminées, un sol uniforme est construit sur toute la grille afin de servir de base visuelle au donjon.

---

1. Placement des salles

La méthode commence par créer une liste vide `roomList` qui stockera les salles validées. Ensuite, elle répète jusqu'à `_maxSteps` fois la tentative suivante :

- Choisir une position aléatoire X;Y dans la grille.
- Construire un rectangle `RectInt room = new RectInt(x, y, 10, 10)` représentant une salle de taille 10x10.
- Vérifier si cette salle peut être placée via `CellAvailable(room)` :
  - La méthode s'assure que toutes les cellules du rectangle (+ une bordure autour) soient libre.
  - Si une cellule est hors grille ou déjà occupé (`cell.ContainObject`), la salle est rejetée.
- Si la salle est validé :
  - `PlaceRoom(room)` remplit la zone avec des cellules de type *SALLE*.
  - La salle est ajoutée à `roomList`.
  - Le délai `GridGenerator.StepDelay` est appliqué pour visualiser l'apparition de la salle.

Cette phase produit un ensemble de salles séparé, et espacées les unes des autres.

---

2. Connexion des salles

Une fois les salles placées, l'algorithme parcourt la liste `roomList` et connecte chaque salle avec la suivante.

- Pour chaque paire de salles consécutives, on récupère leurs centres via `roomList[i].GetCenter()` et `roomList[i + 1].GetCenter()`.
- La méthode `ConnectRooms(a, b)` trace un couloir en deux temps :
  - D'abord une ligne horizontale entre `a.x` et `b.y` sur la ligne `a.y`.
  - Puis une ligne verticale entre `a.y` et `b.y` sur la colonne `b.x`.
- A chaque cellule traversée, une cellule de type *CORRIDOR* est posée.

Cela crée des couloirs en forme de **L** qui relient chaque salle à la suivante, formant un chemin continu à travers le donjon.
Entre chaque connexion, un délai (`GridGenerator.StepDelay`) permet de voir les couloirs se dessiner progressivement.

Une fois les salles et les couloirs dessinés, un sol est créé à son tour.

---

4. Résultat

Au final, cette méthode produit un donjon composé :

- De plusieurs salles rectangulaires de taille identique, dispersées aléatoirement mais sans se chevaucher.
- De couloirs en forme de **L** qui relient les salles entre elles, garantissant une structure navigable.
- D'un sol uniforme recouvrement toute la grille, servant de base à l'ensemble.

Cet algorithme offre une base simple pour générer des donjons procéduralement.

![SimpleRoomPlacementGIF](https://i.imgur.com/qbyXUih.gif)

---

## BSP (Binary Space Partitioning)

Je n'ai pas réussi à coder cette méthode, mais j'ai quand même compris le principe.

---

Le BSP est une méthode récursive, on découpe la grille en deux, puis on reprend chaque moitié et on les recoupe encore en deux.
On continue comme ça plusieurs fois, jusqu'à obtenir plusieurs petits rectangles. Dans chaque rectangle final, on place une salle. Et comme toute ces zones proviennent d'une découpe logique, il suffit ensuite de relier les salles en suivant ces découpes pour créer des couloirs cohérent.

Comme chaque découpe en crée deux nouvelles, le BSP forme naturellement un arbre logique qui organise toutes les zones entre elles.

![BinarySpacePartitioningImage](https://www.tutorialspoint.com/computer_graphics/images/what_are_bsp_trees.jpg)
