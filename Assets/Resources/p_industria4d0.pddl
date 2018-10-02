(define (problem problem0)
	
	(:domain industria4d0)

	(:objects
		m1 m2 m3 m4 - maquina
		inicio locm1 locm2 locm3 locm4 armarioA armarioB - local
		p1 p2 p3 p4 - peca
	)

	(:init 
		(em inicio)
		(localMaquina m1 locm1) (localMaquina m2 locm2)
		(localMaquina m3 locm3) (localMaquina m4 locm4)
		(fazOperacao1 m1) (fazOperacao2 m2)
		(fazOperacao3 m3) (fazOperacao4 m3)
		(fazOperacao1 m4) (fazOperacao4 m4)
		(defeito m1)
		(pecaCrua p1) (pecaCrua p2) (pecaCrua p3) (pecaCrua p4)
	)

	(:goal (and
				(entregueA p1 armarioA) (entregueB p2 armarioB)
				(entregueB p3 armarioB) (entregueA p4 armarioA)
				(em inicio)
			)
	)
)