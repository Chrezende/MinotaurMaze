(define (problem maze)
	
	(:domain minotaur-maze)

	(:objects
		l1c1 - tile
		
		minotaur - minotaur
		l1c3 - tile		
		player - player
		l2c1 - tile		l2c2 - tile		l2c3 - tile		l3c1 - tile
		exit - tile		
		bomb - bomb
			
	)

	(:init
		(neighbour l1c1 l2c1)	
		(neighbour l1c3 l2c3)	
		(neighbour l2c1 l1c1)	(neighbour l2c1 l3c1)	(neighbour l2c1 l2c2)	
		(neighbour l2c2 l2c1)	(neighbour l2c2 l2c3)	
		(neighbour l2c3 l1c3)	(neighbour l2c3 l2c2)	
		(neighbour l3c1 l2c1)	(neighbour l3c1 exit)	
		(neighbour exit l3c1)	
		(visited l1c3)	
		(tileSafe l1c1)	
		(tileSafe l1c3)	
		(tileSafe l2c1)	
		(tileSafe l2c2)	
		(tileSafe l2c3)	
		(tileSafe l3c1)	
		(tileSafe exit)	
		(atTile player l1c3)	
		(atTile minotaur l2c1)	
		(isAlive player)	
		(isAlive minotaur)	
		(haveObject bomb)	
		
	)

	(:goal
		(and (atTile player exit) )
	)
)
