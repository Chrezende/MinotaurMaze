(define (domain minotaur-maze)

  (:requirements 
    :adl
  )

  (:types
    tile - tile
    key - object
    bomb - object
    object
    player - agent
    minotaur - agent
    agent
  )

  (:predicates
    (atTile ?agent - agent ?tile - tile)
    (neighbour ?tile1 - tile ?tile2 - tile)
    (visited ?tile - tile)
    (tileSafe ?tile - tile)
    (objectOnMap ?object - object ?tile - tile)
    (haveObject ?object - object)
    (passage ?tile1 - tile ?tile2 - tile)
    (isAlive ?agent - agent)
  ) 

  (:action moveTo
  	;; Move jogador (?player) do tile de origem (?from) para o tile de destino (?to)
    ;; somente se os tiles foram vizinhos e o minotauro não esteja no destino
    :parameters   (?player - player ?from - tile ?to - tile ?minotaur - minotaur)
    :precondition (and  (atTile ?player ?from) (neighbour ?from ?to)
                        (or (not (isAlive ?minotaur))
                            (not (atTile ?minotaur ?to))
                        )
                  )
    :effect (and  (not (atTile ?player ?from))
                  (atTile ?player ?to)
                  (when (not(visited ?to)) (visited ?to)) 
            )
  )

  (:action pick
    ;; Pega o ?objeto que está no tile ?location
    :parameters   (?player - player ?object - object ?location - tile)
    :precondition (and (atTile ?player ?location) (objectOnMap ?object ?location))
    :effect (and  (not (objectOnMap ?object ?location))
                  (haveObject ?object)
            )
  )

  (:action usePassage
    ;; Usa passagem do tile de origem (?from) para o destino (?to)
    ;; não é obrigatórioos tiles serem vizinhos (teleporte)
    :parameters   (?player - player ?from - tile ?to - tile ?key - key)
    :precondition (and  (haveObject ?key)
                        (atTile ?player ?from)
                        (passage ?from ?to)
                  )
    :effect (and  (not (atTile ?player ?from))
                  (not (haveObject ?key)) 
                  (atTile ?player ?to)
                  (when (not(visited ?to)) (visited ?to))
            )
  )

  (:action throwBomb
    ;; Joga uma bomba no tile (?tile) que explode em cruz (+), matando o minotauro
    ;; é obrigatório o agente ter a bomba e o minotauro estar vivo ainda
    :parameters (?tile - tile ?bomb - bomb ?minotaur - minotaur ?player - player ?pTile - tile)
    :precondition (and  (haveObject ?bomb)
                        (isAlive ?minotaur)
                        (atTile ?player ?pTile)
                        (exists (?pNeighbour - tile)
                                (and  (neighbour ?pTile ?pNeighbour)
                                      (neighbour ?pNeighbour ?tile)
                                )
                        )
                        (or (atTile ?minotaur ?tile)
                            (exists (?mNeighbour - tile) 
                                    (and  (neighbour ?tile ?mNeighbour)
                                          (atTile ?minotaur ?mNeighbour)
                                    )
                            )
                        ) 
                  )
    :effect (and  (not  (haveObject ?bomb))
                  (not  (isAlive ?minotaur))
            )
  )

)