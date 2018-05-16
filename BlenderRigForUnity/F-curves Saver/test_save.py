import bpy
from Saver import * 


print('\n'*100)
path = '/home/mehdi/Blender/Scripts/FcurvesSL/moving'

armature = bpy.data.objects['Armature'] 
curves = armature.animation_data.action.fcurves

saver = StructureSaver('newtest', curves)

saver.save(path)