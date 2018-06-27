# Controller based on Ghost of Tsushima preview

Basic controller for fighting games. WIP


1. Add SamuraiController to a character 
1. It uses SamuraiFiller for holding all the data 
1. A very important class is AnimationStates: holds names and states of triggers. Use `Activate("name_of_the_state")` to launch an animation. 
1. Another one is CharacterStates, which carries states defined by names, and animation correspondances. Beware of spelling mistakes when defining correspondances. Also, precise passive states (in contrast with ponctual actions) and fighting states. 
1. Make sure to add the FightingBehaviourScript for all fighting states, responsible to turning ExitBool to true, for espacing fighting animations. 
1. You can add Effects, Impulsions (useful for jumps)
