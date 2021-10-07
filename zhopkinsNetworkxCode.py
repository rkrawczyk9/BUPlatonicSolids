import networkx as nx
import matplotlib.pyplot as plt
from string import ascii_uppercase

tetrahedron = [
    [1,1,1,0],
    [1,0,1,1],
    [1,1,0,1],
    [0,1,1,1]
]
sqaure = [
    [1,1,1,1,0,0,0,0],
    [1,1,0,0,1,1,0,0],
    [0,1,1,0,0,1,1,0],
    [0,0,1,1,0,0,1,1],
    [1,0,0,1,1,0,0,1],
    [0,0,0,0,1,1,1,1]
]

def edgeCreator(graph, startingPoint, arrEndingPoints, vertName):
    if arrEndingPoints == []:
        return graph
    
    for face in arrEndingPoints:
        graph.add_edge(startingPoint, face, label=vertName)
        
    newStartingPoint = arrEndingPoints.pop(0)
    return edgeCreator(graph, newStartingPoint, arrEndingPoints,vertName)

def crazyGraphMaker(shapeMatrix):

    #sets up the Graph
    shapeGraph = nx.Graph()
    
    #puts all the nodes into the graph
    for i in range(0,len(shapeMatrix)):
        shapeGraph.add_node(ascii_uppercase[i])
    
    
    
    #loop through the vertices
    for i in range(0,len(shapeMatrix[0])):
        
        #loops through and logs all faces a vertex is touching
        arrTouchingFaces = []
        for j in range(0,len(shapeMatrix)):
            if shapeMatrix[j][i] == 1:
                arrTouchingFaces.append(ascii_uppercase[j])
    
        #takes the first face and the array of the rest to compute all edges related to this vertx 
        startingFace = arrTouchingFaces.pop(0)
        edgeCreator(shapeGraph, startingFace, arrTouchingFaces, i+1)
        
    return shapeGraph


# In[81]:


tetraGraph = crazyGraphMaker(tetrahedron)
nx.draw(tetraGraph, with_labels=1)


# In[82]:


squareGraph = crazyGraphMaker(sqaure)
nx.draw(squareGraph, with_labels=1)


# In[94]:


squareGraph.edges()
edge_labels = nx.get_edge_attributes(squareGraph,"label")
print(edge_labels)

