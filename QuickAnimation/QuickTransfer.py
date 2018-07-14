import bpy 
import math 
import mathutils as m 
import pickle 


quat_dict = {0:'LocX', 1:'LocY', 2:'LocZ', 3:'QW', 4:'QX', 5:'QY', 6:'QZ' }
xyz_dict = {0:'LocX', 1:'LocY', 2:'LocZ', 3:'EX', 4:'EY', 5:'EZ'}

class Rig: 

	def __init__(self, rig):

		self.rig = rig

	def create_action(self, name): 

		self.rig.animation_data_create()
		self.rig.animation_data.action = bpy.data.actions.new(name = name)

	@property 
	def all_bones(self):
		l = [b.name for b in self.rig.pose.bones]
		return l 
	@property
	def curves(self):
		return self.rig.animation_data.action.fcurves

	@property
	def pose(self): 
		return self.rig.pose

	def select_from_name(self, bone_name): 

		bpy.ops.pose.select_all(action = 'DESELECT')
		self.rig.data.bones.active = self.rig.data.bones[bone_name]
		self.rig.data.bones[bone_name].select = True

        # print('Selecting: {}'.format(bone_name))


def set_frame(nb): 
    bpy.context.scene.frame_current = nb


def get_point(point, ratio):

    current_point = [c/ratio for c in point.co]
    left_handle = [c/ratio for c in point.handle_left]
    right_handle = [c/ratio for c in point.handle_right]

    return {'p':current_point, 'lh':left_handle, 'rh':right_handle}

def get_curve(c, ratio = 1.):

    frames = [int(k.co.x) for k in c.keyframe_points]
    all_points = []

    for k in c.keyframe_points: 
        all_points.append(get_point(k, ratio))


    return {'points':all_points, 'frames':frames}


def get_infos(curve, rig): 

	name = curve.group.name
	mode = rig.pose.bones[name].rotation_mode
	dico = quat_dict if mode == 'QUATERNION' else xyz_dict

	return name, mode, dico

def save_dict(rig, path): 

	curves = rig.curves 

	data = {}

	current_name, current_mode, current_dico = get_infos(curves[0], rig)

	data[current_name] = {}
	curve_counter = 0

	for i,c in enumerate(curves): 

		if c.group.name != current_name: 
			current_name, current_mode, current_dico = get_infos(c, rig)
			curve_counter = 0 
			data[current_name] = {}

		print('Bone {} in mode {} with curve counter: {}'.format(current_name, current_mode, curve_counter))

		data[current_name][current_dico[curve_counter]] = get_curve(c)

		curve_counter += 1 

	pickle.dump(data, open(path, 'wb'))
	print('Data saved for {} bones'.format(len(data.keys())))

def load_dict(curves, rig, name = 'LastAnimation'):
   
	concerned_bones = list(curves.keys())

	rig.create_action(name)

	bpy.ops.pose.select_all(action = 'SELECT')
	bpy.ops.pose.loc_clear()
	bpy.ops.pose.rot_clear()

	set_frame(0)
	bpy.ops.anim.keyframe_insert_menu(type = '__ACTIVE__', confirm_success = True)

	for bone in concerned_bones:

	# if bone in curves.keys():
		frames_for_current_bone = curves[bone]['LocX']['frames']
		rig.select_from_name(bone)
		for f in frames_for_current_bone: 
		    set_frame(f)
		    try: 
		        bpy.ops.anim.keyframe_insert_menu(type = '__ACTIVE__', confirm_success = True)
		    except RuntimeError: 
		        print('Bone : {} could not be found'.format(bone))

	source_curves = rig.curves

	ratio = 1.

	curve_counter = 0 
	current_name, current_mode, current_dico = get_infos(source_curves[0], rig) 
	for s_c in zip(source_curves):

		# print(s_c[0].group.name)
		actual_source_curve = s_c[0]

		if actual_source_curve.group.name != current_name: 
			current_name, current_mode, current_dico = get_infos(actual_source_curve, rig)
			curve_counter = 0 

		if current_name in list(curves.keys()):
			actual_target_curve = curves[current_name][current_dico[curve_counter]]

			for idx, kf in enumerate(actual_source_curve.keyframe_points): 
			    ref = actual_target_curve['points'][idx]
			    kf.co.y = ref['p'][1]*ratio
			    kf.handle_left = m.Vector(ref['lh'])
			    kf.handle_right = m.Vector(ref['rh'])

			    kf.handle_left.y *= ratio
			    kf.handle_right.y *= ratio
			    kf.handle_left.x *= ratio
			    kf.handle_right.x *= ratio

		curve_counter += 1 

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
        curves = pickle.load(open(full_path, 'rb'))
        target_armature = Rig(bpy.data.objects['Armature'])
        # load_all(full_path, target_armature, 'LastLoaded')
        load_dict(curves, target_armature, 'LastLoaded')

    def launch_save(self): 

        full_path = self.path_to_anim + self.anim_name
        source_armature = Rig(bpy.data.objects['Armature'])
        # curves = source_armature.curves
        # save_all(curves, source_armature,full_path)
        save_dict(source_armature, full_path)


bpy.utils.register_class(DialogOperator)

# test call
bpy.ops.object.dialog_operator('INVOKE_DEFAULT', path_to_anim = '/home/mehdi/Bureau/', anim_name = 'Test', saving = False)