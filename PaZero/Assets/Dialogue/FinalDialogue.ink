-> main

=== main ===
Uma sensação estranha invade sua cabeça ao observar este corpo no chão. Você evade o olhar com repulsão, como se a cena te lembrasse de alguma coisa horrível. Mas... será que você devia olhar mais perto?
	+ [Sim]
		-> chosen("Sim")
	+ [Não]
		-> chosen("Não")

=== chosen(option) ===

{ option: 
- "Sim": Você decide investigar.

- "Não": Você ainda não sentiu necessidade.

}


-> DONE
