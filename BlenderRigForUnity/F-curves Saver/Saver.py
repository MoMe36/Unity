import bpy 
import random 
import math 
import mathutils as m 
import pickle 

# def set_frame(nb): 
# 	bpy.context.scene.frame_current = nb


curve_dico = {0: 'LocX',
			  1: 'LocY',
			  2: 'LocZ', 
			  3: 'RotW',
			  4: 'RotX',
			  5: 'RotY',
			  6: 'RotZ'
			  }


class Point(): 

	def __init__(self, point, ratio): 

		self.point = [c/ratio for c in point.co]
		self.left_handle = [c/ratio for c in point.handle_left]
		self.right_handle = [c/ratio for c in point.handle_right]

	def __repr__(self):

		s = 'Points {}\nLeft handle {}\nRight handle {}\n\n '.format(self.point, self.left_handle, self.right_handle)
		return s


class CurveStructure():

	def __init__(self,c, action_name, bone_nb, curve_number, ratio = 1.): 

		self.name = 'Action {} - Bone {} - Curve {} '.format(action_name, bone_nb, curve_dico[curve_number])
		self.frames = [int(k.co.x) for k in c.keyframe_points]
		self.points = []

		self.name += '- Keyframes {} \n\n'.format(self.frames)

		for k in c.keyframe_points:
			self.points.append(Point(k, ratio))

	def __repr__(self):

		return self.name


class StructureSaver(): 

	def __init__(self,name, curves, armature): 

		self.name = name 
		self.nb_bones = int(len(curves)/7)

		self.bones = armature.bones_names

		leg_length = armature.leg_length
		arm_length = armature.arm_length

		self.curves = []
		for i,c in enumerate(curves): 
			try: 
				current_name = self.bones[int(i/7)]
			except IndexError: 
				print('Exiting before registering other bones')
				break 

			ratio = 1.
			if i%7 < 3:
				if current_name in armature.leg_sensitive:
					print('Current bone {} is leg sensitive. Apply ratio of {:.3f} for {}'.format(current_name, leg_length, curve_dico[i%7]))
					ratio = leg_length
				if current_name in armature.arm_sensitive:
					print('Current bone {} is leg sensitive. Apply ratio of {:.3f} for {}'.format(current_name, arm_length, curve_dico[i%7]))
					ratio = arm_length
			self.curves.append(CurveStructure(c, name, int(i/7), i%7, ratio))

		# for c in self.curves: 
		# 	print(c)

		self.infos =[leg_length, arm_length]

	def save(self,path): 

		pickle.dump(self.curves, open(path, 'wb'))
		print('Data saved')