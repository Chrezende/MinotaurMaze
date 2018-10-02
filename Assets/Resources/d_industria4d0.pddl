(define (domain industria4d0)

  (:requirements	:adl)

  (:predicates
		(em ?local - local)
		(defeito ?maquina - maquina)
		(localMaquina ?maquina - maquina ?local - local)
		(fazOperacao1 ?maquina - maquina)
		(fazOperacao2 ?maquina - maquina)
		(fazOperacao3 ?maquina - maquina)
		(fazOperacao4 ?maquina - maquina)
		(pecaCrua ?peca - peca)
		(pecaOp1 ?peca - peca) (pecaOp2 ?peca - peca)
		(pecaOp3 ?peca - peca) (pecaOp24 ?peca - peca)
		(pecaOp34 ?peca - peca)
		(entregueA ?peca - peca ?local - local)
		(entregueB ?peca - peca ?local - local)
  ) 

  (:action moverPara
		;; Move robô do local de origem ?de para o local de destino ?para
		:parameters   (?de - local ?para - local)
		:precondition (and
											(em ?de)
									)
		:effect (and  
								(not (em ?de))
								(em ?para)
						)
	)

  (:action fazerOp1
		:parameters (?maquina - maquina ?local - local ?peca - peca)
		:precondition (and 
											(fazOperacao1 ?maquina)
											(localMaquina ?maquina ?local)
											(em ?local)
											(pecaCrua ?peca)
											(not (defeito ?maquina))
									)
		:effect (and
								(not (pecaCrua ?peca))
								(pecaOp1 ?peca)
						)
	)

  (:action fazerOp2
		:parameters (?maquina - maquina ?local - local ?peca - peca)
		:precondition (and 
											(fazOperacao2 ?maquina)
											(localMaquina ?maquina ?local)
											(em ?local)
											(pecaOp1 ?peca)
											(not (defeito ?maquina))
									)
		:effect (and
								(not (pecaOp1 ?peca))
								(pecaOp2 ?peca)
						)
	)

  (:action fazerOp3
		:parameters (?maquina - maquina ?local - local ?peca - peca)
		:precondition (and 
											(fazOperacao3 ?maquina)
											(localMaquina ?maquina ?local)
											(em ?local)
											(pecaOp1 ?peca)
											(not (defeito ?maquina))
									)
		:effect (and
								(not (pecaOp1 ?peca))
								(pecaOp3 ?peca)
						)
	)

  (:action fazerOp4
		:parameters (?maquina - maquina ?local - local ?peca - peca)
		:precondition (and 
											(fazOperacao4 ?maquina)
											(localMaquina ?maquina ?local)
											(em ?local)
											(or (pecaOp2 ?peca)
													(pecaOp3 ?peca)
											)
											(not (defeito ?maquina))
									)
		:effect (and
								(when (pecaOp2 ?peca) 
											(and	(not (pecaOp2 ?peca))
														(pecaOp24 ?peca)
											)
								)
								(when (pecaOp3 ?peca) 
											(and	(not (pecaOp3 ?peca))
														(pecaOp34 ?peca)
											)
								)
						)
	)

	(:action entregarA
	  :parameters (?peca - peca ?local - local)
	  :precondition (and 
	  									(pecaOp34 ?peca)
	  									(em ?local)
	  							)
	  :effect (and
	  						(entregueA ?peca ?local)
	  						(not (pecaOp34 ?peca))
	  				)
	)

	(:action entregarB
	  :parameters (?peca - peca ?local - local)
	  :precondition (and 
	  									(pecaOp24 ?peca)
	  									(em ?local)
	  							)
	  :effect (and
	  						(entregueB ?peca ?local)
	  						(not (pecaOp24 ?peca))
	  				)
	)
)
