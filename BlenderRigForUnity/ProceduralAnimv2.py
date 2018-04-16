import bpy 
import mathutils as m
import math 
import pickle


def matrix_to_euler(mat): 

	quat = mat.to_quaternion()
	eul = quat.to_euler()
	eul = [math.degrees(e) for e in eul]
	return eul

def matrix_to_list(mat): 
	
	to_vec = list(mat)
	to_tuple = [v.to_tuple() for v in to_vec]
	return to_tuple

def list_to_matrix(liste): 

	return m.Matrix(liste)

def add_frames(nb_frames): 

	current = bpy.context.scene.frame_current
	bpy.context.scene.frame_current = current + nb_frames

def set_frame(nb_frames): 

	bpy.context.scene.frame_current = nb_frames



class Skeleton(): 

	def __init__(self, armature, load = False): 

		self.armature = armature
		self.footL = self.armature.data.bones['Foot Control L']
		self.footR = self.armature.data.bones['Foot Control R']
		self.handL = self.armature.data.bones['IK Arm L']
		self.handR = self.armature.data.bones['IK Arm R']
		self.head = self.armature.data.bones['Head']
		self.pelvis = self.armature.data.bones['Pelvis']
		self.spine1 = self.armature.data.bones['Spine1']
		self.spine2 = self.armature.data.bones['Spine2']

		self.legL = self.armature.data.bones['Leg L']
		self.legR = self.armature.data.bones['Leg R']

		self.shouL = self.armature.data.bones['Upperarm L']
		self.shouR = self.armature.data.bones['Upperarm R']

		self.targetArmL = self.armature.data.bones['IKT Arm L']
		self.targetArmR = self.armature.data.bones['IKT Arm R']

		self.targetLegL = self.armature.data.bones['IKT Leg L']
		self.targetLegR = self.armature.data.bones['IKT Leg R']

		self.dico = {
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

		self.leg_length = self.legL.head_local.z - self.footL.head_local.z
		self.arm_length = self.handR.head_local.x - self.shouR.head_local.x

		self.get_inital_pose(load) # Getting bone.head_local gets the bone position in edit mode !! 

	def get_inital_pose(self, load): 


		path = '/home/mehdi/Code/PythonBlender/Animation/initial_pose'

		self.initial_pose = {}

		if load: 

			pickled_pose = pickle.load(open('path', 'rb'))
			for entry in self.dico: 
				self.initial_pose[entry] = m.Matrix(pickled_pose[entry])
		else: 
			# In order to get the pose (matrix), make sure to have the bone as ACTIVE (selected isn't enough)
			self.reset_all()
			to_be_pickled = {}
			for bone in self.dico: 

				bpy.ops.pose.select_all(action = 'DESELECT')
				self.dico[bone].select = True
				self.armature.data.bones.active = self.dico[bone]
				pose = bpy.context.active_pose_bone.matrix
				pose_pickable = matrix_to_list(pose)
				self.initial_pose[bone] = pose
				to_be_pickled[bone] = pose_pickable


			pickle.dump(to_be_pickled, open('path', 'wb'))

	
		
	def reset_all(self):

		bpy.ops.pose.select_all(action = 'SELECT')
		bpy.ops.pose.loc_clear()
		bpy.ops.pose.rot_clear() 

	def confirm_animation_pose(self): 

		bpy.ops.pose.select_all(action = 'SELECT')
		bpy.ops.anim.keyframe_insert_menu(type = '__ACTIVE__', confirm_success = True)
		self.reset_all()

	def get_bone_rotation(self, entry): 

		self.set_active(entry)
		return bpy.context.active_pose_bone.matrix.to_quaternion()

	def set_active(self, entry): 

		bpy.ops.pose.select_all(action = 'DESELECT')
		self.armature.data.bones.active = self.dico[entry]

	def move(self, name, vector): 

		if name in ['footL', 'footR']: 
			vector *= self.leg_length
		if name in ['armL', 'armR', 'targetLL', 'targetLR', 'targetAL', 'targetAR']:
			vector *= self.arm_length
		if name == 'pelvis': 
			vector *= self.leg_length

		self.set_active(name)
		bpy.ops.transform.translate(value = vector)

	def rotate(self, name, quaternion): 

		self.set_active(name)
		bpy.ops.transform.rotate(value = quaternion.angle, axis = quaternion.axis)

	def test(self): 

		self.armature.animation_data_create()
		self.armature.animation_data.action = bpy.data.actions.new(name = 'Copy')
		self.reset_all()
		set_frame(1)

		
		self.move('footL', m.Vector((0.5748746991157532,0.0,0.0)))
		self.move('armL', m.Vector((-0.048654794692993164,-2.1592438770312583e-07,1.6542617082595825)))
		self.move('pelvis', m.Vector((0.2328774631023407,-9.16994569166718e-09,-0.24119451642036438)))
		self.rotate('pelvis', m.Quaternion((1.0000097751617432,0.0,0.0,0.0)))
		self.rotate('spine1', m.Quaternion((0.9543267488479614,0.0,0.2987648844718933,2.9802322387695312e-08)))
		self.rotate('spine2', m.Quaternion((0.9706920981407166,-4.470348358154297e-08,0.2403268963098526,5.960464477539063e-08)))

		self.confirm_animation_pose()
		add_frames(20)


		self.move('footL', m.Vector((0.5748746991157532,0.0,0.0)))
		self.move('armL', m.Vector((0.2757103443145752,1.1237013339996338,0.7779748439788818)))
		self.move('armR', m.Vector((-0.793205976486206,1.4829161167144775,0.9551668167114258)))
		self.move('pelvis', m.Vector((0.2328774631023407,-9.16994569166718e-09,-0.24119451642036438)))
		self.rotate('pelvis', m.Quaternion((1.0000097751617432,0.0,0.0,0.0)))
		self.rotate('spine1', m.Quaternion((0.8588167428970337,0.5122829675674438,0.0,0.0)))
		self.rotate('spine2', m.Quaternion((-0.8897241353988647,-0.45649850368499756,0.0,0.0)))
		self.rotate('head', m.Quaternion((0.9612898230552673,0.2755393981933594,0.0,0.0)))
		self.move('targetAL', m.Vector((1.7881393432617188e-07,-0.7136030197143555,1.7840075492858887)))
		self.move('targetAR', m.Vector((-0.9041014909744263,-0.0,2.3893985748291016)))

		self.confirm_animation_pose()
		add_frames(20)

		self.move('footR', m.Vector((-0.5748746991157532,0.0,0.0)))
		self.move('armR', m.Vector((0.048654794692993164,-2.1592438770312583e-07,1.6542617082595825)))
		self.move('pelvis', m.Vector((-0.2328774631023407,-9.16994569166718e-09,-0.24119451642036438)))
		self.rotate('pelvis', m.Quaternion((1.0000097751617432,0.0,0.0,0.0)))
		self.rotate('spine1', m.Quaternion((0.9543267488479614,0.0,-0.2987648844718933,-2.9802322387695312e-08)))
		self.rotate('spine2', m.Quaternion((0.9706920981407166,-2.2351741790771484e-08,-0.24032697081565857,-7.450580596923828e-08)))

		self.confirm_animation_pose()

	def print_pose(self): 

		cumulated_rotation = m.Quaternion([0.7071,0.7071,0.,0.])
		for entry in self.dico: 
			initial = self.initial_pose[entry]

			self.armature.data.bones.active = self.dico[entry]
		

			current = bpy.context.active_pose_bone.matrix

			if entry in ['footL', 'footR']: 
				new_position = current.to_translation() 
				old_position = initial.to_translation()
				translation = new_position - old_position
				if translation.length > 0.05: 
					translation /= self.leg_length
					print('self.move(\'{}\', m.Vector(({},{},{})))'.format(entry, translation.x,translation.y,translation.z))
				
				new_rotation = current.to_quaternion()
				old_rotation = initial.to_quaternion()
				
				if new_rotation != old_rotation: 
					old_rotation.invert()
					rotation = new_rotation*old_rotation

					print('self.rotate(\'{}\', m.Quaternion(({},{},{},{})))'.format(entry, rotation.w,rotation.x,rotation.y,rotation.z))
				

			if entry in ['armL', 'armR', 'targetLL', 'targetLR', 'targetAL', 'targetAR']: 

				new_position = current.to_translation() 
				old_position = initial.to_translation()
				translation = new_position - old_position

				if translation.length > 0.05: 
					translation /= self.arm_length
					print('self.move(\'{}\', m.Vector(({},{},{})))'.format(entry, translation.x,translation.y,translation.z))


			if entry in ['head', 'spine1', 'spine2', 'pelvis']:


				if entry == 'pelvis':	
					new_position = current.to_translation() 
					old_position = initial.to_translation()
					translation = new_position - old_position

					if translation.length > 0.05: 
						translation /= self.leg_length
						print('self.move(\'{}\', m.Vector(({},{},{})))'.format(entry, translation.x,translation.y,translation.z))

					old_rotation = m.Quaternion([0.7071,0.7071,0.,0.])

					new_rotation = self.get_bone_rotation('pelvis')

					if new_rotation != old_rotation: 
						old_rotation.invert()
						rotation = old_rotation*new_rotation

						print('self.rotate(\'{}\', m.Quaternion(({},{},{},{})))'.format(entry, rotation.w,rotation.x,rotation.y,rotation.z))

				
				else:
					if entry == 'spine1': 
						old_rotation = self.get_bone_rotation('pelvis')
						new_rotation = self.get_bone_rotation('spine1')
					elif entry == 'spine2': 
						old_rotation = self.get_bone_rotation('spine1')
						new_rotation = self.get_bone_rotation('spine2')
					elif entry == 'head': 
						old_rotation = self.get_bone_rotation('spine2')
						new_rotation = self.get_bone_rotation('head')

					if new_rotation != old_rotation: 

						c_o_r = old_rotation.copy()
						c_n_r = new_rotation.copy()
						c_o_r.invert()

						rotation = c_n_r*c_o_r
						print('self.rotate(\'{}\', m.Quaternion(({},{},{},{})))'.format(entry, rotation.w,rotation.x,rotation.y,rotation.z))




print('\n'*20)



sketelon = Skeleton(bpy.data.objects['Armature'], load = True)
sketelon.test()
# sketelon.reset_all()

# sketelon.print_pose()
# sketelon.jump()
# sketelon.run()
# sketelon.idle()

# sketelon.hit()