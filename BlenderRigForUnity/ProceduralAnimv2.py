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

			pickled_pose = pickle.load(open(path, 'rb'))
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


			pickle.dump(to_be_pickled, open(path, 'wb'))

	
		
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

	def RunMale(self): 

		self.armature.animation_data_create()
		self.armature.animation_data.action = bpy.data.actions.new(name = 'Run')
		self.reset_all()
		set_frame(1)

		
		self.move('footL', m.Vector((0.0,0.6468410491943359,0.32542940974235535)))
		self.rotate('footL', m.Quaternion((0.8095934391021729,0.5869910717010498,1.0933560723458413e-09,0.0)))
		self.move('footR', m.Vector((0.0,-0.15668821334838867,0.37364116311073303)))
		self.rotate('footR', m.Quaternion((0.9968357682228088,0.07948871701955795,1.4805927317507184e-10,0.0)))
		self.move('armL', m.Vector((0.44656145572662354,0.8304474949836731,0.783440887928009)))
		self.move('armR', m.Vector((-0.4655771255493164,0.21218188107013702,0.07616782188415527)))
		self.rotate('pelvis', m.Quaternion((0.9693430066108704,0.24575096368789673,0.0,-4.5774670165421583e-10)))
		self.rotate('spine1', m.Quaternion((0.9893530011177063,0.11518877744674683,0.047148920595645905,-0.07542866468429565)))
		self.rotate('spine2', m.Quaternion((0.9884982109069824,0.0800093486905098,0.08261135220527649,-0.09820865094661713)))
		self.move('targetAL', m.Vector((0.7912756204605103,1.1956740617752075,0.76777184009552)))
		self.move('targetAR', m.Vector((-0.0,-0.0,1.172295093536377)))


		self.confirm_animation_pose()
		add_frames(10)


		self.move('footL', m.Vector((0.0,0.8242871761322021,0.7037578225135803)))
		self.rotate('footL', m.Quaternion((0.8095934391021729,0.5869910717010498,1.0933560723458413e-09,0.0)))
		self.move('footR', m.Vector((0.0,-0.5718450546264648,0.16606272757053375)))
		self.rotate('footR', m.Quaternion((0.9527202844619751,-0.3038487732410431,0.0,0.0)))
		self.move('armL', m.Vector((0.827400803565979,1.3146575689315796,0.930336058139801)))
		self.move('armR', m.Vector((-0.6877334117889404,0.0870489701628685,0.41257596015930176)))
		self.rotate('pelvis', m.Quaternion((0.9693430066108704,0.24575096368789673,0.0,-4.5774670165421583e-10)))
		self.rotate('spine1', m.Quaternion((0.9904395341873169,0.06155836582183838,0.06543537974357605,-0.10468326508998871)))
		self.rotate('spine2', m.Quaternion((0.9805094003677368,0.08000271022319794,0.09998319298028946,-0.1490096002817154)))
		self.move('targetAL', m.Vector((0.7912756204605103,1.1956740617752075,0.76777184009552)))
		self.move('targetAR', m.Vector((-0.0,-0.0,1.172295093536377)))


		self.confirm_animation_pose()
		add_frames(10)

		self.move('footL', m.Vector((-1.3754917915775877e-07,0.35181185603141785,0.7230424880981445)))
		self.rotate('footL', m.Quaternion((-0.6485740542411804,-0.7611515522003174,-1.0626004520730703e-09,0.0)))
		self.move('footR', m.Vector((0.0,0.07901378720998764,0.16606272757053375)))
		self.move('armL', m.Vector((0.7725597620010376,0.5312165021896362,1.055686593055725)))
		self.move('armR', m.Vector((-0.8365870118141174,0.47093522548675537,0.7572900056838989)))
		self.move('pelvis', m.Vector((-1.212404932005029e-08,0.004821172449737787,-0.05303293839097023)))
		self.rotate('pelvis', m.Quaternion((0.9693430066108704,0.24575096368789673,0.0,-4.5774670165421583e-10)))
		self.rotate('spine1', m.Quaternion((0.9856981635093689,0.1685202419757843,0.0,-5.551115123125783e-17)))
		self.rotate('spine2', m.Quaternion((0.9969902634620667,0.0775270164012909,-2.7755575615628914e-17,1.1102230246251565e-16)))
		self.move('targetAL', m.Vector((0.7912756204605103,1.1956740617752075,0.76777184009552)))
		self.move('targetAR', m.Vector((-0.0,-0.0,1.172295093536377)))

		self.confirm_animation_pose()

	def IdleCombat(self): 

		self.armature.animation_data_create()
		self.armature.animation_data.action = bpy.data.actions.new(name = 'IdleCombat')
		self.reset_all()
		set_frame(1)

		self.move('footL', m.Vector((0.21914754807949066,0.4426664113998413,0.0)))
		self.rotate('footL', m.Quaternion((-0.9511075615882874,0.0,0.0,-0.30885985493659973)))
		self.move('footR', m.Vector((-0.1480371057987213,-0.11457973718643188,0.0)))
		self.move('armL', m.Vector((0.7801657915115356,-0.42323073744773865,1.1484779119491577)))
		self.move('armR', m.Vector((-1.1379348039627075,0.2638631761074066,1.2705512046813965)))
		self.move('pelvis', m.Vector((-3.717063881911669e-18,0.11718139052391052,-0.23101471364498138)))
		self.rotate('pelvis', m.Quaternion((0.997765302658081,-0.06696155667304993,2.3807040783196953e-09,2.248355945866365e-10)))
		self.rotate('spine1', m.Quaternion((0.9869847893714905,0.16081368923187256,4.4861780956750863e-10,-2.3539430404895256e-09)))
		self.rotate('spine2', m.Quaternion((0.9873967170715332,0.1582651138305664,4.415078858066579e-10,-2.3166366602822563e-09)))
		self.rotate('head', m.Quaternion((0.9878208041191101,-0.1555955410003662,-6.012639275354559e-10,-3.18025139556255e-09)))
		self.move('targetLL', m.Vector((-0.8584177494049072,-0.0,-0.0)))
		self.move('targetAL', m.Vector((1.1125181913375854,-0.36474359035491943,-0.0)))
		self.move('targetAR', m.Vector((-2.8919358253479004,1.0182238817214966,-0.0)))

		self.confirm_animation_pose()
		add_frames(10)

		self.move('footL', m.Vector((0.21914754807949066,0.4426664113998413,0.0)))
		self.rotate('footL', m.Quaternion((-0.9511075615882874,0.0,0.0,-0.30885985493659973)))
		self.move('footR', m.Vector((-0.1480371057987213,-0.11457973718643188,0.0)))
		self.move('armL', m.Vector((0.7801657915115356,-0.4341118633747101,1.0451072454452515)))
		self.move('armR', m.Vector((-1.1379348039627075,0.2529820501804352,1.363040804862976)))
		self.move('pelvis', m.Vector((-7.434134381268239e-18,0.13726963102817535,-0.24775491654872894)))
		self.rotate('pelvis', m.Quaternion((0.9984706044197083,-0.05546087026596069,2.3779556101999333e-09,2.522512199121252e-10)))
		self.rotate('spine1', m.Quaternion((0.9869848489761353,0.16081368923187256,5.027395166834481e-10,-2.3429809203889818e-09)))
		self.rotate('spine2', m.Quaternion((0.9804763793945312,0.19663673639297485,5.842855643756195e-10,-2.2848078984338827e-09)))
		self.rotate('head', m.Quaternion((0.9885472059249878,-0.15091297030448914,-2.621578509831579e-10,-3.2259568349957135e-09)))
		self.move('targetLL', m.Vector((-0.8584177494049072,-0.0,-0.0)))
		self.move('targetAL', m.Vector((1.1125181913375854,-0.36474359035491943,-0.0)))
		self.move('targetAR', m.Vector((-2.8919358253479004,1.0182238817214966,-0.0)))

		self.confirm_animation_pose()

	def HitUppercut(self): 

		self.armature.animation_data_create()
		self.armature.animation_data.action = bpy.data.actions.new(name = 'Uppercut')
		self.reset_all()
		set_frame(1)

		self.move('footL', m.Vector((0.21914756298065186,0.4426664412021637,0.0)))
		self.rotate('footL', m.Quaternion((-0.9511076211929321,0.0,0.0,-0.3088598847389221)))
		self.move('footR', m.Vector((-0.1480371057987213,-0.11457973718643188,0.0)))
		self.move('armL', m.Vector((0.7801657915115356,-0.42323073744773865,1.1484779119491577)))
		self.move('armR', m.Vector((-1.1379348039627075,0.2638631761074066,1.2705512046813965)))
		self.move('pelvis', m.Vector((-3.717063881911669e-18,0.11718139797449112,-0.23101471364498138)))
		self.rotate('pelvis', m.Quaternion((0.9977654218673706,-0.06696084141731262,2.2483326311828478e-10,-2.3806798754577585e-09)))
		self.rotate('spine1', m.Quaternion((0.9869847297668457,0.16081371903419495,4.4861692138908893e-10,-2.3539425964003158e-09)))
		self.rotate('spine2', m.Quaternion((0.9873965978622437,0.15826478600502014,4.4150705313938943e-10,-2.316631331211738e-09)))
		self.rotate('head', m.Quaternion((0.9878209829330444,-0.15559512376785278,-6.012628173124313e-10,-3.1802411815107234e-09)))
		self.move('targetLL', m.Vector((-0.8584177494049072,-0.0,-0.0)))
		self.move('targetAL', m.Vector((1.1125181913375854,-0.36474359035491943,-0.0)))
		self.move('targetAR', m.Vector((-2.8919358253479004,1.0182238817214966,-0.0)))



		self.confirm_animation_pose()
		add_frames(10)

		self.move('footL', m.Vector((0.21914757788181305,0.4426664710044861,0.0)))
		self.rotate('footL', m.Quaternion((-0.9511077404022217,0.0,0.0,-0.3088594973087311)))
		self.move('footR', m.Vector((-0.1480371057987213,-0.296706885099411,0.0)))
		self.move('armL', m.Vector((0.8935108184814453,0.613247275352478,1.2770617008209229)))
		self.move('armR', m.Vector((-0.7297666072845459,-0.4305644631385803,1.0774867534637451)))
		self.move('pelvis', m.Vector((0.08292598277330399,0.11718141287565231,-0.3189782500267029)))
		self.rotate('pelvis', m.Quaternion((0.9918255805969238,-0.06656268239021301,0.007295642048120499,-0.10870914161205292)))
		self.rotate('spine1', m.Quaternion((0.9869809150695801,0.15637899935245514,-7.817521691322327e-05,0.03760858625173569)))
		self.rotate('spine2', m.Quaternion((0.8314930200576782,0.30407586693763733,0.14918692409992218,-0.44034144282341003)))
		self.rotate('head', m.Quaternion((0.9878208637237549,-0.08625863492488861,0.11863088607788086,0.05192394554615021)))
		self.move('targetLL', m.Vector((-0.8584177494049072,-0.0,-0.0)))
		self.move('targetAL', m.Vector((1.1125181913375854,-0.36474359035491943,1.0242846012115479)))
		self.move('targetAR', m.Vector((0.33159828186035156,1.0182238817214966,-0.0)))



		self.confirm_animation_pose()
		add_frames(10)

		self.move('footL', m.Vector((0.21914757788181305,0.5147422552108765,0.0)))
		self.rotate('footL', m.Quaternion((-0.9511077404022217,0.0,0.0,-0.3088594973087311)))
		self.move('footR', m.Vector((-0.1480371057987213,-0.30503979325294495,0.0)))
		self.move('armL', m.Vector((0.7044332027435303,0.03140902519226074,0.9562919735908508)))
		self.move('armR', m.Vector((-1.3558244705200195,0.9278703331947327,1.3841233253479004)))
		self.move('pelvis', m.Vector((0.11315133422613144,-0.07114581018686295,-0.3515286147594452)))
		self.rotate('pelvis', m.Quaternion((0.9960921406745911,0.07039684057235718,0.010872609913349152,-0.052402615547180176)))
		self.rotate('spine1', m.Quaternion((0.9693575501441956,0.034879058599472046,-0.0490303710103035,0.23817075788974762)))
		self.rotate('spine2', m.Quaternion((0.9576491117477417,0.2825251519680023,-0.04307006299495697,0.03510802984237671)))
		self.rotate('head', m.Quaternion((0.9878209829330444,-0.12834756076335907,-0.04279458522796631,-0.07684636116027832)))
		self.move('targetLL', m.Vector((-0.8584177494049072,-0.0,-0.0)))
		self.move('targetAL', m.Vector((0.2428983449935913,-0.36474359035491943,1.0242846012115479)))
		self.move('targetAR', m.Vector((-1.7931771278381348,1.0182238817214966,0.21310532093048096)))



		self.confirm_animation_pose()
		add_frames(10)

		self.move('footL', m.Vector((0.21914757788181305,0.32993045449256897,0.0)))
		self.rotate('footL', m.Quaternion((-0.9511077404022217,0.0,0.0,-0.3088594973087311)))
		self.move('footR', m.Vector((-0.1480371057987213,-0.30503979325294495,0.0)))
		self.move('armL', m.Vector((0.7958346605300903,-0.4974138140678406,0.7473743557929993)))
		self.move('armR', m.Vector((-1.4994553327560425,1.344617486000061,0.1578199863433838)))
		self.move('pelvis', m.Vector((0.11315134167671204,-0.09123404324054718,-0.26314035058021545)))
		self.rotate('pelvis', m.Quaternion((0.9751129150390625,-0.027260690927505493,0.0011711642146110535,-0.2200678586959839)))
		self.rotate('spine1', m.Quaternion((0.9504943490028381,0.06152735650539398,0.00964134931564331,0.30443722009658813)))
		self.rotate('spine2', m.Quaternion((0.9609696269035339,0.0780058428645134,-0.04139180853962898,0.26218199729919434)))
		self.move('targetLL', m.Vector((-0.8584177494049072,-0.0,-0.0)))
		self.move('targetAL', m.Vector((0.2428983449935913,-0.36474359035491943,1.0242846012115479)))
		self.move('targetAR', m.Vector((-1.3990931510925293,-1.183652639389038,0.05249989032745361)))

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

sketelon.print_pose()


# sketelon.RunMale()
sketelon.HitUppercut()
