Classe : CellularAutomata
Description : Implémente une méthode de génération procédurale basée sur un automate cellulaire (au même titre que le jeu de la vie de Connway). Elle remplie une grille avec deux type de cellule : terre et eau.
Règle / algorithme : on regarde les 8 cellules voisine d'une cellule, si X cellules (ou plus) sont de type 'TERRE', alors la cellule observé deviendra également de type 'TERRE'.  

Fonctionnement :

1. Initialisation

   Initialise deux buffers (currentBuffer et nextBuffer) de la même taille que la grille.

2. Bruit initial

   La méthode CreateNoise va populate currentBuffer de cellules. Elle utilise GroundRatio pour déterminer la quantité de terre par rapport à la quantité d'eau.

3. Affinage par itération

   - Pour chaque cellule on calcule `nextBuffer[x, y] = SurroundedByGround(x, y)`.
   - SurroundedByGround regarde les 8 cellules voisine de type 'TERRE' de la cellule observé. La méthode retourne `true` si ce nombre est supérieur ou égal à GroundSisterNeeded (le nombre de cellule 'terre' nécessaire pour convertir une cellule).
   - Si une cellule change d'état entre currentBuffer et nextBuffer, sont skin est changé pour correspondre au nouvel état.
   - Pour finir, on swap currentBuffer et nextBuffer, on attend GridGenerator.StepDelay, et on répète l'opération _maxSteps fois.

4. Résultat

   Au fur et à mesure de l'affinage, le bruit s'uniformise pour former des masses de terre cohérente entouré d'eau.