import bpy
import random 
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

        leg_pos = self.armature.data.bones['Leg L'].head.z
        foot_pos = self.armature.data.bones['Foot Control L'].head.z
        hand_pos = self.armature.data.bones['Upperarm L'].head.x 
        shoulder_pos = self.armature.data.bones['IK Arm L'].head.x

        # self.bones_names = [k.name for k in self.armature.data.bones] # PREVIOUS VERSION 
        self.bones_names = [k.name for k in self.armature.data.bones if "other" not in k.name]

        self.arm_length = math.fabs(hand_pos - shoulder_pos)
        self.leg_length = math.fabs(leg_pos - foot_pos)

        self.initial_pelvis_pos = self.pelvis.head.x

        self.leg_sensitive = ['Foot Control L', 'Foot Control R','IKT Leg L','IKT Leg R', 'Pelvis']
        self.arm_sensitive = ['IK Arm L','IK Arm R','IKT Arm L','IKT Arm R' ]

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


def get_point(point, ratio):

	current_point = [c/ratio for c in point.co]
	left_handle = [c/ratio for c in point.handle_left]
	right_handle = [c/ratio for c in point.handle_right]

	return {'p':current_point, 'lh':left_handle, 'rh':right_handle}

def get_curve(c, bone_nb, curve_number, ratio = 1.):

	frames = [int(k.co.x) for k in c.keyframe_points]
	all_points = []

	for k in c.keyframe_points: 
		all_points.append(get_point(k, ratio))


	return {'points':all_points, 'frames':frames}

def save_all(curves, armature, path): 

	nb_bones = int(len(curves)/7)
	bones = armature.bones_names

	leg_length = armature.leg_length
	arm_length = armature.arm_length

	all_curves = []
	for i,c in enumerate(curves): 
		try: 
			current_name = bones[int(i/7)]
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
		all_curves.append(get_curve(c, int(i/7), i%7, ratio))

	# print(all_curves)
	pickle.dump(all_curves, open(path, 'wb'))
	print('Data saved')

def load_all(path_to_data, armature, name): 

	curves = pickle.load(open(path_to_data, 'rb'))

	current_leg_length = armature.leg_length
	current_arm_length = armature.arm_length

	all_bones = armature.bones_names

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
		keyframes = curves[index]['frames']
		name = armature.select_index(i)   # select bone for adding frames for the 7 curves in the same time 

		for k in keyframes: 
			set_frame(k)
			bpy.ops.anim.keyframe_insert_menu(type = '__ACTIVE__')

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

			ref = c_t['points'][idx]
			kf.co.y = ref['p'][1]*ratio
			kf.handle_left = m.Vector(ref['lh'])
			kf.handle_right = m.Vector(ref['rh'])

			kf.handle_left.y *= ratio
			kf.handle_right.y *= ratio
			kf.handle_left.x *= ratio
			kf.handle_right.x *= ratio


		courbe += 1 

class DialogOperator(bpy.types.Operator):
    bl_idname = "object.dialog_operator"
    bl_label = "Save/Load animation"

    saving = bpy.props.BoolProperty(name="Save ? Else load.")
    path_to_anim = bpy.props.StringProperty(name="Path to folder")
    anim_name = bpy.props.StringProperty(name="Animation name:")
    # path_to_anim += "/home/mehdi/Blender/Scripts/"

    def execute(self, context):
        # print('This is execute with: Saving: {}  Name:{}'.format(self.saving, self.path_to_anim))
        
        if self.saving: 
            self.launch_save()
            message = 'Animation {} saved at {}'.format(self.anim_name, self.path_to_anim)
        else: 
            self.launch_load()
            message = 'Animation {} loaded'.format(self.anim_name)

        self.report({'INFO'}, message)

        return {'FINISHED'}

    def invoke(self, context, event):
        wm = context.window_manager
        return wm.invoke_props_dialog(self)

    def launch_load(self): 
        full_path = self.path_to_anim + self.anim_name
        target_armature = Humanoid(bpy.data.objects['Armature'])
        load_all(full_path, target_armature, 'LastLoaded')

    def launch_save(self): 

        full_path = self.path_to_anim + self.anim_name
        source_armature = Humanoid(bpy.data.objects['Armature'])
        curves = source_armature.get_curves()
        save_all(curves, source_armature,full_path)


bpy.utils.register_class(DialogOperator)

# test call
bpy.ops.object.dialog_operator('INVOKE_DEFAULT', path_to_anim = '/home/mehdi/Bureau/', anim_name = 'Test')