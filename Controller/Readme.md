# TPS controller classes and example script 

The controller relies on one main class: `Perso`. This class manages the state machine, the physic and the animations. 

It has a method called `MAJ` that has to be called every frame to update the state machine, ground detection and the animations.
Other methods that are susceptible to be called from outside are: 

* Activate(string name): A method to interact with animator controller and use triggers and so on.
* Move, Jump, Dash


## Other valuable informations

The class Perso uses one instance of Impulsion to generate accelerations, jumps and so on. The impulsion class
holds informations concerning the time during which the force should be applied, the direction and the magnitude. 

To ease the process of filling the informations relevant to the Perso class (animation states, character states, speed, jump force...) 
I implemented a class called PersoFiller which holds these informations and is then used when Perso is instanciated. 

## TO DO 

Provide a better state machine. 
