import bpy 
import Loader 
import pickle 
import mathutils as m 
import math 
from BasicHuma import Humanoid

# class Skeleton(): 

# 	def __init__(self, armature): 

# 		self.arm = armature

# 		self.b0 = armature.pose.bones['b0']
# 		self.b1 = armature.pose.bones['b1']
# 		self.b2 = armature.pose.bones['b2']

# 		self.bones = [self.b0, self.b1, self.b2]

# 	def select(self, nb ): 

# 		bpy.ops.pose.select_all(action = 'DESELECT')
# 		self.arm.data.bones[self.bones[nb].name].select = True

# 	def create_action(self, name): 

# 		self.arm.animation_data_create()
# 		self.arm.animation_data.action = bpy.data.actions.new(name = name)

# 	def get_curves(self):

# 		anim_d = self.arm.animation_data
# 		return anim_d.action.fcurves

print('\n'*100)
path = '/home/mehdi/Blender/Scripts/FcurvesSL/Hook'


a = Humanoid(bpy.data.objects['Armature'])
Loader.Load(path ,a, 'MyTest')