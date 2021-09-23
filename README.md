# BUPlatonicSolids
Everybody can start adding anything they are working on to this repository, if possible (?)

# GreedyCoverer
Hi Platonics, this is Robert. GreedyCoverer is what I've been working on, it's an attempt to implement the ideas we discussed. When it is done it will be an algorithm that takes 3d shape data, and adds the most connected vertex to the 'covering set', until the covering set covers all the faces in the shape. If two vertices are connected to the same amount of faces, the winner will be the one who is closer to any vertex in the covering set. <- That part is not implemented yet. Could we use some variation of a shortest path algorithm to find that?

It returns the covering set.

Currently it is returning too many vertices in the covering set, not sure why, that is something I will be looking at.

I made it in PyCharm, a nice python editing tool, so that is what the venv folder is, but the .py file(s) should run just fine on their own anywhere.

I haven't split things into different files at all, that might be helpful later on to read it easier...

# Platonic Solids To Do's
- GreedyCoverer: get_shortest_path_len_to_any_in_set()
- GreedyCoverer: read shape data from file
- somehow link with Unity
- 

# How to clone/push to this repository
You can push and clone by making a personal access token (github profile icon > Settings > Developer Settings > Personal Access Token > Generate (just check All repo)). Then copy it and save it somewhere, and paste it in place of your password whenever prompted in the terminal, for example while doing git push -u origin develop. That's not a very good explanation I guess
