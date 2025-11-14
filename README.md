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
