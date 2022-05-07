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

def faceMatrixMaker(shapeArr):
    faceMatrix = []
    for face in shapeArr.splitlines():
        faceMatrix.append(face.split(','))
    return faceMatrix

from pandas import read_csv

def get600CellFromGithub():
    url = "https://raw.githubusercontent.com/rkrawczyk9/BUPlatonicSolids/develop/sixhundredcell_3_corrected.csv"
    shapeMatrix = read_csv(url, header = None)
    return shapeMatrix.to_numpy()

def get120CellFromGithub():
    url = "https://raw.githubusercontent.com/rkrawczyk9/BUPlatonicSolids/develop/120cell_facets.csv"
    shapeMatrix = read_csv(url, header = None)
    return shapeMatrix.to_numpy()

def parseCSV(filePath):
    shapeMatrix = read_csv(filePath,header=None)
    return shapeMatrix.to_numpy()

""" if __name__ == '__main__':
    """