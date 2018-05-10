import bpy 
import mathutils as m
import math 
import pickle

def confirm_animation_pose(): 

	bpy.ops.pose.select_all(action = 'SELECT')
	bpy.ops.anim.keyframe_insert_menu(type = '__ACTIVE__', confirm_success = True)
	reset_all()

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

def reset_all(): 

	bpy.ops.pose.select_all(action = 'SELECT')
	bpy.ops.pose.rot_clear()
	bpy.ops.pose.loc_clear()

class Skeleton(): 

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

		self.set_data(load)


	def set_data(self, load): 

		if load: 
			self.initial_pose = {}
			pickled = pickle.load(open(self.path+'initial_pose', 'rb'))
			for b in pickled: 
				self.initial_pose[b] = m.Matrix(pickled[b])

			params = pickle.load(open(self.path+'parameters', 'rb'))
			self.arm_length = params[0]
			self.leg_length = params[1]

		else:
			reset_all()

			to_be_pickled = {}
			self.initial_pose = {}

			self.arm_length = math.fabs(self.handR.head.x - self.shouR.head.x)
			self.leg_length = math.fabs(self.legL.head.z - self.footL.head.z)

			for b in self.bones:

				pose = self.bones[b].matrix
				to_be_pickled[b] = matrix_to_list(pose)
				self.initial_pose[b] = pose


			pickle.dump(to_be_pickled, open(self.path+'initial_pose', 'wb'))
			params = [self.arm_length, self.leg_length]
			pickle.dump(params, open(self.path+'parameters', 'wb'))

	def move(self, bone_name, vec): 

		bone = self.bones[bone_name]
		self.set_active(bone)

		factor = 1.
		if bone_name in ['footL', 'footR', 'targetLL', 'targetLR', 'pelvis']: 
			factor = self.leg_length

		if bone_name in ['armL', 'armR', 'targetAR', 'targetAL']: 
			factor = self.arm_length

		vec = vec.copy()*factor

		bpy.ops.transform.translate(value = vec)

	def rotate(self, bone_name, quat): 

		bone = self.bones[bone_name]
		self.set_active(bone)
		bpy.ops.transform.rotate(value = quat.angle, axis = quat.axis)

	def set_active(self, bone): 

		bpy.ops.pose.select_all(action = 'DESELECT')
		self.armature.data.bones.active = self.armature.data.bones[bone.name]
		self.armature.data.bones[bone.name].select = True



	def print_pose(self): 

		null_rot = m.Quaternion((1.,0.,0.,0.))
		null_trans = m.Vector((0.,0.,0.))

		for b in self.bones: 

			current_rot = self.bones[b].matrix.to_quaternion().copy()
			initial_rot = self.initial_pose[b].to_quaternion().copy()

			current_pos = self.bones[b].matrix.translation.copy()
			initial_pos = self.initial_pose[b].translation.copy()

			if b in ['pelvis', 'spine1', 'spine2', 'head']: 

				if b == 'pelvis': 
					self.explicit_rot(b, current_rot, initial_rot)
					self.explicit_trans(b, current_pos, initial_pos, self.leg_length)
				elif b =='spine1': 
					initial_rot = self.bones['pelvis'].matrix.to_quaternion().copy()
					self.explicit_rot(b, current_rot, initial_rot)
				elif b == 'spine2': 
					initial_rot = self.bones['spine1'].matrix.to_quaternion().copy()
					self.explicit_rot(b, current_rot, initial_rot)
				else: 
					initial_rot = self.bones['spine2'].matrix.to_quaternion().copy()
					self.explicit_rot(b, current_rot, initial_rot)

			else: 

				if b in ['footL', 'footR', 'targetLL', 'targetLR']: 
					self.explicit_rot(b, current_rot, initial_rot)
					self.explicit_trans(b, current_pos, initial_pos, self.leg_length)					
				if b in ['armL', 'armR', 'targetAR', 'targetAL']: 
					self.explicit_rot(b, current_rot, initial_rot)
					self.explicit_trans(b, current_pos, initial_pos, self.arm_length)




	def substract_trans(self, final, initial): 
		translation = final - initial
		return translation


	def explicit_trans(self, b, final, initial, factor = 1.): 
		t = self.substract_trans(final, initial)
		t /= factor

		null_trans = m.Vector((0.,0.,0.))
		if t != null_trans: 
			print('self.move(\'{}\', m.Vector(({:.4f},{:.4f},{:.4f})))'.format(b,t.x,t.y,t.z))

	def substract_rot(self, final, initial): 

		q_tot = final*initial.inverted()
		return q_tot
	
	def explicit_rot(self, b, final, initial): 

		q = self.substract_rot(final, initial)
		null_rot = m.Quaternion((1.,0.,0.,0.))
		if q != null_rot: 
			print('self.rotate(\'{}\', m.Quaternion(({:.4f},{:.4f},{:.4f},{:.4f})))'.format(b, q.w,q.x,q.y,q.z))

	def test(self): 	

		self.armature.animation_data_create()
		self.armature.animation_data.action = bpy.data.actions.new(name = 'Run')
		reset_all()
		set_frame(1)

		self.move('footL', m.Vector((0.0000,-0.2786,0.4208)))
		self.move('footR', m.Vector((0.0000,0.2193,0.0000)))
		self.move('armL', m.Vector((-0.8331,0.0000,-0.8861)))
		self.move('armR', m.Vector((0.8090,0.0000,-0.9342)))
		self.rotate('pelvis', m.Quaternion((0.9610,-0.2493,-0.0302,-0.1163)))
		self.move('pelvis', m.Vector((0.0000,-0.0296,-0.1126)))
		self.rotate('spine1', m.Quaternion((0.9541,0.2909,-0.0625,0.0347)))
		self.rotate('spine2', m.Quaternion((0.9782,0.2017,-0.0433,0.0241)))
		self.rotate('head', m.Quaternion((1.0000,0.0000,-0.0000,0.0000)))

		confirm_animation_pose()
		add_frames(10)

		self.rotate('footL', m.Quaternion((0.9386,-0.3451,-0.0000,0.0000)))
		self.move('footL', m.Vector((0.0000,-0.5173,0.1142)))
		self.rotate('footR', m.Quaternion((-0.7016,-0.7126,-0.0000,0.0000)))
		self.move('footR', m.Vector((0.0000,0.5630,0.3252)))
		self.move('armL', m.Vector((-0.8331,0.5658,-0.6814)))
		self.move('armR', m.Vector((0.8090,-0.6300,-0.8901)))
		self.rotate('pelvis', m.Quaternion((0.9610,-0.2493,-0.0302,-0.1163)))
		self.move('pelvis', m.Vector((0.0000,-0.0296,-0.1126)))
		self.rotate('spine1', m.Quaternion((0.9541,0.2909,-0.0625,0.0347)))
		self.rotate('spine2', m.Quaternion((0.9782,0.2017,-0.0433,0.0241)))
		self.rotate('head', m.Quaternion((1.0000,0.0000,-0.0000,0.0000)))

		confirm_animation_pose()
		add_frames(10)

		self.move('footL', m.Vector((0.0000,0.2193,0.0000)))
		self.move('footR', m.Vector((0.0000,-0.2786,0.4208)))
		self.move('armL', m.Vector((-0.8090,0.0000,-0.9342)))
		self.move('armR', m.Vector((0.8331,0.0000,-0.8861)))
		self.rotate('pelvis', m.Quaternion((0.9610,-0.2493,0.0302,0.1163)))
		self.move('pelvis', m.Vector((0.0000,-0.0296,-0.1126)))
		self.rotate('spine1', m.Quaternion((0.9541,0.2909,0.0625,-0.0347)))
		self.rotate('spine2', m.Quaternion((0.9782,0.2017,0.0433,-0.0241)))
		self.rotate('head', m.Quaternion((1.0000,0.0000,0.0000,-0.0000)))

		confirm_animation_pose()





print('\n'*30)


path = '/home/mehdi/Blender/Scripts/'
skeleton = Skeleton(bpy.data.objects['Armature'], load = True,  path= path)

# skeleton.test()
skeleton.print_pose()
# skeleton.get_rot()