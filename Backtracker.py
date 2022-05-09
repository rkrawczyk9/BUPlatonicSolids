# Author: Sylvia Krech
# Bradley University CS Capstone: 4D Platonic Solids
#================================

# This file provides a class that can perform backtracking on existing solutions of
# If this file is ran directly, it will take commandline arguements to perform backtracking on a csv of existing solutions
#--------------------------------

# Possible improvements:
    # change offset to an object's variable, rather than passing it into a few functions. It should be known by then.
        # could even set it automatically then, and properly, during fit_shape_data()
    # generally speed up, I think most of the glaring logical optimizations are complete, and now its down to having more familiarity with python
        # on that note, best practice for speedup probably would be to recode this in a faster language
from os.path import exists
from itertools import combinations
import csv
import sys


class Backtracker:
    # declare/reset all the internal variables
    def __init__(self):
        # somewhat hardcoded. It should be flexible enough to change without erroring, but it defaults whats needed for the 600 cell.
        self.autoInclude = 24
        self.outputFilePath = "BacktrackerOutput.csv"
        self.maxAcceptableStartingSetSize = 51
        self.targetLength = 48
        self.numNodesToRemove = 3

        # To be replaced via fit_shape_data
        self.maxFacetsPerVertex = 0
        self.maxNumOfConnections = 0
        self.numNodes = 0
        self.numFaces = 0
        self.setOfFirst24Connections = set()
        self.completeSetOfFaces = set()

        # build lists for untouchedNodes()
        self.listOfFacetConnections = []
        self.listOfNodeConnections = []


    # Parameters: filepath to a csv of shape data
    # Results:
        # Updates all the internal variables, so that the run type commands will function
        # Returns None.
    def fit_shape_data(self, filePath):
        self.__init__() #clear internal variables incase this object is reused for some reason
        if exists(filePath):
            with open(filePath, newline='') as file:
                reader = csv.reader(file)
                data = list(reader)
                # pull data from the file
                for row in range(len(data)):
                    self.listOfFacetConnections.append(set())
                    for col in range(len(data[0])):
                        self.listOfFacetConnections[-1].add(int(data[row][col]))
                    if len(self.listOfFacetConnections[-1]) > self.maxNumOfConnections:
                        self.maxNumOfConnections = len(self.listOfFacetConnections[-1])

            # compute additional preprocessed constants for this shape
            tempSet = set()
            for i in self.listOfFacetConnections:
                tempSet.update(i)
            self.numNodes = len(tempSet) #this shouldn't be different than len(listOfSets) now that I think about it? unsure.

            self.numFaces = len(self.listOfFacetConnections)
            for i in range(self.numNodes):
                self.listOfNodeConnections.append(set())

            for i in range(len(self.listOfFacetConnections)):
                self.completeSetOfFaces.add(i+1) #offset by one because of our dataset

            for face in range(len(self.listOfFacetConnections)):
                for node in self.listOfFacetConnections[face]:
                    self.listOfNodeConnections[node-1].add(face+1)

            for i in range(24):
                self.setOfFirst24Connections.update(self.listOfNodeConnections[i])

            self.maxFacetsPerVertex = -1
            for i in self.listOfNodeConnections:
                if self.maxFacetsPerVertex < len(i):
                    self.maxFacetsPerVertex = len(i)
        else:
            print("Sorry, could not find that file")
        return


    # Parmeters:
        # inputSet is a set of nodes, cheat auto adds the first 24 nodes,
        # offset shifts all the values in inputSet down by one to work with the
        # list properly, and setOutput changes the output from a number to a set of facets.
    # Returns:
        # if inputSet is a solution this returns 0 OR set() if setOutput = True
        # otherwise returns the number of faces not covered, OR if setOutput = true, the facets that are untouched
    def untouched_facets(self, inputSet, cheat = False, offset = True, setOutput = True):
        #constructs a set of faces that the input nodes connect to, and checks that against self.completeSetOfFaces
        solutionSet = set()

        # add the facets connecting to the set in inputSet
        for i in inputSet:
            try:
                solutionSet.update(self.listOfNodeConnections[i - offset])
            except:
                print("data probably didn't import properly", i)
                break

        # Len comparison is obviously much faster, but some functions need the set of facets that are untouched so this is used instead of copying code
        if setOutput:
            return self.completeSetOfFaces.difference(solutionSet)
        return len(self.completeSetOfFaces) - len(solutionSet)


    # Parameters:
        # inputSet: A set of verticies, offset: if the numbering of the verticies begins at 0, set to False, if at 1, set to True
    def facet_connections(self, inputSet, offset = True):
        output = set()
        for facet in inputSet:
            output.update(self.listOfFacetConnections[facet - offset])
        return output


    # Parameters: inputSet, a potential parent of solutions
    # Returns:
        # if not possible, return false,
        # if it is, return the untouched facets (so it will evaluate to true, and save another computation of untouched_facets)
    def possible_component_of_solution(self, inputSet):
        untouchedFacets = self.untouched_facets(inputSet)
        if untouchedFacets == set() and len(inputSet) <= self.targetLength:
            return set()
        if len(inputSet) >= self.targetLength:
            return False

        #if there are more untouchedFacets than its possible to cover with remaining vertecies to choose, not possible
        if len(untouchedFacets) / (self.targetLength - len(inputSet)) > self.maxFacetsPerVertex:
            return False
        return untouchedFacets


    # Parameters: inputSet: a potential solution, lastAddedNode: an int that keeps track of the last added node, to prevent duplicate groupings w/ simple loops.
    # Returns:
        # if the inputSet is a solution: return inputSet
        # if the inputSet is not a "parent" of any possible solutions: return
    # Note: As is, will return [] if no solutions are found
    def run(self, inputSet, lastAddedNode = -1):
        possible = self.possible_component_of_solution(inputSet)

        # if already a solution, return inputSet
        if possible == set():
            return inputSet
        #if impossible to complete from here, return nothing
        if not possible:
            return
        #else recurse w/ all possible
        #TODO translate from face to vetices
        potentialVerticies = set(self.facet_connections(possible))
        potentialVerticies.difference_update(inputSet)

        result = []
        for vertex in potentialVerticies:
            if int(vertex) <= lastAddedNode:
                continue #this should prevent (1, 2, 3), (1, 3, 2), (2, 1, 3).... and just have one set be done
            child = inputSet.union(set([vertex]))
            #print(child)
            result.append(self.run(child, lastAddedNode=int(vertex)))
        return remove_nones(flatten(result))

    # Removes all combinations of self.numNodesToRemove number of nodes, and passes the resulting sets onto run to see if any solutions exist.
        # Perhaps reworking to make it remove, say, 2x the intended #, and then adding them back in via run() would faster?
    def run_start(self, inputSet):
        print("starting inputSet:", inputSet)
        print("starting inputSetLen", len(inputSet))
        # go back X number of nodes in the search tree
        combos = combinations(inputSet, self.numNodesToRemove)
        result = []
        for nodesToRemove in combos:
            result.append(self.run(inputSet.difference(nodesToRemove)))
        result = remove_nones(flatten(result))
        print("results:", result)
        return result

    # Used to process a file of
    # Parameter: inputFilePath: str representing file path to where the solutions to be optimized are
    # Outputs: a csv containing all unique solutions of the target length that are found to wherever self.outputFilePath is set to
    def batch_run(self, inputFilePath):
        writtenLists = []
        if exists(inputFilePath):
            with open(inputFilePath, newline='') as readFile:
                reader = csv.reader(readFile)
                data = list(reader)
                with open(self.outputFilePath, 'w') as writeFile:
                    writer = csv.writer(writeFile)

                    for row in data:
                        #print("data", data)
                        #print("row", row)
                        listRow = list(row)
                        setRow = set()
                        for i in listRow:
                            setRow.add(int(i))
                        if self.autoInclude != None:
                            setRow.update([i for i in range(1,self.autoInclude+1)])
                        print("setRow", setRow)
                        if len(setRow) == self.targetLength:
                            print("Solution is already \"optimal\"")
                            continue
                        if len(setRow) > self.maxAcceptableStartingSetSize:
                            print("Input solution is too large to try to deal with")
                            continue
                        resultList = self.run_start(setRow)
                        for result in resultList:
                            if result not in writtenLists and result != []:
                                print("Writing",result)
                                writer.writerow(result)
                                writtenLists.append(result)
        else:
            print("Couldn't find that file")
        return

# Originally from: https://discourse.mcneel.com/t/python-flatten-an-irregular-list-of-list-of-list/4398/3
# It converts a list of lists down to 1 dimensions, regardless of previous dimensionality, or inconsistencies
def flatten(lst):
    return sum( ([x] if not isinstance(x, list) else flatten(x) for x in lst), [] )

# Parameters: lst, a list that may contain None values
# Returns: a list containing all of lst's values in the same order, excluding None values
def remove_nones(lst):
    return [x for x in lst if x is not None]

# Commandline Arguements:
# Parameter 1: Filepath to the csv of the shape
# Parameter 2: Filepath to the csv of the (bad) solutions
if __name__ == '__main__':
    args = sys.argv[1:]
    backtrackerObj = Backtracker()
    backtrackerObj.fit_shape_data(args[0]) #must be ran before any of the other functions otherwise they have nothing to work with
    backtrackerObj.batch_run(args[1])
