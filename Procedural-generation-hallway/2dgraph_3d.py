# making the 3d model from 2d coordinates
# doesn't give thickness to walls. 
import numpy as np


#output from pathway gen
#coordinates and node number
edges = [(1, 5), (1, 6), (2, 9), (2, 8), (3, 6), (3, 7), (4, 10), (4, 80), (5, 9), (5, 14), (6, 140), (7, 10), (7, 150), (8, 13), (8, 80), (9, 13), (10, 160), (11, 14), (11, 12), (12, 13), (140, 110), (110, 150), (80, 160), (150, 160)]

nodes = {1: (1, 1), 2: (1, 12), 3: (6, 1), 4: (6, 12), 5: (1, 6), 6: (3, 1), 7: (6, 6), 8: (4, 12), 9: (1, 10), 10: (6, 9), 11: (4, 6), 12: (4, 9), 13: (4, 10), 14: (3, 6), 140: (3, 5.5), 110: (4.5, 5.5), 80: (4.5, 12), 150: (4.5, 6), 160: (4.5, 9)}

bottom = [(1,3), (3,4), (4,2), (2,1)]

vertices = []
faces = []
depth = 2

def find_vertex(vertices, vertex):
	index = -1
	for i in range(0, len(vertices)):
		v = vertices[i]
		if v == vertex:
			index = i
			break	
	return index

def add_vertex(vertices, vertex):
	index = find_vertex(vertices, vertex)
	if index == -1:
		vertices.append(vertex)
		index = len(vertices) - 1

	return (index, vertices)

for e in edges:
	v1 = nodes[e[0]]
	v2 = nodes[e[1]]

	index1, vertices = add_vertex(vertices, [v1[0], v1[1], 0]) # 1
	index2, vertices = add_vertex(vertices, [v2[0], v2[1], 0]) # 2
	index3, vertices = add_vertex(vertices, [v1[0], v1[1], depth]) # 3
	index4, vertices = add_vertex(vertices, [v2[0], v2[1], depth]) # 4

	faces.append([index1+1,index3+1,index4+1,index2+1])

# Bottom
b1 = nodes[bottom[0][0]]
b2 = nodes[bottom[1][0]]
b3 = nodes[bottom[2][0]]
b4 = nodes[bottom[3][0]]
faces.append([ find_vertex(vertices, [b1[0], b1[1], 0]) + 1,
				find_vertex(vertices, [b2[0], b2[1], 0]) + 1,
				find_vertex(vertices, [b3[0], b3[1], 0]) + 1,
				find_vertex(vertices, [b4[0], b4[1], 0]) + 1,
			 ])

for v in vertices:
	print(v)

print("Faces")

for f in faces:
	print(f)

with open('building.obj', 'w') as f:
	for v in vertices:
		f.write("v %.4f %.4f %.4f\n" % (v[0],v[1],v[2]))
	for fc in faces:
		f.write("f %d %d %d %d\n" % (fc[0],fc[1],fc[2],fc[3]))

