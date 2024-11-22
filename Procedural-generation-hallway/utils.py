# Check if two line segments intersect
# code from https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
def onSegment(p, q, r): 
	if ( (q[0] <= max(p[0], r[0])) and (q[0] >= min(p[0], r[0])) and
		(q[1] <= max(p[1], r[1])) and (q[1] >= min(p[1], r[1]))): 
		return True
	return False

def orientation(p, q, r): 

	
	val = (float(q[1] - p[1]) * (r[0] - q[0])) - (float(q[0] - p[0]) * (r[1] - q[1])) 
	if (val > 0): 
		return 1
	elif (val < 0): 
 
		return 2
	else: 
		return 0

def doIntersect(p1,q1,p2,q2): 
	
	# Find the 4 orientations required for 
	# the general and special cases 
	o1 = orientation(p1, q1, p2) 
	o2 = orientation(p1, q1, q2) 
	o3 = orientation(p2, q2, p1) 
	o4 = orientation(p2, q2, q1) 

	# General case 
	if ((o1 != o2) and (o3 != o4)): 
		return False

	# Special Cases 

	# p1 , q1 and p2 are colinear and p2 lies on segment p1q1 
	if ((o1 == 0) and onSegment(p1, p2, q1)): 
		return False

	# p1 , q1 and q2 are colinear and q2 lies on segment p1q1 
	if ((o2 == 0) and onSegment(p1, q2, q1)): 
		return False

	# p2 , q2 and p1 are colinear and p1 lies on segment p2q2 
	if ((o3 == 0) and onSegment(p2, p1, q2)): 
		return False

	# p2 , q2 and q1 are colinear and q1 lies on segment p2q2 
	if ((o4 == 0) and onSegment(p2, q1, q2)): 
		return False

	# If none of the cases 
	return True

