# Creates a list of empty lists
def create_list_of_empty_lists(length):
    return [ [] for _ in range(0, length) ]

# Converts a face-wise matrix into a vertex-wise matrix
def facematrix_to_vertexmatrix(f_matrix, vert_count):
    v_matrix = create_list_of_empty_lists(vert_count)
    for vert_id in range(0, vert_count):
        v_matrix[vert_id].append(vert_id) # For convenience, the first entry of every row is the vertex's id/num/index
        for face_id in range(0, len(f_matrix)):
            if vert_id in f_matrix[ face_id ]:
                v_matrix[vert_id].append(face_id)
    return v_matrix

"""""
# UNUSED
# Terrible sort
def sort_by_len(v_matrix):

    working_matrix = v_matrix.copy()
    sorted_matrix = []
    while len(working_matrix) > 0:
        best_vert_index = 0
        for curr_vert_index in range(0, len(working_matrix)):
            if len(working_matrix[curr_vert_index]) > len(working_matrix[best_vert_index]):
                best_vert_index = curr_vert_index
        sorted_matrix.append( working_matrix[best_vert_index].copy() )
        working_matrix.remove( working_matrix[best_vert_index] )

    return sorted_matrix
"""

# Get shortest path length to any in set
def get_shortest_path_len_to_any_in_set(vert, covering_verts, all_faces, all_edges):
    """""
    # Remove own vert ID as the first entry in each row of covering_verts
    # Necessary in order to easily check if a face is in a row
    for i in range(len(covering_verts)):
        covering_verts[i].remove(covering_verts[i][0])

    # Convert covering_verts into covered_faces
    covered_faces = []
    for face in all_faces:
        for vert_id in face:
            for covering_vert in covering_verts:
                if vert_id == covering_vert[0]:
                    covered_faces.append( face.copy() )
                    break
    """

    shortest_path_len = 10000
    #TODO: get shortest path to any in set

    return 1

# Get vertex with longest list of connected faces, and furthest from any covering verts wins ties
def get_next_best_vert(vert_candidates, all_faces, all_edges, covering_set):
    tied_verts = [] # tied for best

    # Determine highest length
    highest_len = 0
    for vert in vert_candidates:
        if len(vert) > highest_len:
            print("len(vert " + str(vert[0]) + ") = " + str(len(vert)) + " > highest len " + str(highest_len))
            highest_len = len(vert)

    # Determine which verts match the highest length (they are tied for best)
    for vert in vert_candidates:
        if len(vert) == highest_len:
            print("len(vert) == " + str(len(vert)) + " == highest_len == " + str(highest_len))
            tied_verts.append( vert.copy() )
            if vert in covering_set:
                print("Duplicate vert detected")

    # Tiebreaker: shortest path to covering set
    if len( tied_verts ) == 0:
        print("len(tied_verts) == 0")
        return
    best_tied_vert = tied_verts[0]
    shortest_path_from_best_tied_vert = get_shortest_path_len_to_any_in_set(best_tied_vert, all_faces, all_edges, covering_set)
    for tied_vert in tied_verts:
        if get_shortest_path_len_to_any_in_set(tied_vert, covering_set, all_faces, all_edges) < shortest_path_from_best_tied_vert:
            best_tied_vert = tied_vert
            # If they tie again, first in matrix wins by default

    return best_tied_vert


# Removes all instances of a given face no. in a vertex matrix
def remove_face(face, v_matrix):
    for v_row in v_matrix:
        if face in v_row:
            v_row.remove(face)
    return v_matrix

# Checks if all faces in the object are covered by the given vertices
def covers(covering_vertices, all_faces, vert_count):
    if len(covering_vertices) <= 2: # 2 is the lower bound
        print("Doesn't cover because too few verts")
        return False
    elif len(covering_vertices) == vert_count: # the number of faces in the shape is the upper bound
        print("Covers because max number of verts")
        return True
    elif len(covering_vertices) > vert_count: # over max
        print("Problem: Over max verts in the covering set")
        return False
    # else: less than covers
    print("asdlkj")

    all_faces_copy = all_faces.copy()

    for covering_vertex in covering_vertices:
        for face in all_faces:
            if covering_vertex[0] in face:
                try:
                    all_faces_copy.remove(face)
                except Exception:
                    print("Face already removed")

    covers = ( len(all_faces_copy) == 0 )
    print("Does it cover?: " + str(covers))
    return covers

# The main function
# Picks the most connected vertex until all faces are covered
# Note: This only checks one combination of vertices. Not guaranteed to find the best/minimal combination.
def greedy(faces, edges, vert_count):
    covering_vs = []

    verts = facematrix_to_vertexmatrix(faces, vert_count)
    working_verts = verts.copy()

    while not covers(covering_vs, faces, vert_count):
        # Add most-connected vertex to the covering set
        next_best_vert = get_next_best_vert(working_verts, faces, edges, covering_vs).copy()
        covering_vs.append( next_best_vert )
        # Consider those faces covered (remove them from the working matrix)
        for face in next_best_vert:
            working_verts = remove_face(face, working_verts)
            #TODO: remove_face is not working

    return covering_vs

# Test greedy()
def main():
    # TODO: Eventually change it to read this data from a text file
    # To test, here is the data for a cube. Reference image
    cube_faces = [[0, 1, 2, 3],
                  [0, 1, 4, 5],
                  [0, 2, 4, 6],
                  [2, 3, 6, 7],
                  [3, 1, 7, 5],
                  [4, 5, 6, 7]]
    cube_edges = [[0,1],[2,3],[0,2],[1,3],
                  [4,5],[6,7],[4,6],[5,7]]
    cube_vert_count = 8

    # Run greedy approach
    print("Running greedy approach")
    covering_vertices = greedy(cube_faces, cube_edges, cube_vert_count)

    # Print result
    print("Covering set = { ", end="")
    for covering_vertex in covering_vertices:
        print(covering_vertex[0], end=" ")
    print("}")

    return


if __name__ == '__main__':
    main()
