import bpy 
import mathutils as m 
import math 
import pickle 


class Humanoid(): 

	def __init__(self, armature, load = False, path = ''): 

		self.path = path

		self.armature = armature
		self.footL = self.armature.pose.bones['Foot Control L']
		self.footR = self.armature.pose.bones['Foot Control R']
		self.handL = self.armature.pose.bones['IK Arm L']
		self.handR = self.armature.pose.bones['IK Arm R']
		self.head = self.armature.pose.bones['Head']
		self.pelvis = self.armature.pose.bones['Pelvis']
		self.spine1 = self.armature.pose.bones['Spine1']
		self.spine2 = self.armature.pose.bones['Spine2']

		self.legL = self.armature.pose.bones['Leg L']
		self.legR = self.armature.pose.bones['Leg R']

		self.shouL = self.armature.pose.bones['Upperarm L']
		self.shouR = self.armature.pose.bones['Upperarm R']

		self.targetArmL = self.armature.pose.bones['IKT Arm L']
		self.targetArmR = self.armature.pose.bones['IKT Arm R']

		self.targetLegL = self.armature.pose.bones['IKT Leg L']
		self.targetLegR = self.armature.pose.bones['IKT Leg R']

		self.bones = {
		'footL':self.footL,
		'footR':self.footR,
		'armL':self.handL, 
		'armR':self.handR, 
		'pelvis':self.pelvis, 
		'spine1':self.spine1, 
		'spine2':self.spine2, 
		'head':self.head, 
		'targetLL':self.targetLegL, 
		'targetLR':self.targetLegR,
		'targetAL':self.targetArmL, 
		'targetAR':self.targetArmR
		}


		self.bones_names = [k.name for k in self.armature.data.bones]

	def select(self, bone): 

		bpy.ops.pose.select_all(action = 'DESELECT')
		self.armature.data.bones.active = self.armature.data.bones[bone.name]
		self.armature.data.bones[bone.name].select = True

	def select_index(self, nb): 

		# Récuperer le nom de l'os. S'en servir pour le sélectionner. Je crois que c'est ce qui fait sauter le fait de plaquer les courbes.
		name = self.bones_names[nb]

		print('Selecting {}'.format(name))
		bone = self.armature.pose.bones[name]
		self.select(bone)

		return name

	def get_curves(self): 

		return self.armature.animation_data.action.fcurves

	def create_action(self, name): 

		self.armature.animation_data_create()
		self.armature.animation_data.action = bpy.data.actions.new(name = name)
