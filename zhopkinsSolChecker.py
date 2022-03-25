"""
vertArr = an array of numbers that represent vertices
faceMatrix = matrix of each face of a shape and the vertices that touch them

uses the face matrix to check if the vetArr is a coverage of it
"""
def solChecker(vertArr, faceMatrix):
    #loops through each face
    for face in faceMatrix:
        #makes a list comperhension of all the verts in vert array that it is toching 
        #then checks if the length is 0
        if len([i for i in vertArr if i in face]) == 0:
            #if it is then it returns false
            return False
    #if it runs through every face then it is true
    return True