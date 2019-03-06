
Para realizar a simulação através da aplicação desenvolvida, basta utilizar os executáveis compilados do projeto ElevatorSimulator.Sample.
Siga as seguintes etapas:

	1. Executar primeiro a aplicação 'ElevatorSimulator.ExternalService.exe' (servidor das chamadas externas);
	2. Executar em seguida a aplicação 'ElevatorSimulator.Sample.exe'.

Os executáveis foram disponibilizados no repositório do GitHub https://github.com/felipe-romao/projetoElevador/Simulator/. 

-------------------------------------------	
 Abaixo, segue uma resumo de cada aplicação:
-------------------------------------------
	ElevatorSimulator.ExternalService.exe:
	
		Trata-se de uma aplicação console, responsável em fornecer as chamadas extermas.
		Sua função para uso resume em inserir o andar.
		
		Possui um server (background) que fornece os andares inseridos pelo usuário, afim que o elevador vá até eles, simulando as chamadas externas.
		Esta aplicação utiliza seu arquivo de configuração para:
			- Host do server  : default 127.0.0.1
			- Porta do server : default 13000
		
		
		
	ElevatorSimulator.Sample.exe:
	
		Trata-se também de uma aplicação console, responsável em simular as operações do elevador.
		Basicamente as opções para uso são:
			- embarque, informando o peso;
			- criação da rota, informando os andares de destino;
			- desembarque, informando o peso;
			OBS: no desembarque, este a opção de um novo embarque. Basta informar o peso e a rota deste novo embarque. 
			
		Possui um service que consome o client da aplicação 'ElevatorSimulator.ExternalService.exe', obtendo as chamadas externa geradas dele.
		Esta aplicação utiliza seu arquivo de configuração para:
			- Host do serviço  : default 127.0.0.1
			- Porta do serviço : default 13000
			- Tipo do elevador : SOCIAL ou SERVICE


			
	
