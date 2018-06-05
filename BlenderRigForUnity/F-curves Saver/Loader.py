import bpy 
import random 
import math 
import mathutils as m 
import pickle 

curve_dico = {0: 'LocX',
			  1: 'LocY',
			  2: 'LocZ', 
			  3: 'RotW',
			  4: 'RotX',
			  5: 'RotY',
			  6: 'RotZ'
			  }

def set_frame(nb): 
	bpy.context.scene.frame_current = nb


def Load(path_to_data, armature, name): 

	curves = pickle.load(open(path_to_data, 'rb'))
	# curves = data[0]
	# infos = data[1]

	current_leg_length = armature.leg_length
	current_arm_length = armature.arm_length

	all_bones = armature.bones_names

	# ratio_arm = infos[1]/current_arm_length
	# ratio_leg = infos[0]/current_leg_length

	# print('Ratio Arms {} --  Legs {}'.format(ratio_arm, ratio_leg))

	print('Curves: {} -- Bones {}'.format(len(curves), len(curves)/7))
	
	armature.create_action(name)
	source_curves = armature.get_curves()
	
	# reset all 
	bpy.ops.pose.select_all(action = 'SELECT')
	bpy.ops.pose.loc_clear()
	bpy.ops.pose.rot_clear()

	set_frame(0)

	# Add one frame for curves 

	bpy.ops.anim.keyframe_insert_menu(type = '__ACTIVE__', confirm_success = True)

	# add frames for each curve 

	for i in range(len(armature.bones_names)): 
		index = i*7 # make sure to iterate well (7 curves per bone)
		keyframes = curves[index].frames
		name = armature.select_index(i)   # select bone for adding frames for the 7 curves in the same time 

		for k in keyframes: 
			set_frame(k)
			bpy.ops.anim.keyframe_insert_menu(type = '__ACTIVE__')
			# print('Bone {} - Adding keyframe {} '.format(name, k))

	# Flat curves are set up. 
	# Now, time to move the control points and handles 

	courbe = 0 
	for c_s, c_t in zip(source_curves, curves): 
		ratio = 1.
		current_bone = all_bones[int(courbe/7)]
		if courbe%7 <3: 
			if current_bone in armature.leg_sensitive: 
				ratio = current_leg_length
			elif current_bone in armature.arm_sensitive: 
				ratio = current_arm_length


		for idx,kf in enumerate(c_s.keyframe_points):
			# print('Courbe {} -- Point {} '.format(courbe, idx))
			ref = c_t.points[idx]
			kf.co.y = ref.point[1]*ratio
			kf.handle_left = m.Vector(ref.left_handle)
			kf.handle_right = m.Vector(ref.right_handle)

			kf.handle_left.y *= ratio
			kf.handle_right.y *= ratio
			kf.handle_left.x *= ratio
			kf.handle_right.x *= ratio


		courbe += 1 

