# Ennemi Behaviour 

This class implements a stochastic behaviour. Basically, it works as follows: 

* Define some actions and their probability. These are StochasticBehaviour 
* BasicEnnemiSubBehaviour has an array of those actions. When instantiated, it normalized the probabilities. When the `Sample` method is called, it returns a string corresponding to the name of the chosen action, according to probability distribution 
* BasicEnnemiSubBehaviour also has min and max distances, that are use by the higher hierarchy to decide which SubBehaviour to use. 
* This is implemented in BasicEnnemiBehaviour. When the `SelectAction` method is called, it does what is described above, according to a float corresponding to the distance
* Finally, all of this is held by the BasicEnnemi class which relies on Perso for movement and animation and on a Navigation class and this decision module



## TODO: 

* Create a custom editor for these behaviors. Would be much easier to use 
