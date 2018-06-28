import bpy
import mathutils as m

bpy.ops.object.armature_add()
arma = bpy.ops.object 

arma.editmode_toggle()

arma = bpy.ops.transform
arma.translate(value = (0,0,8))

arma = bpy.ops.armature

arma.select_all(action = 'SELECT')

for a in range(2): 
    arma.subdivide()
    
    
arma = bpy.data.objects["Armature"].data


i = 0
nom = ['Pelvis', 'Spine2', 'Spine1', 'Head']

for b in bpy.context.selected_editable_bones: 
    b.select = False
    b.name = nom[i]
    i = i+1


# -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_

# -_-_-_-_-_-_-_-_ Ajout des bras -_-_-_-_-_-_-_-_-_-_-_-_

ob = bpy.context.object
arm = ob.data

sh = arm.edit_bones.new('Shoulder L')
sh.head = (0.3,-0.3,6)
sh.tail = sh.head + m.Vector((1,0.3,0))

Ep = arm.edit_bones.new('Upperarm L')
Ep.head = (1.5,0,6)
Ep.tail = Ep.head + m.Vector((2,0,0))

bras = arm.edit_bones.new('Arm L')
bras.head = Ep.tail
bras.tail = bras.head + m.Vector((2,0,0))

main = arm.edit_bones.new('Hand L')
main.head = bras.tail
main.tail = main.head + m.Vector((1,0,0))

arma = bpy.data.objects["Armature"].data

arma.edit_bones['Shoulder L'].parent = arma.edit_bones['Spine2']

arma.edit_bones['Upperarm L'].parent = arma.edit_bones['Shoulder L']
arma.edit_bones['Arm L'].parent = arma.edit_bones['Upperarm L']
arma.edit_bones['Arm L'].use_connect = True

arma.edit_bones['Hand L'].parent = arma.edit_bones['Arm L']
arma.edit_bones['Hand L'].use_connect = True



# -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_

# -_-_-_-_-_-_-_-_ Ajout des jambes -_-_-_-_-_-_-_-_-_-_-_-_


Cuisse = arm.edit_bones.new('Leg L')
Cuisse.head = (0.5,0,0)
Cuisse.tail = (0.5,0,-3)

Mollet = arm.edit_bones.new('Knee L')
Mollet.head = Cuisse.tail
Mollet.tail = (0.5,0,-6)

Pied = arm.edit_bones.new('Foot L')
Pied.tail = (0.5,-1,-6.2)
Pied.head = Mollet.tail

Pied2 = arm.edit_bones.new('FootBis L')
Pied2.tail = (0.5,-1,-6.2)
Pied2.head = Mollet.tail

arma.edit_bones['Leg L'].parent = arma.edit_bones['Pelvis']
arma.edit_bones['Knee L'].parent = arma.edit_bones['Leg L']
arma.edit_bones['Foot L'].parent = arma.edit_bones['Knee L']
arma.edit_bones['FootBis L'].parent = arma.edit_bones['Knee L']

arma.edit_bones['Knee L'].use_connect = True
arma.edit_bones['Foot L'].use_connect = True

Cuisse.tail = Cuisse.tail + m.Vector((0,-0.1,0))
Ep.tail = Ep.tail + m.Vector((0,0.1,0))




# -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_

# -_-_-_-_-_-_-_-_ Ajout IK et poT -_-_-_-_-_-_-_-_-_-_-_-_


IKBL = arm.edit_bones.new('IK Arm L')
IKBL.head = bras.tail
IKBL.tail = IKBL.head + m.Vector((0,0.8,0))

IKJL = arm.edit_bones.new('IK Leg L')
IKJL.head = Pied.head
IKJL.tail = IKJL.head + m.Vector((0,0.8,0))


pj = arm.edit_bones.new('IKT Leg L')
pj.tail = Mollet.head + m.Vector((0,-6,0))
pj.head = pj.tail + m.Vector((0,0.8,0))

pb = arm.edit_bones.new('IKT Arm L')
pb.tail = bras.head + m.Vector((0,-6,0))
pb.head = pb.tail + m.Vector((0,0.8,0))


# -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_

# -_-_-_-_-_-_-_-_ Ajout Contrôle pied L-_-_-_-_-_-_-_-_-_-_-_-_

CPL = bpy.context.object.data.edit_bones.new('Foot Control L')
CPL.head = Pied.head + m.Vector((0,0,-0.5)) + m.Vector((0,0.5,0))
CPL.tail = CPL.head + m.Vector((0,-1.5,0))

IKJL.parent = CPL
Pied2.use_connect = False
Pied2.parent= CPL
Pied2.use_deform = False

NDF = [CPL, IKBL, IKJL, pj, pb]

for ndf in NDF:
    ndf.use_deform = False
    
# -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_

# -_-_-_-_-_-_-_-_ Ajout Relations IK -_-_-_-_-_-_-_-_-_-_-_-_


bpy.ops.object.posemode_toggle()
pArm = bpy.context.object.pose


pMol = pArm.bones['Knee L']
pIKJ = pArm.bones['IK Leg L']

pB = pArm.bones['Arm L']
pIKB = pArm.bones['IK Arm L']


pMol.constraints.new('IK')
pMol.constraints['IK'].target = bpy.context.object
pMol.constraints['IK'].subtarget = 'IK Leg L'
pMol.constraints['IK'].chain_count = 2
pMol.constraints['IK'].pole_target = bpy.context.object
pMol.constraints['IK'].pole_subtarget = 'IKT Leg L'

pB.constraints.new('IK')
pB.constraints['IK'].target = bpy.context.object
pB.constraints['IK'].subtarget = 'IK Arm L'
pB.constraints['IK'].chain_count = 2
pB.constraints['IK'].pole_target = bpy.context.object
pB.constraints['IK'].pole_subtarget = 'IKT Arm L'


pCon = pArm.bones ['Foot L']
pCon.constraints.new('COPY_LOCATION')
pCon.constraints["Copy Location"].target = bpy.context.object
pCon.constraints["Copy Location"].subtarget = "Knee L"
pCon.constraints["Copy Location"].head_tail = 1

pCon.constraints.new('COPY_ROTATION')
pCon.constraints["Copy Rotation"].target = bpy.context.object
pCon.constraints["Copy Rotation"].subtarget = "FootBis L"


pCon = pArm.bones ['FootBis L']
pCon.constraints.new('COPY_LOCATION')
pCon.constraints["Copy Location"].target = bpy.context.object
pCon.constraints["Copy Location"].subtarget = "Knee L"
pCon.constraints["Copy Location"].head_tail = 1


# -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_

# -_-_-_-_-_-_-_-_ Finalisation -_-_-_-_-_-_-_-_-_-_-_-_

bpy.ops.object.editmode_toggle()
eArm = bpy.ops.armature
eArm.select_all(action = 'TOGGLE')
eArm.select_all(action = 'TOGGLE')

eArm.calculate_roll(type = 'GLOBAL_POS_X')
eArm.calculate_roll(type = 'GLOBAL_POS_Y')
eArm.calculate_roll(type = 'GLOBAL_POS_Z')

bpy.ops.object.posemode_toggle()

pMol.constraints['IK'].pole_angle = 3.1416/2

bpy.ops.object.editmode_toggle()
bpy.ops.armature.symmetrize()

bpy.ops.object.posemode_toggle() 


# Finish setting constraints 

pArm.bones['Arm R'].constraints["IK"].pole_angle = 3.1416

bpy.ops.object.editmode_toggle()
bpy.context.object.data.use_mirror_x = True
bpy.data.objects['Armature'].show_x_ray = True