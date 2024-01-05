#  Booker API Testing
Este projeto é o segundo desafio individual do curso de Formação em Teste de Software da Iterasys relacionado aos teste de API em C#
O desafio proposto foi Utilizando a API Restful-booker fazer testes para cada requisição. 
Os testes contém: 
- Inicia com a função `getTestData` que é uma função de leitura de dados que será utilizada para o testes de massa CSV. 
- O primeiro teste `PostCreateTokenTest` cria o Token e salva para utilizar posteriormente. 
- O segundo teste `PostBookingTest` cria uma reserva a partir dos dados da reserva1.json e valida o retorno da resposta além de salvar o bookingid para usar posteriomente. 
- O terceiro teste `GetBookingsIdsTest` retorna todos os ids das reservas e valida que o id da requisição PostCreateTokenTest consta na lista dos ids retornados. 
- O quarto teste `GetBookingsTest` retorna o dados da reserva criada no `PostBookingTest` e valida o preço e o additionalneeds. 
- O quinto teste `UpdateBookingTest` realiza update da reserva bookingId criada no PostCreateTokenTest através dos dados passado no arquivo updated_reserva.json e valida se os dados foram alterados corretamente.
- O sexto teste `PartialUpdateBookingTest` realiza update parcial da reserva bookingId criada no PostCreateTokenTest passando valores no proprio corpo da requisição, alterando o firstName e o lastName e validando a lateração. 
- O setimo teste `DeleteBookingTest` deleta a reserva criada no PostCreateTokenTest e valida se o código foi 201 a o termo Created.
- O oitavo teste `HealthCheckTest` realiza uma checagem na api e verifica o retorno com sucesso retornando codigo 201 e a palavra Created.
- O nono teste `postBookingDDTest` realiza testes utilizando Data - Driven, lendo o a massa de testes que esta no arquivo reserva.csv, onde contem 10 reservas. Nesse testes foi utilizado a classe modelo  BookingsModel, para deixar a estrutura de dados mais organizada, legivel e poder ser reutilizada em outros testes, caso acha necessidade. 

OBS. Para os testes Update e Delete é necessario usar o token salvo no teste  `PostCreateTokenTest `.


## Endereço e documentação da API 

https://restful-booker.herokuapp.com/apidoc/index.html


## Estrutura do Projeto

- `Models`: Contém classes de modelo para representar entidades da API.
- `Tests`: Contém os testes NUnit para a API.
- `Fixtures/`:  Armazena arquivos de dados para testes.

##  Bibliotecas


1 - Add o pacote Nunit comando => dotnet new nunit
2 - Add pacote Rest => dotnet add package RestSharp


2. **Clonar o Repositório:**
   ```bash
    git clone https://github.com/welitaluisa/nunit-booker.git
    ```

## Comando para rodar os testes
- dotnet test -v n  // modo de log verboso
