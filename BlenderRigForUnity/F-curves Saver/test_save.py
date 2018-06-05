import bpy
from Saver import * 
from BasicHuma import Humanoid


print('\n'*100)
path = '/home/mehdi/Blender/Scripts/FcurvesSL/Hook'

armature = Humanoid(bpy.data.objects['Armature']) 
curves = armature.get_curves()

saver = StructureSaver('newtest', curves, armature)

saver.save(path)