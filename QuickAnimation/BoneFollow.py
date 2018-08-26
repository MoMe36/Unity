import bpy 
import math 
import mathutils as m 



def select_bone_from_name(rig, name): 

	bpy.ops.pose.select_all(action ='DESELECT')
	rig.data.bones.active = rig.data.bones[name]
	rig.data.bones[name].select = True

def set_frame(nb): 
	bpy.context.scene.frame_set(nb)
	print('Current frame is: {}'.format(bpy.context.scene.frame_current))

def add_kf_to_bone(rig, name): 

	select_bone_from_name(rig, name)
	bpy.ops.anim.keyframe_insert_menu(type = '__ACTIVE__', confirm_success = False)


def follow_in_animation(rig, target_name, bone_name): 

	rig = bpy.data.objects['Armature']
	rig.pose.bones[bone_name].rotation_mode  = rig.pose.bones[target_name].rotation_mode

	current_animation = rig.animation_data.action
	curves = current_animation.fcurves
	set_frame(0)
	bpy.ops.pose.select_all(action='SELECT')
	bpy.ops.anim.keyframe_insert_menu(type = '__ACTIVE__', confirm_success = False)
	set_frame(int(current_animation.fcurves[0].keyframe_points[-1].co.x))
	bpy.ops.anim.keyframe_insert_menu(type = '__ACTIVE__', confirm_success = False)


	target_bone = rig.pose.bones[target_name]
	source_bone = rig.pose.bones[bone_name]


	for curve in curves:

		if(curve.group.name == target_name): 

			for kf in curve.keyframe_points: 

				set_frame(int(kf.co.x))

				source_bone.matrix = target_bone.matrix
				select_bone_from_name(rig, bone_name)
				bpy.ops.anim.keyframe_insert_menu(type = '__ACTIVE__', confirm_success = False)
			break 

class DialogOperator(bpy.types.Operator):
	bl_idname = "object.dialog_operator"
	bl_label = "Bone follow"

	armature_name = bpy.props.StringProperty(name = "Armature name ?")
	bone_name = bpy.props.StringProperty(name="Following bone")
	bone_target_name = bpy.props.StringProperty(name="Bone to follow")
	all_anims = bpy.props.BoolProperty(name="All animations ?")
	# path_to_anim += "/home/mehdi/Blender/Scripts/"

	def execute(self, context):

		if self.all_anims: 
			self.launch_follow_all()
		else: 
			self.launch_follow()

		message = 'Finished following {} by {}'.format(self.bone_target_name, self.bone_name)
		self.report({'INFO'}, message)

		return {'FINISHED'}

	def invoke(self, context, event):
		wm = context.window_manager
		return wm.invoke_props_dialog(self)

	def launch_follow_all(self): 

		all_actions = bpy.data.actions
		rig = bpy.data.objects[self.armature_name]
		for action in all_actions: 

			rig.animation_data.action = action
			follow_in_animation(rig, self.bone_target_name, self.bone_name)


	def launch_follow(self): 

		rig = bpy.data.objects[self.armature_name]
		follow_in_animation(rig,self.bone_target_name, self.bone_name)


bpy.utils.register_class(DialogOperator)

# test call
bpy.ops.object.dialog_operator('INVOKE_DEFAULT', armature_name = 'Armature')

