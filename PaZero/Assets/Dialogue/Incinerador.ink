-> main

=== main ===
Entrar no incinerador?
	+ [Sim]
		-> chosen("Sim")
	+ [Não]
		-> chosen("Não")

=== chosen(option) ===

{ option: 
- "Sim": Você se ajoelha para entrar.

- "Não": Você se afasta, tentando lembrar se esqueceu de alguma coisa.

}


-> DONE
