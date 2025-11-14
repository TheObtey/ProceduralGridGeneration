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
