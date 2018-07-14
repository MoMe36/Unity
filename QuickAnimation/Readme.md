# Quick animation scripts 

1. Use UnityArmature.py to generate an armature consistent with futher steps
1. Use FcurvesOperator.py to save or load animations. 
1. If you add new bones to the structure: make sure their name starts with `other_` and be aware that they will not be recorded in the animation


# New version ! 
Several improvements have been made: 
* No need to insert keyframes for all the bones 
* No restriction on names. Whichever name you used to record the animation will be used as reference for loading it.
* Has a small tentative to conserve proportions via a ratio 


Example at : https://youtu.be/b1CiLcGc3Rw


# Newer version ! 
Most important improvement: Rigify support ! There is now no limit as to the rig you use. Tested on BasicHuman, BasicQuadruped. Some things that could be improved: 

* No more support for the ratio. Given that the structure has changed, the scripts now finds bones by name, and given the number of bones in rigify, I did not go over them all. One idea to be tested: save informations about the rig along with animation in order to be used to scale animation when loaded 

* Also, it could be interesting to propose the user to remap the names. 

File is: `QuickTransfer.py`. Enjoy ! 
